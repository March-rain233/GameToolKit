using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;
using System;
using GameToolKit.Utility;
using System.Reflection;

namespace GameToolKit
{
    public abstract class BaseNode : IDisposable
    {
        #region 编辑器相关成员
#if UNITY_EDITOR
        /// <summary>
        /// 名称
        /// </summary>
        [HideInGraphInspector]
        public string Name;
        /// <summary>
        /// 视图位置
        /// </summary>
        [HideInGraphInspector]
        public Vector2 ViewPosition;
#endif
        #endregion

        #region 字段与属性
        /// <summary>
        /// 唯一标识符
        /// </summary>
        [HideInGraphInspector, OdinSerialize]
        public int Id { get; internal protected set; }

        /// <summary>
        /// 输入边信息
        /// </summary>
        public IReadOnlyList<SourceEdge> InputEdges => _inputEdges;
        [SerializeField, HideInGraphInspector]
        private List<SourceEdge> _inputEdges = new List<SourceEdge>();

        /// <summary>
        /// 输出边信息
        /// </summary>
        public IReadOnlyList<SourceEdge> OutputEdges => _outputEdges;
        [SerializeField, HideInGraphInspector]
        private List<SourceEdge> _outputEdges = new List<SourceEdge>();

        /// <summary>
        /// 节点是否已初始化
        /// </summary>
        public bool HasInitialized { get; private set; } = false;

        /// <summary>
        /// 当节点为脏
        /// </summary>
        public Action OnDirty;

        private bool _disposedValue = false;
        #endregion

        #region 数据传输相关方法
        /// <summary>
        /// 增加输入边
        /// </summary>
        /// <param name="sourceNode"></param>
        /// <param name="sourceField"></param>
        /// <param name="targetField"></param>
        public void AddInputEdge(BaseNode sourceNode, string sourceField, string targetField)
        {
            SourceEdge edge = new SourceEdge(sourceNode, this, sourceField, targetField);
            _inputEdges.Add(edge);
            if(HasInitialized)
                edge.RegisterDirtyHandler();
        }

        /// <summary>
        /// 增加输出边
        /// </summary>
        /// <param name="targetNode"></param>
        /// <param name="sourceField"></param>
        /// <param name="targetField"></param>
        public void AddOutputEdge(BaseNode targetNode, string sourceField, string targetField)
        {
            SourceEdge edge = new SourceEdge(this, targetNode, sourceField, targetField);
            _outputEdges.Add(edge);
        }

        /// <summary>
        /// 移除输入边
        /// </summary>
        /// <param name="sourceNode"></param>
        /// <param name="sourceField"></param>
        /// <param name="targetField"></param>
        public void RemoveInputEdge(BaseNode sourceNode, string sourceField, string targetField)
        {
            for(int i = _inputEdges.Count - 1; i >= 0; --i)
            {
                if(_inputEdges[i].SourceNode == sourceNode &&
                    _inputEdges[i].SourceField == sourceField &&
                    _inputEdges[i].TargetField == targetField)
                {
                    _inputEdges[i].UnregisterDirtyHandler();
                    _inputEdges.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// 移除输入边
        /// </summary>
        /// <param name="sourceNode"></param>
        public void RemoveInputEdge(BaseNode sourceNode)
        {
            for (int i = _inputEdges.Count - 1; i >= 0; --i)
            {
                if (_inputEdges[i].SourceNode == sourceNode)
                {
                    _inputEdges[i].UnregisterDirtyHandler();
                    _inputEdges.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 移除输出边
        /// </summary>
        /// <param name="sourceNode"></param>
        /// <param name="sourceField"></param>
        /// <param name="targetField"></param>
        public void RemoveOutputEdge(BaseNode sourceNode, string sourceField, string targetField)
        {
            for (int i = _outputEdges.Count - 1; i >= 0; --i)
            {
                if (_outputEdges[i].SourceNode == sourceNode &&
                    _outputEdges[i].SourceField == sourceField &&
                    _outputEdges[i].TargetField == targetField)
                {
                    _outputEdges.RemoveAt(i);
                    return;
                }
            }
        }
        
        /// <summary>
        /// 移除输出边
        /// </summary>
        /// <param name="sourceNode"></param>
        /// <param name="sourceField"></param>
        /// <param name="targetField"></param>
        public void RemoveOutputEdge(BaseNode sourceNode)
        {
            for (int i = _outputEdges.Count - 1; i >= 0; --i)
            {
                if (_outputEdges[i].SourceNode == sourceNode)
                {
                    _outputEdges.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// 设置脏标记
        /// </summary>
        internal void SetDirty()
        {
            OnDirty?.Invoke();
        }

        /// <summary>
        /// 设置指定成员的值
        /// </summary>
        /// <remarks>
        /// 默认通过反射实现，如果想优化性能请重载
        /// </remarks>
        /// <param name="name">成员名称</param>
        /// <param name="value">值</param>
        protected virtual void SetValue(string name, object value)
        {
            var list = name.Split('.');
            FieldInfo field = TypeUtility.GetField(list[0], GetType(), typeof(BaseNode));
            object temp = this;
            for (int i = 1; i < list.Length; i++)
            {
                temp = field.GetValue(temp);
                field = TypeUtility.GetField(list[i], field.FieldType);
            }
            field.SetValue(temp, value);
        }

        /// <summary>
        /// 获取指定成员的值
        /// </summary>
        /// <remarks>
        /// 默认通过反射实现，如果想优化性能请重载
        /// </remarks>
        /// <param name="name">成员名称</param>
        protected virtual object GetValue(string name)
        {
            var list = name.Split('.');
            FieldInfo field = TypeUtility.GetField(list[0], GetType(), typeof(BaseNode));
            object temp = this;
            for (int i = 1; i < list.Length; i++)
            {
                temp = field.GetValue(temp);
                field = TypeUtility.GetField(list[i], field.FieldType);
            }
            return field.GetValue(temp);
        }

        /// <summary>
        /// 拉取指定字段的值
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        protected virtual object PullValue(string fieldName)
        {
            Refresh();
            return GetValue(fieldName);
        }

        /// <summary>
        /// 推送指定字段的值
        /// </summary>
        protected virtual void PushValue(string fieldName, object value)
        {
            SetValue(fieldName, value);
            Refresh();
        }

        /// <summary>
        /// 执行数据更新逻辑
        /// </summary>
        protected abstract void OnValueUpdate();

        /// <summary>
        /// 拉取输入的数据
        /// </summary>
        protected void InitInputData()
        {
            foreach (var edge in InputEdges.Where(e => e.IsDirty))
            {
                var obj = edge.SourceNode.PullValue(edge.SourceField);
                SetValue(edge.TargetField, obj);
                edge.ClearDirty();
            }
        }

        /// <summary>
        /// 推送输出的数据
        /// </summary>
        protected void InitOutputData()
        {
            foreach (var edge in OutputEdges)
            {
                edge.TargetNode.PushValue(edge.TargetField, GetValue(edge.SourceField));
            }
        }

        /// <summary>
        /// 刷新节点数据状态
        /// </summary>
        public virtual void Refresh()
        {
            InitInputData();
            OnValueUpdate();
            InitOutputData();
        }
        #endregion

        #region 编辑器相关方法
#if UNITY_EDITOR
        /// <summary>
        /// 获取合法的端口列表
        /// </summary>
        /// <returns></returns>
        public virtual List<PortData> GetValidPortDataList()
        {
            var list = new List<PortData>();
            var fields = TypeUtility.GetAllField(GetType(), typeof(BaseNode))
                .Where(field => field.IsDefined(typeof(PortAttribute), true));
            foreach (var field in fields)
            {
                var attrs = field.GetCustomAttributes(typeof(PortAttribute), true);
                foreach (PortAttribute attr in attrs)
                {
                    if (attr.IsMemberFields)
                    {
                        var children = TypeUtility.GetAllField(field.FieldType);
                        foreach (var child in children)
                        {
                            var data = new PortData();
                            data.PreferredType = child.FieldType;
                            data.NickName = child.Name;
                            data.Name = $"{field.Name}.{child.Name}";
                            data.PortDirection = attr.Direction;
                            data.PortTypes = new HashSet<Type>() { data.PreferredType };
                            list.Add(data);
                        }
                    }
                    else
                    {
                        var data = new PortData();
                        data.PreferredType = field.FieldType;
                        data.NickName = attr.Name;
                        data.Name = field.Name;
                        data.PortDirection = attr.Direction;
                        data.PortTypes = new HashSet<Type>() { data.PreferredType };
                        if (attr.ExtendPortTypes != null)
                        {
                            data.PortTypes.UnionWith(attr.ExtendPortTypes);
                        }
                        list.Add(data);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 当字段改名后用来重新校对字段名称
        /// </summary>
        /// <param name="oldField"></param>
        /// <returns></returns>
        public virtual string FixPortIndex(string oldField) => null;

        public struct PortData
        {
            /// <summary>
            /// 字段的名称
            /// </summary>
            public string Name;
            /// <summary>
            /// 字段的显示名称
            /// </summary>
            public string NickName;
            /// <summary>
            /// 端口数据流动方向
            /// </summary>
            public PortDirection PortDirection;
            /// <summary>
            /// 端口可匹配的数据类型
            /// </summary>
            public HashSet<Type> PortTypes;
            /// <summary>
            /// 端口的首选数据类型
            /// </summary>
            public Type PreferredType;
        }
#endif
        #endregion

        #region 生命周期
        /// <summary>
        /// 克隆当前节点（仅克隆节点内部信息，与其他节点交互的信息由图实现
        /// </summary>
        /// <remarks>
        /// 默认为浅拷贝，需要深拷贝时请进行重载
        /// </remarks>
        public virtual BaseNode Clone()
        {
            var node = MemberwiseClone() as BaseNode;
            node.Id = Id;
            node._inputEdges = new List<SourceEdge>();
            node._outputEdges = new List<SourceEdge>();
            return node;
        }

        public void Init()
        {
            OnInit();
            HasInitialized = true;
        }

        protected virtual void OnInit()
        {
            foreach (var edge in _inputEdges)
                edge.RegisterDirtyHandler();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    foreach(var e in _inputEdges)
                        e.UnregisterDirtyHandler();
                    _inputEdges.Clear();
                    _outputEdges.Clear();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                _disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        ~BaseNode()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

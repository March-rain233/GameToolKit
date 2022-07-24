using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;
using System;

namespace GameFrame
{
    public abstract class BaseNode
    {
        #region 编辑器相关成员
#if UNITY_EDITOR
        /// <summary>
        /// 名称
        /// </summary>
        [HideInTreeInspector]
        public string Name;
        /// <summary>
        /// 视图位置
        /// </summary>
        [HideInTreeInspector]
        public Vector2 ViewPosition;
#endif
        #endregion

        #region 字段与属性
        /// <summary>
        /// 唯一标识符
        /// </summary>
        [HideInTreeInspector]
        public string Guid;
        /// <summary>
        /// 输入边信息
        /// </summary>
        [SerializeField]
        [HideInTreeInspector]
        public List<SourceInfo> InputEdges = new List<SourceInfo>();
        /// <summary>
        /// 输出边信息
        /// </summary>
        [SerializeField]
        [HideInTreeInspector]
        public List<SourceInfo> OutputEdges = new List<SourceInfo>();
        /// <summary>
        /// 上一次数据更新时间
        /// </summary>
        [ReadOnly]
        [OdinSerialize]
        public float LastDataUpdataTime { get; protected set; } = 0;
        #endregion

        #region 数据传输相关方法
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
            var type = GetType();
            do
            {
                var field = type.GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(this, value);
                    break;
                }
                type = type.BaseType;
            } while (type != typeof(BaseNode));
        }
        /// <summary>
        /// 获取指定成员的值
        /// </summary>
        /// <remarks>
        /// 默认通过反射实现，如果想优化性能请重载
        /// </remarks>
        /// <param name="name">成员名称</param>
        /// <returns>值</returns>
        protected virtual object GetValue(string name)
        {
            var type = GetType();
            do
            {
                var field = type.GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    return field.GetValue(this);
                }
                type = type.BaseType;
            } while (type != typeof(BaseNode));
            return default;
        }
        /// <summary>
        /// 拉取指定字段的值
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        protected virtual object PullValue(string fieldName)
        {
            if (LastDataUpdataTime != Time.time)
            {
                InitInputData();
                OnValueUpdate();
                LastDataUpdataTime = Time.time;
            }
            return GetValue(fieldName);
        }

        /// <summary>
        /// 推送指定字段的值
        /// </summary>
        /// <param name="fieldName"></param>
        protected virtual void PushValue(string fieldName, object value)
        {
            SetValue(fieldName, value);
            OnValueUpdate();
            InitOutputData();
        }

        /// <summary>
        /// 执行数据更新逻辑
        /// </summary>
        protected abstract void OnValueUpdate();

        /// <summary>
        /// 拉取输入的数据
        /// </summary>
        protected virtual void InitInputData()
        {
            foreach (var edge in InputEdges)
            {
                var obj = edge.SourceNode.PullValue(edge.SourceField);
                SetValue(edge.TargetField, obj);
            }
        }

        /// <summary>
        /// 推送输出的数据
        /// </summary>
        protected virtual void InitOutputData()
        {
            foreach (var edge in OutputEdges)
            {
                edge.TargetNode.PushValue(edge.TargetField, GetValue(edge.SourceField));
            }
        }

        /// <summary>
        /// 获取合法的端口列表
        /// </summary>
        /// <returns></returns>
        public virtual List<PortData> GetValidPortDataList()
        {
            var list = new List<PortData>();
            var type = GetType();
            while (type != typeof(BaseNode))
            {
                var portField = type.GetFields(
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
                    ).Where(field => field.IsDefined(typeof(PortAttribute), true) && field.DeclaringType == type);
                foreach (var field in portField)
                {
                    var data = new PortData();
                    PortAttribute attr = field.GetCustomAttributes(typeof(PortAttribute), true)[0] as PortAttribute;
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
                type = type.BaseType;
            }
            return list;
        }
        #endregion
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
            public Direction PortDirection;
            /// <summary>
            /// 端口可匹配的数据类型
            /// </summary>
            public HashSet<Type> PortTypes;
            /// <summary>
            /// 端口的首选数据类型
            /// </summary>
            public Type PreferredType;
        }
    }
}

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
        #region �༭����س�Ա
#if UNITY_EDITOR
        /// <summary>
        /// ����
        /// </summary>
        [HideInGraphInspector]
        public string Name;
        /// <summary>
        /// ��ͼλ��
        /// </summary>
        [HideInGraphInspector]
        public Vector2 ViewPosition;
#endif
        #endregion

        #region �ֶ�������
        /// <summary>
        /// Ψһ��ʶ��
        /// </summary>
        [HideInGraphInspector, OdinSerialize]
        public int Id { get; internal protected set; }

        /// <summary>
        /// �������Ϣ
        /// </summary>
        public IReadOnlyList<SourceEdge> InputEdges => _inputEdges;
        [SerializeField, HideInGraphInspector]
        private List<SourceEdge> _inputEdges = new List<SourceEdge>();

        /// <summary>
        /// �������Ϣ
        /// </summary>
        public IReadOnlyList<SourceEdge> OutputEdges => _outputEdges;
        [SerializeField, HideInGraphInspector]
        private List<SourceEdge> _outputEdges = new List<SourceEdge>();

        /// <summary>
        /// �ڵ��Ƿ��ѳ�ʼ��
        /// </summary>
        public bool HasInitialized { get; private set; } = false;

        /// <summary>
        /// ���ڵ�Ϊ��
        /// </summary>
        public Action OnDirty;

        private bool _disposedValue = false;
        #endregion

        #region ���ݴ�����ط���
        /// <summary>
        /// ���������
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
        /// ���������
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
        /// �Ƴ������
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
        /// �Ƴ������
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
        /// �Ƴ������
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
        /// �Ƴ������
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
        /// ��������
        /// </summary>
        internal void SetDirty()
        {
            OnDirty?.Invoke();
        }

        /// <summary>
        /// ����ָ����Ա��ֵ
        /// </summary>
        /// <remarks>
        /// Ĭ��ͨ������ʵ�֣�������Ż�����������
        /// </remarks>
        /// <param name="name">��Ա����</param>
        /// <param name="value">ֵ</param>
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
        /// ��ȡָ����Ա��ֵ
        /// </summary>
        /// <remarks>
        /// Ĭ��ͨ������ʵ�֣�������Ż�����������
        /// </remarks>
        /// <param name="name">��Ա����</param>
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
        /// ��ȡָ���ֶε�ֵ
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        protected virtual object PullValue(string fieldName)
        {
            Refresh();
            return GetValue(fieldName);
        }

        /// <summary>
        /// ����ָ���ֶε�ֵ
        /// </summary>
        protected virtual void PushValue(string fieldName, object value)
        {
            SetValue(fieldName, value);
            Refresh();
        }

        /// <summary>
        /// ִ�����ݸ����߼�
        /// </summary>
        protected abstract void OnValueUpdate();

        /// <summary>
        /// ��ȡ���������
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
        /// �������������
        /// </summary>
        protected void InitOutputData()
        {
            foreach (var edge in OutputEdges)
            {
                edge.TargetNode.PushValue(edge.TargetField, GetValue(edge.SourceField));
            }
        }

        /// <summary>
        /// ˢ�½ڵ�����״̬
        /// </summary>
        public virtual void Refresh()
        {
            InitInputData();
            OnValueUpdate();
            InitOutputData();
        }
        #endregion

        #region �༭����ط���
#if UNITY_EDITOR
        /// <summary>
        /// ��ȡ�Ϸ��Ķ˿��б�
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
        /// ���ֶθ�������������У���ֶ�����
        /// </summary>
        /// <param name="oldField"></param>
        /// <returns></returns>
        public virtual string FixPortIndex(string oldField) => null;

        public struct PortData
        {
            /// <summary>
            /// �ֶε�����
            /// </summary>
            public string Name;
            /// <summary>
            /// �ֶε���ʾ����
            /// </summary>
            public string NickName;
            /// <summary>
            /// �˿�������������
            /// </summary>
            public PortDirection PortDirection;
            /// <summary>
            /// �˿ڿ�ƥ�����������
            /// </summary>
            public HashSet<Type> PortTypes;
            /// <summary>
            /// �˿ڵ���ѡ��������
            /// </summary>
            public Type PreferredType;
        }
#endif
        #endregion

        #region ��������
        /// <summary>
        /// ��¡��ǰ�ڵ㣨����¡�ڵ��ڲ���Ϣ���������ڵ㽻������Ϣ��ͼʵ��
        /// </summary>
        /// <remarks>
        /// Ĭ��Ϊǳ��������Ҫ���ʱ���������
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

                // TODO: �ͷ�δ�йܵ���Դ(δ�йܵĶ���)����д�ս���
                // TODO: �������ֶ�����Ϊ null
                _disposedValue = true;
            }
        }

        // // TODO: ������Dispose(bool disposing)��ӵ�������ͷ�δ�й���Դ�Ĵ���ʱ������ս���
        ~BaseNode()
        {
            // ��Ҫ���Ĵ˴��롣�뽫���������롰Dispose(bool disposing)��������
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // ��Ҫ���Ĵ˴��롣�뽫���������롰Dispose(bool disposing)��������
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

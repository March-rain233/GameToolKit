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
        #region �༭����س�Ա
#if UNITY_EDITOR
        /// <summary>
        /// ����
        /// </summary>
        [HideInTreeInspector]
        public string Name;
        /// <summary>
        /// ��ͼλ��
        /// </summary>
        [HideInTreeInspector]
        public Vector2 ViewPosition;
#endif
        #endregion

        #region �ֶ�������
        /// <summary>
        /// Ψһ��ʶ��
        /// </summary>
        [HideInTreeInspector]
        public string Guid;
        /// <summary>
        /// �������Ϣ
        /// </summary>
        [SerializeField]
        [HideInTreeInspector]
        public List<SourceInfo> InputEdges = new List<SourceInfo>();
        /// <summary>
        /// �������Ϣ
        /// </summary>
        [SerializeField]
        [HideInTreeInspector]
        public List<SourceInfo> OutputEdges = new List<SourceInfo>();
        /// <summary>
        /// ��һ�����ݸ���ʱ��
        /// </summary>
        [ReadOnly]
        [OdinSerialize]
        public float LastDataUpdataTime { get; protected set; } = 0;
        #endregion

        #region ���ݴ�����ط���
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
        /// ��ȡָ����Ա��ֵ
        /// </summary>
        /// <remarks>
        /// Ĭ��ͨ������ʵ�֣�������Ż�����������
        /// </remarks>
        /// <param name="name">��Ա����</param>
        /// <returns>ֵ</returns>
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
        /// ��ȡָ���ֶε�ֵ
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
        /// ����ָ���ֶε�ֵ
        /// </summary>
        /// <param name="fieldName"></param>
        protected virtual void PushValue(string fieldName, object value)
        {
            SetValue(fieldName, value);
            OnValueUpdate();
            InitOutputData();
        }

        /// <summary>
        /// ִ�����ݸ����߼�
        /// </summary>
        protected abstract void OnValueUpdate();

        /// <summary>
        /// ��ȡ���������
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
        /// �������������
        /// </summary>
        protected virtual void InitOutputData()
        {
            foreach (var edge in OutputEdges)
            {
                edge.TargetNode.PushValue(edge.TargetField, GetValue(edge.SourceField));
            }
        }

        /// <summary>
        /// ��ȡ�Ϸ��Ķ˿��б�
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
            public Direction PortDirection;
            /// <summary>
            /// �˿ڿ�ƥ�����������
            /// </summary>
            public HashSet<Type> PortTypes;
            /// <summary>
            /// �˿ڵ���ѡ��������
            /// </summary>
            public Type PreferredType;
        }
    }
}

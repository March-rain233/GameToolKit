using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameToolKit
{
    /// <summary>
    /// �ڰ��������
    /// </summary>
    [Serializable]
    public abstract class BlackboardVariable
    {
        [Serializable]
        public delegate void VariableChangedHandler();

        /// <summary>
        /// ����ֵ
        /// </summary>
        public abstract object Value
        {
            get;
            set;
        }

        public bool IsReadOnly = false;

        public abstract Type TypeOfValue { get; }

        /// <summary>
        /// ������ֵ�仯ʱ�����¼�
        /// </summary>
        [SerializeField]
        public event VariableChangedHandler ValueChanged;

        /// <summary>
        /// ��¡����
        /// </summary>
        /// <remarks>
        /// Ĭ��Ϊǳ�������¼��Ķ����б����и���
        /// </remarks>
        /// <returns></returns>
        public virtual BlackboardVariable Clone()
        {
            var obj = MemberwiseClone() as BlackboardVariable;
            var list = obj.ValueChanged?.GetInvocationList();
            if (list != null)
                foreach (VariableChangedHandler item in list) 
                    obj.ValueChanged -= item;
            return obj;
        }

        protected void InvokeValueChanged()
        {
            ValueChanged?.Invoke();
        }
    }
}

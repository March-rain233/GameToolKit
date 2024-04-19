using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace GameToolKit
{
    /// <summary>
    /// �ڰ��������
    /// </summary>
    [Serializable]
    [GenericSelector("BaseValue")]
    public abstract class BlackboardVariable
    {
        public delegate void VariableChangedHandler();

        /// <summary>
        /// ����ֵ
        /// </summary>
        public abstract object Value
        {
            get;
            set;
        }

        public bool ReadOnly = false;

        public abstract Type TypeOfValue { get; }

        /// <summary>
        /// ������ֵ�仯ʱ�����¼�
        /// </summary>
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

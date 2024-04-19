using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace GameToolKit
{
    [GenericSelector("BaseValue")]
    public class GenericVariable<T> : BlackboardVariable
    {
        static readonly Type _typeOfValue = typeof(T);

        [SerializeField]
        [OnValueChanged("InvokeValueChanged")]
        [DisableIf("ReadOnly")]
        T _value;

        public override object Value 
        { 
            get => _value;
            set => SetValue((T)value);
        }

        public override Type TypeOfValue => _typeOfValue;

        /// <summary>
        /// ��ȡֵ
        /// </summary>
        /// <remarks>�����װ��</remarks>
        /// <returns></returns>
        public T GetValue() => _value;

        /// <summary>
        /// ����ֵ
        /// </summary>
        /// <remarks>�����װ��</remarks>
        /// <returns></returns>
        public void SetValue(T value)
        {
            if (ReadOnly)
            {
                Debug.LogError("An attempt was made to assign a value to a variable, but it is read-only");
                return;
            }
            _value = value;
            InvokeValueChanged();
        }
    }
}

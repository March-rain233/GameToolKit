using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameToolKit
{
    /// <summary>
    /// 黑板变量基类
    /// </summary>
    [Serializable]
    public abstract class BlackboardVariable
    {
        [Serializable]
        public delegate void VariableChangedHandler();

        /// <summary>
        /// 变量值
        /// </summary>
        public abstract object Value
        {
            get;
            set;
        }

        public bool IsReadOnly = false;

        public abstract Type TypeOfValue { get; }

        /// <summary>
        /// 当变量值变化时触发事件
        /// </summary>
        [SerializeField]
        public event VariableChangedHandler ValueChanged;

        /// <summary>
        /// 克隆变量
        /// </summary>
        /// <remarks>
        /// 默认为浅拷贝，事件的订阅列表不进行复制
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

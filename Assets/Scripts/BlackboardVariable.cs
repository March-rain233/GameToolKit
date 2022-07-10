using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame
{
    /// <summary>
    /// 黑板变量基类
    /// </summary>
    [System.Serializable]
    public abstract class BlackboardVariable
    {
        /// <summary>
        /// 变量值
        /// </summary>
        public abstract object Value
        {
            get;
            set;
        }
        public abstract BlackboardVariable Clone();
    }
}

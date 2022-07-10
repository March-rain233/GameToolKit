using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame
{
    /// <summary>
    /// �ڰ��������
    /// </summary>
    [System.Serializable]
    public abstract class BlackboardVariable
    {
        /// <summary>
        /// ����ֵ
        /// </summary>
        public abstract object Value
        {
            get;
            set;
        }
        public abstract BlackboardVariable Clone();
    }
}

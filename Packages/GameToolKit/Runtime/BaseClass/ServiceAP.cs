using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameToolKit
{
    /// <summary>
    /// �����������
    /// </summary>
    /// <remarks>
    /// �����ṩȫ�ֵķ�����ʵ�
    /// </remarks>
    public class ServiceAP : SingletonBase<ServiceAP>
    {
        /// <summary>
        /// ȫ�ֱ�����
        /// </summary>
        public Dictionary<string, BlackboardVariable> GlobalVariable;

        ServiceAP()
        {
            GlobalVariable = new Dictionary<string, BlackboardVariable>();
            
        }
    }
}

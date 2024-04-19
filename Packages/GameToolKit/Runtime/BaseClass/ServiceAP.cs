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
        public GlobalBlackboard GlobalBlackboard { get; protected set; }
        public Dialog.DialogManager DialogManager { get; protected set; }
        public PanelManager PanelManager { get; protected set; }

        ServiceAP()
        {
            GlobalBlackboard = GlobalBlackboard.Instance;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            Instance.DialogManager = new Dialog.DialogManager();
            Instance.PanelManager = new PanelManager();
        }
    }
}

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
        public GlobalBlackboard GlobalBlackboard;
        public Dialog.DialogManager DialogManager;
        public PanelManager PanelManager;

        ServiceAP()
        {
            GlobalBlackboard = GlobalBlackboard.Instance;
            DialogManager = new Dialog.DialogManager();
            PanelManager = new PanelManager();
        }
    }
}

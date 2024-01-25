using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameToolKit
{
    /// <summary>
    /// 管理器接入点
    /// </summary>
    /// <remarks>
    /// 用于提供全局的服务访问点
    /// </remarks>
    public class ServiceAP : SingletonBase<ServiceAP>
    {
        /// <summary>
        /// 全局变量库
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

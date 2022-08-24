using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame
{
    /// <summary>
    /// Ĭ�ϳ�ʼ����
    /// </summary>
    public class GeneralInitializer : ServiceInitializer
    {
        public override void Initialize()
        {
            var instance = ServiceFactory.Instance;
            instance.Register<EventManager>();
            instance.Register<PanelManager>();
            instance.Register<Dialog.DialogManager>();
        }
    }
}

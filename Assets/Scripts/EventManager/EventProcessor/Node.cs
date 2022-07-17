using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GameFrame.EventProcessor
{
    [NodeColor("#ffffff")]
    public abstract class Node : BaseNode
    {
        /// <summary>
        /// 节点绑定的处理器
        /// </summary>
        [OdinSerialize, HideInInspector]
        public EventProcessor Processor { get; private set; }
        public void Attach() 
        { 
            LastDataUpdataTime = Time.time;
            OnDetach();
        }
        protected virtual void OnAttach() { }
        public void Detach() 
        {
            OnDetach();
            LastDataUpdataTime = 0;
        }
        protected virtual void OnDetach() { }
    }
}

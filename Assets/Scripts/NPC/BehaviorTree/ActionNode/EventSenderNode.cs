using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;

namespace GameFrame.Behavior.Tree
{
    public class EventSenderNode : ActionNode
    {
        public string EventName;

        [OdinSerialize]
        public EventCenter.EventArgs EventArg;

        protected override NodeStatus OnUpdate()
        {
            GameManager.Instance.EventCenter.SendEvent(EventName, EventArg);
            return NodeStatus.Success;
        }
    }
}

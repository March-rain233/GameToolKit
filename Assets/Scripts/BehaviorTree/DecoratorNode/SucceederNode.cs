using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// ³É¹¦Æ÷
    /// </summary>
    public class SucceederNode : DecoratorNode
    {
        protected override NodeStatus OnUpdate()
        {
            Child.Tick();
            return NodeStatus.Success;
        }
    }
}
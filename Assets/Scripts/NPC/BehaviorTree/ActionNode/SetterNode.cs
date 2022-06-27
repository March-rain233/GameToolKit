using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// 设值节点
    /// </summary>
    public class SetterNode : ActionNode
    {
        [OdinSerialize]
        public Dictionary<string, EventCenter.EventArgs> Variables;

        protected override NodeStatus OnUpdate()
        {
            foreach(var p in Variables)
            {
                //runner.Variables[p.Key] = p.Value;
            }
            return NodeStatus.Success;
        }
    }
}

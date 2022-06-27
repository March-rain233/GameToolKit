using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{

    public class CheckTalkNode : ConditionNode
    {
        public string Name;
        protected override NodeStatus OnUpdate()
        {
            if (GameManager.Instance.GameSave.Story.HaveTriggered.Contains(Name))
            {
                return NodeStatus.Success;
            }
            return NodeStatus.Failure;
        }
    }
}
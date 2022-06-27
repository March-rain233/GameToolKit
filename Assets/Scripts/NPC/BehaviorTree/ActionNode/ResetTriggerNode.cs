using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{

    public class ResetTriggerNode : ActionNode
    {
        /// <summary>
        /// Ҫ�趨��trigger������
        /// </summary>
        [SerializeField]
        private string[] _triggerNames;

        protected override NodeStatus OnUpdate()
        {
            //var anim = runner.Variables["Animator"].Object as Animator;
            //System.Array.ForEach(_triggerNames, trigger => anim.ResetTrigger(trigger));
            return NodeStatus.Success;
        }
    }
}

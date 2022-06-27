using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{

    public class ResetTriggerNode : ActionNode
    {
        /// <summary>
        /// 要设定的trigger的名字
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

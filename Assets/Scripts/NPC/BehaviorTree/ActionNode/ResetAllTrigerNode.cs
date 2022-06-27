using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{

    public class ResetAllTrigerNode : ActionNode
    {

        /// <summary>
        /// ������еļ����е�trigger����
        /// </summary>
        public void ResetAllTriggers(Animator animator)
        {
            AnimatorControllerParameter[] aps = animator.parameters;
            for (int i = 0; i < aps.Length; i++)
            {
                AnimatorControllerParameter paramItem = aps[i];
                if (paramItem.type == AnimatorControllerParameterType.Trigger)
                {
                    string triggerName = paramItem.name;
                    bool isActive = animator.GetBool(triggerName);
                    if (isActive)
                    {
                        animator.ResetTrigger(triggerName);
                    }
                }
            }
        }

        protected override NodeStatus OnUpdate()
        {
            //ResetAllTriggers(runner.Variables["Animator"].Object as Animator);
            return NodeStatus.Success;
        }
    }
}
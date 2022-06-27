using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{

    public class ResetAllTrigerNode : ActionNode
    {

        /// <summary>
        /// 清除所有的激活中的trigger缓存
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
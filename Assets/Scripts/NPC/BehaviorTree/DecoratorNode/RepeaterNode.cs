using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    public class RepeaterNode : DecoratorNode
    {
        /// <summary>
        /// 运行次数
        /// </summary>
        public int Times;

        /// <summary>
        /// 累计次数
        /// </summary>
        private int _add;

        /// <summary>
        /// 是否无限运行
        /// </summary>
        public bool IsForever;

        protected override void OnEnter()
        {
            base.OnEnter();
            _add = 0;
        }

        protected override NodeStatus OnUpdate()
        {
            Child.Tick();
            ++_add;
            if (IsForever || _add < Times)
            {
                return NodeStatus.Running;
            }
            return NodeStatus.Success;
        }
    }
}
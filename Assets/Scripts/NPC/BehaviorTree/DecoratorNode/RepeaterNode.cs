using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    public class RepeaterNode : DecoratorNode
    {
        /// <summary>
        /// ���д���
        /// </summary>
        public int Times;

        /// <summary>
        /// �ۼƴ���
        /// </summary>
        private int _add;

        /// <summary>
        /// �Ƿ���������
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
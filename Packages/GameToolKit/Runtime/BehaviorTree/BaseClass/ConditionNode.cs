using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Behavior.Tree
{
    [NodeCategory("Condition")]
    public abstract class ConditionNode : Leaf
    {
        /// <summary>
        /// �Ƿ�Խ��ȡ��
        /// </summary>
        public bool Invert = false;

        protected sealed override NodeStatus OnUpdate() =>
            Invert ^ OnCheck() ? NodeStatus.Success : NodeStatus.Failure;
        protected abstract bool OnCheck();
    }
}

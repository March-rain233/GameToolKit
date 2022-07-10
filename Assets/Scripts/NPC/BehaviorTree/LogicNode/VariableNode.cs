using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameFrame.Behavior.Tree
{
    [NodeCategory("NULL")]
    public class VariableNode<T> : InputNode<T>
    {
        public string Index;
        protected override void OnUpdate()
        {
            Value = BehaviorTree.Blackboard.GetValue<T>(Index);
        }
    }
}

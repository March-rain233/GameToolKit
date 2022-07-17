using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree {
    [NodeCategory("NULL")]
    public class GlobalVariableNode<T> : VariableNode<T>
    {
        protected override IBlackboard _blackboard => GlobalDatabase.Instance;
        protected override IEnumerable<string> GetValidIndex()
        {
            List<string> validIndex = new List<string>();
            var type = typeof(T);
            foreach (var blackboardItem in GlobalDatabase.Instance.GetVariables())
            {
                if (blackboardItem.Value.TypeOfValue == type)
                {
                    validIndex.Add(blackboardItem.Key);
                }
            }
            return validIndex;
        }
    }
}

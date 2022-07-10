using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrame.Behavior.Tree
{
    [Node("#55efc4", NodeAttribute.PortType.Single, NodeAttribute.PortType.None)]
    public abstract class Leaf : Node
    {
#if UNITY_EDITOR
        public override void AddChild(Node node)
        {
            Debug.LogWarning("不允许向叶子节点添加子节点");
        }
        public override void RemoveChild(Node node)
        {
            Debug.LogWarning("不允许向叶子节点移除子节点");
        }
#endif
    }
}

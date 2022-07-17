using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrame.Behavior.Tree
{
    [Node("#55efc4", NodeAttribute.PortType.Single, NodeAttribute.PortType.None)]
    public abstract class Leaf : ProcessNode
    {
        public override void AddChild(ProcessNode node)
        {
            Debug.LogError("不允许向叶子节点添加子节点");
        }
        public override void RemoveChild(ProcessNode node)
        {
            Debug.LogError("不允许向叶子节点移除子节点");
        }
        public override ProcessNode[] GetChildren()
        {
            return new ProcessNode[] {};
        }
    }
}

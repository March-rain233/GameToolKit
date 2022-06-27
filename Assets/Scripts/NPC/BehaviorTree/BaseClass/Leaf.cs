using GameFrame.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrame.Behavior.Tree
{
    public abstract class Leaf : Node
    {
#if UNITY_EDITOR
        public override sealed INode.PortType Input => INode.PortType.Single;
        public override sealed INode.PortType Output => INode.PortType.None;
        public override void AddChild(Node node)
        {
            Debug.Log("不允许向叶子节点添加子节点");
        }
        public override void RemoveChild(Node node)
        {
            Debug.Log("不允许向叶子节点移除子节点");
        }
#endif

        public override INode[] GetChildren()
        {
            //返回空数组
            return new INode[] { };
        }
        public override Node Clone()
        {
            return Instantiate(this);
        }
    }
}

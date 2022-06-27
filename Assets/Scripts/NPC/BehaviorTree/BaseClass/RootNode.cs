using GameFrame.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// 根节点
    /// </summary>
    public class RootNode: Node
    {
        /// <summary>
        /// 子节点
        /// </summary>
        public Node Child;

#if UNITY_EDITOR
        public override sealed INode.PortType Input => INode.PortType.None;
        public override sealed INode.PortType Output => INode.PortType.Single;

        public override void AddChild(Node node)
        {
            if (Child)
            {
                Debug.Log("尝试向根节点非法添加复数个节点");
                return;
            }
            Child = node;
        }
        public override void RemoveChild(Node node)
        {
            if (Child != node)
            {
                Debug.Log($"尝试移除{Name}的{node.Name}子节点，但是{name}已无{node.Name}子节点");
                return;
            }
            Child = null;
        }
#endif
        public override Node Clone()
        {
            var node = Instantiate(this);
#if UNITY_EDITOR
            node.Guid = UnityEditor.GUID.Generate().ToString();
#endif
            node.Child = Child?.Clone();
            return node;
        }

        public override INode[] GetChildren()
        {
            if (Child) return new INode[] { Child };
            else return new INode[] { };
        }

        protected override NodeStatus OnUpdate()
        {
            return Child.Tick();
        }
    }
}

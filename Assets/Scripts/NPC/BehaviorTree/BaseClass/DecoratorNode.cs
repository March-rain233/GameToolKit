using GameFrame.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���νڵ�
/// </summary>
namespace GameFrame.Behavior.Tree
{
    public abstract class DecoratorNode : Node
    {
        /// <summary>
        /// �ӽڵ�
        /// </summary>
        public Node Child;
#if UNITY_EDITOR
        public override sealed INode.PortType Input => INode.PortType.Single;
        public override sealed INode.PortType Output => INode.PortType.Single;
        public override void AddChild(Node node)
        {
            if (Child)
            {
                Debug.Log("������װ�νڵ�Ƿ���Ӹ������ڵ�");
                return;
            }
            Child = node;
        }
        public override void RemoveChild(Node node)
        {
            if (Child != node)
            {
                Debug.Log($"�����Ƴ�{Name}��{node.Name}�ӽڵ㣬����{name}����{node.Name}�ӽڵ�");
                return;
            }
            Child = null;
        }
#endif

        protected override void OnAbort()
        {
            if (Child.Status == NodeStatus.Running) { Child.Abort(); }
        }

        public override INode[] GetChildren()
        {
            if (Child) return new INode[] { Child };
            else return new INode[] { };
        }

        public override Node Clone()
        {
            var node = Instantiate(this);
#if UNITY_EDITOR
            node.Guid = UnityEditor.GUID.Generate().ToString();
#endif
            node.Child = Child?.Clone();
            return node;
        }
    }
}

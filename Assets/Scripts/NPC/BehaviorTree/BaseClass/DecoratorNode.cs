using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// ���νڵ�
    /// </summary>
    [Node("#00b894", NodeAttribute.PortType.Single, NodeAttribute.PortType.Single)]
    [NodeCategory("Decorator")]
    public abstract class DecoratorNode : Node
    {
#if UNITY_EDITOR
        public override void AddChild(Node node)
        {
            if (Child != null)
            {
                Debug.LogWarning("������װ�νڵ�Ƿ���Ӹ������ڵ�");
                return;
            }
            Child = node;
        }
        public override void RemoveChild(Node node)
        {
            if (Child != node)
            {
                Debug.LogWarning($"�����Ƴ�{Name}��{node.Name}�ӽڵ㣬����{Name}����{node.Name}�ӽڵ�");
                return;
            }
            Child = null;
        }
#endif
        /// <summary>
        /// �ӽڵ�
        /// </summary>
        public Node Child;

        protected override void OnAbort()
        {
            if (Child.Status == NodeStatus.Running) { Child.Abort(); }
        }

        public override Node[] GetChildren()
        {
            if (Child != null) return new Node[] { Child };
            else return new Node[] { };
        }

        public override BaseNode Clone(BehaviorTree tree)
        {
            var node = base.Clone(tree) as DecoratorNode;
            var child = tree.Nodes.Find(node => node.Guid == Child.Guid);
            if(child == null)
            {
                child = Child.Clone(tree);
            }
            node.Child = child as Node;
            return node;
        }
    }
}

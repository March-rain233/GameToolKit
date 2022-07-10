using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// ���ڵ�
    /// </summary>
    [Node("#e84393", NodeAttribute.PortType.None, NodeAttribute.PortType.Single)]
    [NodeCategory("NULL")]
    [SerializeField]
    public class RootNode : Node
    {
#if UNITY_EDITOR
        public override void AddChild(Node node)
        {
            if (Child != null)
            {
                Debug.LogWarning("��������ڵ�Ƿ���Ӹ������ڵ�");
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
        public override BaseNode Clone(BehaviorTree tree)
        {
            var node = base.Clone(tree) as RootNode;
            var child = tree.Nodes.Find(node => node.Guid == Child.Guid);
            if (child == null)
            {
                child = Child.Clone(tree);
            }
            node.Child = child as Node;
            return node;
        }

        public override Node[] GetChildren()
        {
            if (Child != null) return new Node[] { Child };
            else return new Node[] { };
        }

        protected override NodeStatus OnUpdate()
        {
            return Child.Tick();
        }
    }
}

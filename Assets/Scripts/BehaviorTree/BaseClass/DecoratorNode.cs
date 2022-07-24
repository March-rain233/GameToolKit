using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// ���νڵ�
    /// </summary>
    [Node(NodeAttribute.PortType.Single, NodeAttribute.PortType.Single)]
    [NodeCategory("Decorator")]
    [NodeColor("#00b894")]
    public abstract class DecoratorNode : ProcessNode
    {
        /// <summary>
        /// �ӽڵ�
        /// </summary>
        [HideInTreeInspector]
        public ProcessNode Child;

        protected override void OnAbort()
        {
            if (Child.Status == NodeStatus.Running) 
            {
                Child.Abort();
            }
        }

        public override ProcessNode[] GetChildren()
        {
            if (Child != null)
            {
                return new ProcessNode[] { Child };
            }
            else return new ProcessNode[] { };
        }

        public override void AddChild(ProcessNode node)
        {
            if (Child != null)
            {
                Debug.LogError("������װ�νڵ�Ƿ���Ӹ������ڵ�");
                return;
            }
            Child = node;
        }

        public override void RemoveChild(ProcessNode node)
        {
            if (Child != node)
            {
                Debug.LogError($"�����Ƴ�{Name}��{node.Name}�ӽڵ㣬����{Name}����{node.Name}�ӽڵ�");
                return;
            }
            Child = null;
        }

        public override Node Clone()
        {
            var n = base.Clone() as DecoratorNode;
            n.Child = null;
            return n;
        }
    }
}

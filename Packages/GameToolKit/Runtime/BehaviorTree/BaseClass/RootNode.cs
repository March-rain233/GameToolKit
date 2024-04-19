using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameToolKit.Behavior.Tree
{
    /// <summary>
    /// ���ڵ�
    /// </summary>
    [Node(NodeAttribute.PortType.None, NodeAttribute.PortType.Single)]
    [NodeCategory("NULL")]
    [NodeColor("#e84393")]
    [SerializeField]
    public class RootNode : ProcessNode
    {
        /// <summary>
        /// �ӽڵ�
        /// </summary>
        [HideInGraphInspector, HideDuplicateReferenceBox, HideReferenceObjectPicker]
        public ProcessNode Child;

        public override ProcessNode[] GetChildren()
        {
            if (Child != null) return new ProcessNode[] { Child };
            else return new ProcessNode[] { };
        }

        protected override NodeStatus OnUpdate()
        {
            return Child.Tick();
        }

        public override BaseNode Clone()
        {
            var n = base.Clone() as RootNode;
            n.Child = null;
            return n;
        }

        public override void AddChild(ProcessNode node)
        {
            if (Child != null)
            {
                Debug.LogError("��������ڵ�Ƿ���Ӹ������ڵ�");
                return;
            }
            Child = node;
        }
        public override void RemoveChild(ProcessNode node)
        {
            if (Child != node)
            {
                Debug.LogError($"�����Ƴ�{this}��{node}�ӽڵ㣬����{this}����{node}�ӽڵ�");
                return;
            }
            Child = null;
        }

        public override void OrderChildren(Func<ProcessNode, ProcessNode, bool> func)
        {
            
        }
    }
}

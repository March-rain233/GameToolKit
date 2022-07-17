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
            Debug.LogError("��������Ҷ�ӽڵ�����ӽڵ�");
        }
        public override void RemoveChild(ProcessNode node)
        {
            Debug.LogError("��������Ҷ�ӽڵ��Ƴ��ӽڵ�");
        }
        public override ProcessNode[] GetChildren()
        {
            return new ProcessNode[] {};
        }
    }
}

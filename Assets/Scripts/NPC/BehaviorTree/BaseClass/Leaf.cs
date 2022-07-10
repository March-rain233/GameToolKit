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
            Debug.LogWarning("��������Ҷ�ӽڵ�����ӽڵ�");
        }
        public override void RemoveChild(Node node)
        {
            Debug.LogWarning("��������Ҷ�ӽڵ��Ƴ��ӽڵ�");
        }
#endif
    }
}

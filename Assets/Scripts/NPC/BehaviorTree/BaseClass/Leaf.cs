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
            Debug.Log("��������Ҷ�ӽڵ�����ӽڵ�");
        }
        public override void RemoveChild(Node node)
        {
            Debug.Log("��������Ҷ�ӽڵ��Ƴ��ӽڵ�");
        }
#endif

        public override INode[] GetChildren()
        {
            //���ؿ�����
            return new INode[] { };
        }
        public override Node Clone()
        {
            return Instantiate(this);
        }
    }
}

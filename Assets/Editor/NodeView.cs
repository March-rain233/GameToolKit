using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using GameFrame.Interface;

namespace GameFrame.Editor
{
    public class NodeView : Node
    {
        /// <summary>
        /// ���ýڵ㱻ѡ��
        /// </summary>
        public Action<NodeView> OnNodeSelected;

        /// <summary>
        /// ���ڵ㱻ѡ��ɾ��
        /// </summary>
        public Action<NodeView> OnDeleted;

        /// <summary>
        /// �ڵ�ʵ��
        /// </summary>
        public INode Node;

        /// <summary>
        /// ����˿�
        /// </summary>
        public Port Input;

        /// <summary>
        /// ����˿�
        /// </summary>
        public List<Port> Output = new List<Port>();

        public NodeView(INode node)
        {
            Node = node;
            title = node.Name;
            viewDataKey = node.Guid;

            style.left = node.ViewPosition.x;
            style.top = node.ViewPosition.y;

            //��������˿�
            switch (node.Input)
            {
                case INode.PortType.Single:
                    Input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, Node.GetType());
                    Input.name = "";
                    inputContainer.Add(Input);
                    break;
                case INode.PortType.Multi:
                    Input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, Node.GetType());
                    Input.name = "";
                    inputContainer.Add(Input);
                    break;
                case INode.PortType.None:
                default:
                    break;
            }

            //��ʼ������˿�
            switch (node.Output)
            {
                case INode.PortType.Single:
                    AddOutputPort();
                    break;
                case INode.PortType.Multi:
                    Button btn = new Button(AddOutputPort);
                    btn.text = "�������˿�";
                    titleButtonContainer.Add(btn);
                    var n = node.GetChildren().Length;
                    for (int i = 0; i < n; ++i)
                    {
                        AddOutputPort();
                    }
                    break;
                case INode.PortType.None:
                default:
                    break;
            }

            titleContainer.Q<Label>().text = "123";

            node.OnColorChanged += color => style.backgroundColor = color;
        }

        /// <summary>
        /// ���Ľڵ������
        /// </summary>
        private void ChangeName(string name)
        {
            Node.Name = name;

        }

        /// <summary>
        /// �������˿�
        /// </summary>
        private void AddOutputPort()
        {
            Output.Add(InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, null));
            Output[Output.Count - 1].portName = outputContainer.childCount.ToString();
            outputContainer.Add(Output[Output.Count - 1]);
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Node.ViewPosition = new Vector2(newPos.xMin, newPos.yMin);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            evt.menu.AppendAction("ɾ��", (a) => OnDeleted?.Invoke(this));
        }

    }
}


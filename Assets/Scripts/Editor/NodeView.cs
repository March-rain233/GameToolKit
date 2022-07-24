using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFrame.Editor
{
    /// <summary>
    /// �ڵ���ͼ
    /// </summary>
    public class NodeView : Node
    {
        /// <summary>
        /// �ڵ�ʵ��
        /// </summary>
        public BaseNode Node;
        /// <summary>
        /// �ڵ����ؼ�
        /// </summary>
        private Label _title;
        /// <summary>
        /// �ڵ��������
        /// </summary>
        private TextField _input;
        public NodeView(BaseNode node)
        {
            Node = node;

            //�󶨿ؼ��ֶ�
            _title = titleContainer.Q<Label>("title-label");
            _input = new TextField();
            titleContainer.Add(_input);
            _input.PlaceBehind(_title);
            _input.style.display = DisplayStyle.None;
            _input.RegisterCallback<FocusOutEvent>(e =>
            {
                _input.style.display = DisplayStyle.None;
                _title.style.display = DisplayStyle.Flex;
                ChangeName(_input.text);
            });

            //��������
            if (node.Name == null)
                node.Name = node.GetType().Name;
            title = node.Name;
            name = node.Name;
            _input.value = node.Name;

            //��guid
            viewDataKey = node.Guid;

            //������ͼ����
            style.left = node.ViewPosition.x;
            style.top = node.ViewPosition.y;

            //������Դ�˿�
            var list = node.GetValidPortDataList();
            foreach (var portData in list)
            {
                var tooltip = portData.PreferredType.Name;
                foreach (var t in portData.PortTypes)
                {
                    if (t != portData.PreferredType)
                    {
                        tooltip += $" {t.Name}";
                    }
                }
                if (portData.PortDirection == Direction.Input)
                {
                    var port = base.InstantiatePort(Orientation.Horizontal,
                        UnityEditor.Experimental.GraphView.Direction.Input,
                        Port.Capacity.Single,
                        portData.PreferredType);
                    port.portName = portData.NickName;
                    port.name = portData.Name;
                    port.tooltip = tooltip;
                    port.userData = portData.PortTypes;
                    inputContainer.Add(port);
                }
                else
                {
                    var port = base.InstantiatePort(Orientation.Horizontal,
                        UnityEditor.Experimental.GraphView.Direction.Output,
                        Port.Capacity.Multi,
                        portData.PreferredType);
                    port.portName = portData.NickName;
                    port.name = portData.Name;
                    port.tooltip = tooltip;
                    port.userData = portData.PortTypes;
                    outputContainer.Add(port);
                }
            }

            //�������
            var colorAttr = node.GetType().GetCustomAttributes(typeof(NodeColorAttribute), true);
            Color color;
            if(colorAttr == null || colorAttr.Length == 0)
            {
                color = Color.cyan;
            }
            else
            {
                color = (colorAttr[0] as NodeColorAttribute).Color;
            }
            titleContainer.style.backgroundColor = color;
            ColorUtility.TryParseHtmlString("#000000", out var temp);
            titleContainer.Q<Label>().style.color = temp;
        }

        /// <summary>
        /// ���Ľڵ������
        /// </summary>
        public void ChangeName(string name)
        {
            Node.Name = name;
            this.name = name;
            this.title = name;
            _input.value = name;
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Node.ViewPosition = new Vector2(newPos.xMin, newPos.yMin);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Rename", evt =>
            {
                _input.style.display = DisplayStyle.Flex;
                _title.style.display = DisplayStyle.None;
            });
            evt.menu.AppendSeparator();
            base.BuildContextualMenu(evt);
        }
    }
}
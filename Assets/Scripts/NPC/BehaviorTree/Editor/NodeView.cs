using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace GameFrame.Behavior.Tree.Editor
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        /// <summary>
        /// 当该节点被选中
        /// </summary>
        public Action<NodeView> OnNodeSelected;
        /// <summary>
        /// 当该节点被取消选中
        /// </summary>
        public Action<NodeView> OnNodeUnselected;

        /// <summary>
        /// 当节点被选择删除
        /// </summary>
        public Action<NodeView> OnDeleted;

        /// <summary>
        /// 节点实例
        /// </summary>
        public BaseNode Node;

        private Label _title;
        private TextField _input;
        public NodeView(BaseNode node)
        {
            Node = node;

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

            if(node.Name == null)
                node.Name = node.GetType().Name;
            title = node.Name;
            name = node.Name;
            _input.value = node.Name;

            viewDataKey = node.Guid;

            style.left = node.ViewPosition.x;
            style.top = node.ViewPosition.y;

            node.OnColorChanged += color => style.backgroundColor = color;

            //创建流程端口
            var nodeAttr = node.GetType().GetCustomAttributes(typeof(NodeAttribute), true)[0] as NodeAttribute;
            if(nodeAttr.InputPort == NodeAttribute.PortType.Single)
            {
                var port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(Node));
                port.portName = "Pre";
                port.name = "Pre";
                inputContainer.Add(port);
            }
            if(nodeAttr.OutputPort == NodeAttribute.PortType.Single)
            {
                var port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Node));
                port.portName = "Next";
                port.name = "Next";
                outputContainer.Add(port);
            }
            else if(nodeAttr.OutputPort == NodeAttribute.PortType.Multi)
            {
                var port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(Node));
                port.portName = "Next";
                port.name = "Next";
                outputContainer.Add(port);
            }
            //创建资源端口
            var type = node.GetType();
            while (type != typeof(BaseNode))
            {
                var portField = type.GetFields(
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
                    ).Where(field => field.IsDefined(typeof(PortAttribute), true) && field.DeclaringType == type);
                foreach (var field in portField)
                {
                    PortAttribute attr = field.GetCustomAttributes(typeof(PortAttribute), true)[0] as PortAttribute;
                    Type valueType = field.FieldType;
                    if (attr.PortType == PortType.Input)
                    {
                        var port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, valueType);
                        port.portName = attr.Name + $"({valueType.Name})";
                        port.name = field.Name;
                        inputContainer.Add(port);
                    }
                    else
                    {
                        var port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, valueType);
                        port.portName = attr.Name + $"({valueType.Name})";
                        port.name = field.Name;
                        outputContainer.Add(port);
                    }
                }
                type = type.BaseType;
            }
            //设置外观
            Color temp;
            titleContainer.style.backgroundColor = nodeAttr.Color;
            ColorUtility.TryParseHtmlString("#000000", out temp);
            titleContainer.Q<Label>().style.color = temp;
        }

        /// <summary>
        /// 更改节点的名字
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

        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            OnNodeUnselected?.Invoke(this);
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


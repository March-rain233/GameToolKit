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
        /// 节点实例
        /// </summary>
        public Node Node;

        private Label _title;
        private TextField _input;
        public NodeView(Node node)
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

            var border = this;
            border.style.paddingBottom =
                border.style.paddingLeft =
                border.style.paddingRight =
                border.style.paddingTop = 5;
            border.style.borderRightWidth =
                border.style.borderLeftWidth =
                border.style.borderTopWidth =
                border.style.borderBottomWidth = 2;
            border.style.borderBottomLeftRadius =
                border.style.borderBottomRightRadius =
                border.style.borderTopLeftRadius =
                border.style.borderTopRightRadius = 6;
            border.style.borderBottomColor =
                border.style.borderTopColor =
                border.style.borderRightColor =
                border.style.borderLeftColor = new Color(0, 0, 0, 0);
            //当节点状态改变时更改边框颜色
            node.OnColorChanged += color =>
                            {
                                border.style.borderBottomColor =
                                border.style.borderTopColor =
                                border.style.borderRightColor =
                                border.style.borderLeftColor =
                                color;
                            };

            //创建流程端口
            var nodeAttr = node.GetType().GetCustomAttributes(typeof(NodeAttribute), true)[0] as NodeAttribute;
            if(nodeAttr.InputPort == NodeAttribute.PortType.Single)
            {
                var port = base.InstantiatePort(Orientation.Horizontal, 
                    UnityEditor.Experimental.GraphView.Direction.Input, 
                    Port.Capacity.Single, 
                    typeof(ProcessNode));
                port.portName = "Pre";
                port.name = "Pre";
                port.userData = new HashSet<Type>() { port.portType };
                inputContainer.Add(port);
            }
            if(nodeAttr.OutputPort == NodeAttribute.PortType.Single)
            {
                var port = base.InstantiatePort(Orientation.Horizontal, 
                    UnityEditor.Experimental.GraphView.Direction.Output, 
                    Port.Capacity.Single, 
                    typeof(ProcessNode));
                port.portName = "Next";
                port.name = "Next";
                port.userData = new HashSet<Type>() { port.portType };
                outputContainer.Add(port);
            }
            else if(nodeAttr.OutputPort == NodeAttribute.PortType.Multi)
            {
                var port = base.InstantiatePort(Orientation.Horizontal, 
                    UnityEditor.Experimental.GraphView.Direction.Output, 
                    Port.Capacity.Multi, 
                    typeof(ProcessNode));
                port.portName = "Next";
                port.name = "Next";
                port.userData = new HashSet<Type>() { port.portType };
                outputContainer.Add(port);
            }
            //创建资源端口
            var type = node.GetType();
            while (type != typeof(Node))
            {
                var portField = type.GetFields(
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
                    ).Where(field => field.IsDefined(typeof(PortAttribute), true) && field.DeclaringType == type);
                foreach (var field in portField)
                {
                    PortAttribute attr = field.GetCustomAttributes(typeof(PortAttribute), true)[0] as PortAttribute;
                    Type valueType = field.FieldType;
                    var portname = attr.Name;
                    var tooltip = valueType.Name;
                    var typelist = new HashSet<Type>();
                    typelist.Add(valueType);
                    if(attr.ExtendPortTypes != null)
                    {
                        typelist.UnionWith(attr.ExtendPortTypes);
                        foreach (var t in attr.ExtendPortTypes)
                        {
                            tooltip += $" {t.Name}";
                        }
                    }
                    if (attr.Direction == Direction.Input)
                    {
                        var port = base.InstantiatePort(Orientation.Horizontal, 
                            UnityEditor.Experimental.GraphView.Direction.Input, 
                            Port.Capacity.Single, 
                            valueType);
                        port.portName = portname;
                        port.name = field.Name;
                        port.tooltip = tooltip;
                        port.userData = typelist;
                        inputContainer.Add(port);
                    }
                    else
                    {
                        var port = base.InstantiatePort(Orientation.Horizontal, 
                            UnityEditor.Experimental.GraphView.Direction.Output, 
                            Port.Capacity.Multi, 
                            valueType);
                        port.portName = portname;
                        port.name = field.Name;
                        port.tooltip = tooltip;
                        port.userData = typelist;
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


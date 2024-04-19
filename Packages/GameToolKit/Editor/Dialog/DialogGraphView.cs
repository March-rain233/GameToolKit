using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameToolKit.Editor;
using UnityEngine.UIElements;
using System;
using System.Linq;
using UnityEditor.Experimental.GraphView;

namespace GameToolKit.Dialog.Editor
{
    using BlackboardFieldUserData = Tuple<Domain, string>;
    public class DialogGraphView : DataFlowGraphView<DialogGraph, Node>
    {
        public new class UxmlFactory : UxmlFactory<DialogGraphView, UxmlTraits> { }

        public DialogGraphView() : base() 
        {
            RegisterCallback<DragUpdatedEvent>((e) =>
            {
                if (DragAndDrop.GetGenericData("DragSelection") != null && _blackboard.selection.Count > 0)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                }
            });
            RegisterCallback<DragPerformEvent>(e =>
            {
                Vector2 point = contentViewContainer.WorldToLocal(e.localMousePosition);
                foreach (BlackboardField field in _blackboard.selection)
                {
                    (var domain, var id) = (BlackboardFieldUserData)field.userData; ;
                    var type = typeof(VariableNode<>);
                    var parameterType = Graph.Blackboard[id, domain].TypeOfValue;
                    type = type.MakeGenericType(parameterType);
                    var view = CreateNode(type);
                    type.GetProperty("VariableId").SetValue(view.Node, id);
                    view.SetPosition(new Rect(point, Vector2.zero));
                    view.ChangeName(field.text);
                }
            });
        }

        protected override NodeView<Node> CreateNodeView(Node node)
        {
            switch (node)
            {
                case ProcessNode processNode:
                    {
                        var view = new ProcessNodeView(processNode);
                        AddElement(view);
                        return view;
                    }
                case LogicNode logicNode:
                    {
                        var view = new LogicNodeView(logicNode);
                        AddElement(view);
                        return view;
                    }
                default:
                    return base.CreateNodeView(node);
            }
        }

        public override void PopulateView(DialogGraph graph)
        {
            base.PopulateView(graph);
            //连接流程边
            foreach (var node in graph.Nodes.OfType<ProcessNode>())
            {
                var view = FindNodeView(node);
                //连接行为后继
                foreach(var child in node.Children)
                {
                    var childView = FindNodeView(child);
                    Edge edge = view.outputContainer.Q<ProcessPort>("Next").ConnectTo(childView.inputContainer.Q<ProcessPort>("Prev"));
                    AddElement(edge);
                }
            }
        }

        protected override void OnNodeToRemove(NodeView<Node> nodeView)
        {
            if (nodeView.Node is not EntryNode && nodeView.Node is not ExitNode)
            {
                Graph.RemoveNode(nodeView.Node);
            }
            else
            {
                CreateNodeView(nodeView.Node);
            }
        }

        protected override void OnEdgeToCreate(Edge edge)
        {
            switch (edge)
            {
                case ProcessEdgeView e:
                    var parentView = edge.output.node as NodeView<Node>;
                    var childView = edge.input.node as NodeView<Node>;
                    (parentView.Node as ProcessNode).Children.Add(childView.Node as ProcessNode);
                    (childView.Node as ProcessNode).Parents.Add(parentView.Node as ProcessNode);
                    return;
                default:
                    base.OnEdgeToCreate(edge);
                    return;
            }
        }

        protected override void OnEdgeToRemove(Edge edge)
        {
            switch (edge)
            {
                case ProcessEdgeView e:
                    var parentView = edge.output.node as NodeView<Node>;
                    var childView = edge.input.node as NodeView<Node>;
                    (parentView.Node as ProcessNode).Children.Remove(childView.Node as ProcessNode);
                    (childView.Node as ProcessNode).Parents.Remove(parentView.Node as ProcessNode);
                    return;
                default:
                    base.OnEdgeToRemove(edge);
                    return;
            }
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            switch (startPort)
            {
                case ProcessPort:
                    var list = ports.OfType<ProcessPort>()
                        .Where(endPort =>
                            endPort.direction != startPort.direction
                            && endPort.node != startPort.node)
                        .ToList<Port>();
                    return list;
                default:
                    return base.GetCompatiblePorts(startPort, nodeAdapter);
            }
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (Graph != null)
            {
                base.BuildContextualMenu(evt);
                if (evt.target is ProcessNodeView 
                    && ((evt.target as ProcessNodeView).Node is EntryNode
                    || (evt.target as ProcessNodeView).Node is ExitNode))
                {
                    var list = evt.menu.MenuItems();
                    for (int i = 0; i < list.Count; i++)
                    {
                        var action = list[i] as DropdownMenuAction;
                        if (action != null && action.name == "Delete")
                        {
                            evt.menu.RemoveItemAt(i);
                            evt.menu.InsertAction(i, "Delete", (e) => { }, DropdownMenuAction.Status.Disabled);
                            break;
                        }
                    }
                }
            }
        }

        protected override NodeSearchProvider<DialogGraph, Node> CreateNodeSearchProvider()
        {
            var provider = ScriptableObject.CreateInstance<NodeSearchProvider>();
            provider.Init(this, Window);
            return provider;
        }
    }
}

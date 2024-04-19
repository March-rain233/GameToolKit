using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using GameToolKit.Behavior.Tree;
using GameToolKit.Editor;
using GameToolKit.Utility;
using Sirenix.OdinInspector.Editor.Internal.UIToolkitIntegration;
using Sirenix.OdinInspector.Editor;


namespace GameToolKit.Behavior.Tree.Editor
{
    using BlackboardFieldUserData = Tuple<Domain, string>;

    /// <summary>
    /// 行为树图
    /// </summary>
    public class BehaviorTreeView : DataFlowGraphView<BehaviorTree, Node>
    {
        public new class UxmlFactory : UxmlFactory<BehaviorTreeView, UxmlTraits> { }

        class TreeAdapter : GraphAdapter
        {
            public TreeAdapter(DataFlowGraphView<BehaviorTree, Node> view) : base(view)
            {
            }

            protected override List<NodeView<Node>> GetNodeList(DataFlowGraphView<BehaviorTree, Node> view)
            {
                var list = base.GetNodeList(view);
                var root = list.Find(n => n.Node is RootNode);
                list.Remove(root);
                list.Insert(0, root);
                return list;
            }

            protected override int GetEdgeLevel(Edge edge) => edge switch
            {
                ProcessEdgeView => 2,
                _ => base.GetEdgeLevel(edge)
            };

            public override IEnumerable<int> GetDescendant(int index)
            {
                var list = new List<int>();
                if(_views[index].Node is ProcessNode)
                {
                    list.AddRange((_views[index].Node as ProcessNode).GetChildren()
                        .Select(n => _views.FindIndex(v => v.Node == n)));
                }
                for (int i = 0; i < Nodes.Length; i++)
                    if (EdgeMatrix[index, i] == 1)
                        list.Add(i);
                return list;
            }
        }

        public BehaviorTreeView() : base()
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

        public override void PopulateView(BehaviorTree graph)
        {
            base.PopulateView(graph);
            //连接流程边
            foreach (var node in graph.Nodes.OfType<ProcessNode>())
            {
                var view = FindNodeView(node);
                //连接行为后继
                var children = node.GetChildren();
                if (children == null || children.Length == 0)
                {
                    continue;
                }
                Array.ForEach(children, child =>
                {
                    var childView = FindNodeView(child);
                    Edge edge = view.outputContainer.Q<ProcessPort>("Next").ConnectTo(childView.inputContainer.Q<ProcessPort>("Prev"));
                    AddElement(edge);
                });
            }
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

        protected override GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            base.OnGraphViewChanged(graphViewChange);
            if (graphViewChange.movedElements != null)
            {
                foreach (var element in graphViewChange.movedElements)
                {
                    switch (element)
                    {
                        case ProcessNodeView node:
                            var parent = node.inputContainer.Q<ProcessPort>("Prev")?.connections;
                            if (parent != null && parent.Count() > 0)
                            {
                                ((parent.First().output.node as NodeView<Node>).Node as ProcessNode).OrderChildren(CompareProcessNodePriority);
                            }
                            break;
                    }
                }
            }

            return graphViewChange;
        }

        private bool CompareProcessNodePriority(ProcessNode a, ProcessNode b) =>
             a.ViewPosition.y < b.ViewPosition.y;

        protected override void OnNodeToRemove(NodeView<Node> nodeView)
        {
            if (nodeView.Node is not RootNode)
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
                    (parentView.Node as ProcessNode).AddChild(childView.Node as ProcessNode);
                    (parentView.Node as ProcessNode).OrderChildren(CompareProcessNodePriority);
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
                    (parentView.Node as ProcessNode).RemoveChild(childView.Node as ProcessNode);
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
                if (evt.target is ProcessNodeView && (evt.target as ProcessNodeView).Node is RootNode)
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

        protected override NodeSearchProvider<BehaviorTree, Node> CreateNodeSearchProvider()
        {
            var provider = ScriptableObject.CreateInstance<NodeSearchProvider>();
            provider.Init(this, Window);
            return provider;
        }

        protected override void SortGraph()
        {
            var adapter = new TreeAdapter(this);
            var roots = Enumerable.Range(0, nodes.Count())
                .Where(i => adapter.GetPrecursor(i).Count() == 0)
                .Union(Enumerable.Range(0, nodes.Count()));
            GraphLayoutUtility.TreeLayout(adapter, roots.ToList());
        }
    }
}
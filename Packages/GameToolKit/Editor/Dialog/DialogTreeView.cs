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
    public class DialogTreeView : DataFlowGraphView<DialogTree, Node>
    {
        public new class UxmlFactory : UxmlFactory<DialogTreeView, UxmlTraits> { }
        protected override NodeSearchProviderBase GetSearchWindowProvider()
        {
            var provider = ScriptableObject.CreateInstance<NodeSearchProvider>();
            provider.Init(this, Window);
            return provider;
        }

        protected override NodeView CreateNodeView(BaseNode node)
        {
            NodeView nodeView = new DialogNodeView(node);
            AddElement(nodeView);
            return nodeView;
        }
        protected override GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            //当元素被移除
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    RemoveFromSelection(elem);
                    //移除点
                    NodeView nodeView = elem as NodeView;
                    if (nodeView != null)
                    {
                        if (nodeView.Node is not EntryNode && nodeView.Node is not ExitNode)
                        {
                            Graph.RemoveNode(nodeView.Node as Node);
                        }
                        else
                        {
                            CreateNodeView(nodeView.Node);
                        }
                    }
                    //移除边
                    Edge edge = elem as Edge;
                    if (edge != null)
                    {
                        NodeView parentView = edge.output.node as NodeView;
                        NodeView childView = edge.input.node as NodeView;
                        if (edge.input.name == "Prev" && edge.output.name == "Next")
                        {
                            (parentView.Node as ProcessNode).Children.Remove(childView.Node as ProcessNode);
                            (childView.Node as ProcessNode).Parents.Remove(parentView.Node as ProcessNode);
                        }
                        else
                        {
                            switch (((SyncType)edge.userData))
                            {
                                case SyncType.Pull:
                                    childView.Node.RemoveInputEdge(parentView.Node, edge.output.name, edge.input.name);
                                    break;
                                case SyncType.Push:
                                    parentView.Node.RemoveOutputEdge(childView.Node, edge.output.name, edge.input.name);
                                    break;
                            }
                        }
                    }
                });
            }
            //当边被创建
            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    edge.userData = SyncType.Pull;
                    if (edge.input.name == "Prev" && edge.output.name == "Next")
                    {
                        (parentView.Node as ProcessNode).Children.Add(childView.Node as ProcessNode);
                        (childView.Node as ProcessNode).Parents.Add(parentView.Node as ProcessNode);
                    }
                    else
                    {
                        childView.Node.AddInputEdge(parentView.Node, edge.output.name, edge.input.name);
                    }
                });
            }
            return graphViewChange;
        }

        public override void PopulateView(DataFlowGraph<DialogTree, Node> graph)
        {
            base.PopulateView(graph);
            //为传入的树的边生成视图
            foreach (var node in graph.Nodes)
            {
                var view = FindNodeView(node);
                //连接行为后继
                if (node is ProcessNode)
                {
                    var children = ((ProcessNode)node).Children;
                    if (children == null || children.Count == 0)
                    {
                        continue;
                    }
                    foreach (var child in children)
                    {
                        NodeView childView = FindNodeView(child);
                        Edge edge = view.outputContainer.Q<Port>("Next").ConnectTo(childView.inputContainer.Q<Port>("Prev"));
                        AddElement(edge);
                    }
                }
            }
        }

        protected override void OnAddFieldToInspector(ISelectable selectable)
        {
            switch (selectable)
            {
                case Edge edge:
                    if (edge.input.name != "Prev" || edge.output.name != "Next")
                    {
                        base.OnAddFieldToInspector(selectable);
                    }
                    return;
                default:
                    base.OnAddFieldToInspector(selectable);
                    return;
            }
        }

        protected override void OnRemoveField(ISelectable selectable)
        {
            switch (selectable)
            {
                case Edge edge:
                    if (edge.input.name != "Prev" || edge.output.name != "Next")
                    {
                        base.OnRemoveField(selectable);
                    }
                    return;
                default:
                    base.OnRemoveField(selectable);
                    return;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;
using System.Linq;
using GameFrame.Editor;

namespace GameFrame.EventProcessor.Editor
{
    public class ProcessorView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<ProcessorView, UxmlTraits> { }
        EventProcessor _processor;
        GraphInspector _inspector;
        public override bool supportsWindowedBlackboard => false;
        protected override bool canCopySelection => false;
        protected override bool canDeleteSelection => true;
        protected override bool canPaste => false;
        protected override bool canCutSelection => false;
        public override bool canGrabFocus => true;
        protected override bool canDuplicateSelection => false;

        public ProcessorView()
        {
            var background = new GridBackground();
            Insert(0, background);

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            _inspector = new GraphInspector();
            Add(_inspector);
            _inspector.visible = false;
        }

        public void ShowInspector(bool visible)
        {
            if (_processor == null) return;
            _inspector.visible = visible;
        }
        NodeView FindNodeView(Node node)
        {
            return GetNodeByGuid(node.Guid) as NodeView;
        }
        /// <summary>
        /// 更新树，当选择的tree改变时，重新加载视图
        /// </summary>
        /// <param name="processor"></param>
        public void PopulateView(EventProcessor processor)
        {
            _processor = processor;

            //删除上一颗树的视图
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;

            //为传入的树的原有节点生成视图
            foreach (var node in _processor.Nodes)
            {
                CreateNodeView(node);
            }

            //为传入的树的边生成视图
            foreach (var node in _processor.Nodes)
            {
                var view = FindNodeView(node);
                //生成资源边
                foreach (var edge in node.InputEdges)
                {
                    var sourcePort = FindNodeView(edge.SourceNode as Node).outputContainer.Q<Port>(edge.SourceField);
                    var e = view.inputContainer.Q<Port>(edge.TargetField).ConnectTo(sourcePort);
                    e.userData = SyncType.Pull;
                    AddElement(e);
                }
                foreach (var edge in node.OutputEdges)
                {
                    var targetPort = FindNodeView(edge.TargetNode as Node).inputContainer.Q<Port>(edge.TargetField);
                    var e = view.outputContainer.Q<Port>(edge.SourceField).ConnectTo(targetPort);
                    e.userData = (SyncType)e.userData | SyncType.Push;
                    AddElement(e);
                }
            }

            //修改显示参数
            _inspector.title = processor.name;
        }

        /// <summary>
        /// 清除组件
        /// </summary>
        public void ClearView()
        {
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;
            ShowInspector(false);
            _processor = null;

        }
        /// <summary>
        /// 根据传入节点创建节点视图
        /// </summary>
        /// <param name="node"></param>
        private NodeView CreateNodeView(Node node)
        {
            NodeView nodeView = new NodeView(node);
            AddElement(nodeView);
            return nodeView;
        }
        /// <summary>
        /// 创建节点
        /// </summary>
        /// <param name="type"></param>
        public NodeView CreateNode(Type type)
        {
            Node node = _processor.CreateNode(type);
            return CreateNodeView(node);
        }
        /// <summary>
        /// 当图发生变化时
        /// </summary>
        /// <param name="graphViewChange"></param>
        /// <returns></returns>
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
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
                        _processor.RemoveNode(nodeView.Node);
                    }
                    //移除边
                    Edge edge = elem as Edge;
                    if (edge != null)
                    {
                        NodeView parentView = edge.output.node as NodeView;
                        NodeView childView = edge.input.node as NodeView;
                        if (((SyncType)edge.userData & SyncType.Pull) != 0)
                            childView.Node.InputEdges.RemoveAll(
                                e => e.SourceNode == parentView.Node
                                && e.TargetNode == childView.Node
                                && e.SourceField == edge.output.name
                                && e.TargetField == edge.input.name
                                );
                        if (((SyncType)edge.userData & SyncType.Push) != 0)
                            parentView.Node.OutputEdges.RemoveAll(
                                e => e.SourceNode == parentView.Node
                                && e.TargetNode == childView.Node
                                && e.SourceField == edge.output.name
                                && e.TargetField == edge.input.name
                                );
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
                    childView.Node.InputEdges.Add(new SourceInfo(parentView.Node, childView.Node, edge.output.name, edge.input.name));
                });
            }
            return graphViewChange;
        }
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var typeList = startPort.userData as HashSet<Type>;
            var list = ports.ToList().Where(endPort =>
                endPort.direction != startPort.direction
                && endPort.node != startPort.node
                && (endPort.userData as HashSet<Type>).Overlaps(typeList))
                .ToList();
            return list;
        }
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (_processor != null)
            {
                base.BuildContextualMenu(evt);
                if (evt.target is NodeView && (evt.target as NodeView).Node is EventSenderNode)
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
        public override void AddToSelection(ISelectable selectable)
        {
            base.AddToSelection(selectable);
            var node = selectable as NodeView;
            if (node != null)
            {
                _inspector.Add(new NodeField(node.Node));
            }
            var edge = selectable as Edge;
            if (edge != null && edge.userData != null)
            {
                _inspector.Add(new EdgeField(
                    new SourceInfo((edge.output.node as NodeView).Node,
                    (edge.input.node as NodeView).Node,
                    edge.output.portName,
                    edge.input.portName),
                    (SyncType)edge.userData,
                    SetEdge));
            }
        }
        public override void ClearSelection()
        {
            base.ClearSelection();
            _inspector.Clear();
        }
        public override void RemoveFromSelection(ISelectable selectable)
        {
            base.RemoveFromSelection(selectable);
            var node = selectable as NodeView;
            if (node != null)
            {
                foreach (var child in _inspector.Children())
                {
                    var n = child as NodeField;
                    if (n != null && n.Node == node.Node)
                    {
                        _inspector.Remove(child);
                        break;
                    }
                }
            }
            var edge = selectable as Edge;
            if (edge != null && edge.userData != null)
            {
                var info = new SourceInfo((edge.output.node as NodeView).Node,
                    (edge.input.node as NodeView).Node,
                    edge.output.portName,
                    edge.input.portName);
                foreach (var child in _inspector.Children())
                {
                    var n = child as EdgeField;
                    if (n != null && n.Source == info)
                    {
                        _inspector.Remove(child);
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// 设置指定边的数据传输方式
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="type"></param>
        private void SetEdge(SourceInfo edge, SyncType type)
        {
            var e = edges.ToList().Find(e => new SourceInfo((e.output.node as NodeView).Node,
                    (e.input.node as NodeView).Node,
                    e.output.portName,
                    e.input.portName) == edge);
            var actual = new SourceInfo((e.output.node as NodeView).Node,
                    (e.input.node as NodeView).Node,
                    e.output.name,
                    e.input.name);
            if ((type & SyncType.Pull) != 0)
            {
                if (edge.TargetNode.InputEdges.Find(e => e == actual) == default)
                {
                    edge.TargetNode.InputEdges.Add(actual);
                }
            }
            else
            {
                var temp = edge.TargetNode.InputEdges.Find(e => e == actual);
                if (temp != default)
                {
                    edge.TargetNode.InputEdges.Remove(temp);
                }
            }
            if ((type & SyncType.Push) != 0)
            {
                if (edge.SourceNode.OutputEdges.Find(e => e == actual) == default)
                {
                    edge.SourceNode.OutputEdges.Add(actual);
                }
            }
            else
            {
                var temp = edge.SourceNode.OutputEdges.Find(e => e == actual);
                if (temp != default)
                {
                    edge.SourceNode.OutputEdges.Remove(temp);
                }
            }
            if (type == 0)
            {
                RemoveFromSelection(e);
                e.output.Disconnect(e);
                e.input.Disconnect(e);
                RemoveElement(e);
            }
            else
            {
                e.userData = type;
            }
        }
    }
}


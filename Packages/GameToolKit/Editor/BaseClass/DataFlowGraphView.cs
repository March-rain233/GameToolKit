using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameToolKit.Editor
{
    public abstract class DataFlowGraphView<TGraph, TNode> : GraphView 
        where TNode : BaseNode
        where TGraph : DataFlowGraph<TGraph, TNode>
    {
        #region ͼ���������
        public override bool supportsWindowedBlackboard => false;
        protected override bool canCopySelection => false;
        protected override bool canDeleteSelection => true;
        protected override bool canPaste => false;
        protected override bool canCutSelection => false;
        public override bool canGrabFocus => true;
        protected override bool canDuplicateSelection => false;
        #endregion

        #region ��ͼ���
        /// <summary>
        /// ������
        /// </summary>
        protected GraphInspector _inspector;
        #endregion

        /// <summary>
        /// ��ǰ������ͼ��
        /// </summary>
        public DataFlowGraph<TGraph, TNode> Graph { get; protected set; }

        /// <summary>
        /// ���ŵĴ���
        /// </summary>
        public EditorWindow Window;

        #region ͼ���ʼ��
        public DataFlowGraphView()
        {
            var background = new GridBackground();
            Insert(0, background);

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.march_rain.gametoolkit/Editor/BaseClass/USS/CustomGraph.uss");
            styleSheets.Add(styleSheet);

            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), GetSearchWindowProvider());

            _inspector = CreateInspector();
            _inspector.visible = false;
            Add(_inspector);
        }

        protected abstract NodeSearchProviderBase GetSearchWindowProvider();

        protected virtual GraphInspector CreateInspector()
        {
            return new GraphInspector();
        }
        #endregion

        #region ͼ�����
        /// <summary>
        /// ����������ѡ���tree�ı�ʱ�����¼�����ͼ
        /// </summary>
        /// <param name="graph"></param>
        public virtual void PopulateView(DataFlowGraph<TGraph, TNode> graph)
        {
            //ɾ����һ��������ͼ
            ClearView();
            
            Graph = graph;
            //Ϊ���������ԭ�нڵ�������ͼ
            foreach (var node in graph.Nodes)
                CreateNodeView(node);


            //Ϊ��������ı�������ͼ
            foreach (var node in graph.Nodes)
            {
                var view = FindNodeView(node);

                //������Դ������
                EdgeRectify(node);

                //������Դ��
                foreach (var edge in node.InputEdges)
                {
                    var sourcePort = FindNodeView(edge.SourceNode).outputContainer.Q<Port>(edge.SourceField);
                    var targetPort = view.inputContainer.Q<Port>(edge.TargetField);
                    //�鿴���Ƿ��Ѵ���
                    Edge e = sourcePort.connections.FirstOrDefault(e => e.input == targetPort);
                    if (e == null)
                    {
                        e = targetPort.ConnectTo(sourcePort);
                        e.userData = SyncType.Pull;
                    }
                    else
                    {
                        e.userData = SyncType.Pull | SyncType.Push;
                    }
                    AddElement(e);
                }
                foreach (var edge in node.OutputEdges)
                {
                    var targetPort = FindNodeView(edge.TargetNode).inputContainer.Q<Port>(edge.TargetField);
                    var sourcePort = view.outputContainer.Q<Port>(edge.SourceField);
                    //�鿴���Ƿ��Ѵ���
                    Edge e = targetPort.connections.FirstOrDefault(e => e.output == sourcePort);
                    if (e == null)
                    {
                        e = sourcePort.ConnectTo(targetPort);
                        e.userData = SyncType.Push;
                    }
                    else
                    {
                        e.userData = SyncType.Pull | SyncType.Push;
                    }
                    AddElement(e);
                }
            }

            //�޸���ʾ����
            _inspector.title = graph.name;
        }

        /// <summary>
        /// ��Դ�ߺϷ��Խ���
        /// </summary>
        /// <param name="edges"></param>
        protected void EdgeRectify(TNode node)
        {
            var inputEdgeToCorrect = from edge in node.InputEdges 
                                     let source = FindNodeView(edge.SourceNode)
                                     let target = FindNodeView(edge.TargetNode)
                                     let isSource = source.outputContainer.Q<Port>(edge.SourceField) == null
                                     let isTarget = target.inputContainer.Q<Port>(edge.TargetField) == null
                                     where  isSource || isTarget
                                     select (edge, isSource, isTarget);
            foreach(var (edge, isSource, isTarget) in inputEdgeToCorrect)
            {
                string sourceField, targetField;
                if (isSource) sourceField = edge.SourceNode.FixPortIndex(edge.SourceField);
                else sourceField = edge.SourceField;
                if (isTarget) targetField = edge.TargetNode.FixPortIndex(edge.TargetField);
                else targetField = edge.TargetField;

                node.RemoveInputEdge(edge.TargetNode, edge.SourceField, edge.TargetField);
                if (!string.IsNullOrEmpty(sourceField) && !string.IsNullOrEmpty(targetField))
                    node.AddInputEdge(edge.TargetNode, sourceField, targetField);            
            }

            var outputEdgeToCorrect = from edge in node.OutputEdges
                                     let source = FindNodeView(edge.SourceNode)
                                     let target = FindNodeView(edge.TargetNode)
                                     let isSource = source.outputContainer.Q<Port>(edge.SourceField) == null
                                     let isTarget = target.inputContainer.Q<Port>(edge.TargetField) == null
                                     where isSource || isTarget
                                     select (edge, isSource, isTarget);
            foreach (var (edge, isSource, isTarget) in inputEdgeToCorrect)
            {
                string sourceField, targetField;
                if (isSource) sourceField = edge.SourceNode.FixPortIndex(edge.SourceField);
                else sourceField = edge.SourceField;
                if (isTarget) targetField = edge.TargetNode.FixPortIndex(edge.TargetField);
                else targetField = edge.TargetField;

                node.RemoveOutputEdge(edge.TargetNode, edge.SourceField, edge.TargetField);
                if (!string.IsNullOrEmpty(sourceField) && !string.IsNullOrEmpty(targetField))
                    node.AddOutputEdge(edge.TargetNode, sourceField, targetField);
            }
        }


        /// <summary>
        /// ��ͼ�����仯ʱ
        /// </summary>
        /// <param name="graphViewChange"></param>
        /// <returns></returns>
        protected virtual GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            //��Ԫ�ر��Ƴ�
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    RemoveFromSelection(elem);
                    //�Ƴ���
                    NodeView nodeView = elem as NodeView;
                    if (nodeView != null)
                    {
                        Graph.RemoveNode(nodeView.Node as TNode);
                    }
                    //�Ƴ���
                    Edge edge = elem as Edge;
                    if (edge != null)
                    {
                        NodeView parentView = edge.output.node as NodeView;
                        NodeView childView = edge.input.node as NodeView;
                        switch (((SyncType)edge.userData)){
                            case SyncType.Pull:
                                childView.Node.RemoveInputEdge(parentView.Node, edge.output.name, edge.input.name);
                                break;
                            case SyncType.Push:
                                parentView.Node.RemoveOutputEdge(childView.Node, edge.output.name, edge.input.name);
                                break;
                        }
                    }
                });
            }
            //���߱�����
            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    edge.userData = SyncType.Pull;
                    childView.Node.AddInputEdge(parentView.Node, edge.output.name, edge.input.name);
                });
            }
            return graphViewChange;
        }

        /// <summary>
        /// ������
        /// </summary>
        public void ClearView()
        {
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;

            _inspector.ClearTabAll();
            ShowInspector(false);

            Graph = null;
        }

        /// <summary>
        /// ���ݴ���ڵ㴴���ڵ���ͼ
        /// </summary>
        /// <param name="node"></param>
        protected virtual NodeView CreateNodeView(BaseNode node)
        {
            NodeView nodeView = new NodeView(node);
            AddElement(nodeView);
            return nodeView;
        }
        #endregion

        #region ͼ�����
        /// <summary>
        /// �����ڵ�
        /// </summary>
        /// <param name="type"></param>
        public NodeView CreateNode(Type type)
        {
            TNode node = Graph.CreateNode(type);
            return CreateNodeView(node);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var typeList = startPort.userData as HashSet<Type>;
            bool isAcceptAll = startPort.direction == Direction.Input && typeList.Contains(typeof(object));
            var list = ports.ToList().Where(endPort =>
                endPort.direction != startPort.direction
                && endPort.node != startPort.node
                && (isAcceptAll || (endPort.userData as HashSet<Type>).Overlaps(typeList)))
                .ToList();
            return list;
        }

        /// <summary>
        /// ����ָ���ߵ����ݴ��䷽ʽ
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="type"></param>
        //protected void SetEdge(SourceEdge edge, SyncType type)
        //{
        //    //���Ҷ�Ӧ�ı���ͼ
        //    var edgeView = edges.ToList().Find(e => new SourceEdge((e.output.node as NodeView).Node,
        //            (e.input.node as NodeView).Node,
        //            e.output.portName,
        //            e.input.portName, false) == edge);
        //    BaseNode addNode = null;
        //    BaseNode removeNode = null;
        //    switch (type)
        //    {
        //        case SyncType.Pull:
        //            addNode = edge.TargetNode;
        //            removeNode = edge.SourceNode;
        //            break;
        //        case SyncType.Push:
        //            addNode = edge.SourceNode;
        //            removeNode = edge.TargetNode;
        //            break;
        //    }
        //    addNode.InputEdges.Add(new SourceEdge(edge.SourceNode, edge.TargetNode, edge.TargetField, edge.SourceField, true));
        //    removeNode.OutputEdges.RemoveAll(e=>e==edge);
        //    edgeView.userData = type;
        //}

        public void SaveChange()
        {
            EditorUtility.SetDirty(Graph);
            AssetDatabase.SaveAssets();
        }
        #endregion

        #region ����
        /// <summary>
        /// ��ʾ������
        /// </summary>
        /// <param name="visible"></param>
        public void ShowInspector(bool visible)
        {
            _inspector.visible = visible && Graph != null;
        }

        /// <summary>
        /// ���Ҷ�Ӧ�ڵ����ͼ
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public NodeView FindNodeView(BaseNode node)
        {
            return GetNodeByGuid(node.Id.ToString()) as NodeView;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (Graph != null)
            {
                base.BuildContextualMenu(evt);
            }
        }

        public override void AddToSelection(ISelectable selectable)
        {
            base.AddToSelection(selectable);
            OnAddFieldToInspector(selectable);
        }

        public override void ClearSelection()
        {
            base.ClearSelection();
            ClearCurrentInspector();
        }

        public override void RemoveFromSelection(ISelectable selectable)
        {
            base.RemoveFromSelection(selectable);
            OnRemoveField(selectable);
        }

        /// <summary>
        /// �����ǰ�������ϵ��ֶ�
        /// </summary>
        protected virtual void ClearCurrentInspector()
        {
            _inspector.ClearTab();
        }

        /// <summary>
        /// ������������ֶ�
        /// </summary>
        /// <param name="field"></param>
        protected virtual void OnAddFieldToInspector(ISelectable selectable)
        {
            switch (selectable)
            {
                case NodeView node:
                    _inspector.AddToTab(new NodeField(node.Node, node.name +
                        $"({node.Node.GetType().Name.Split('.').Last()})"));
                    return;
                //case Edge edge:
                //    _inspector.AddToTab(new EdgeField(
                //        new SourceEdge((edge.output.node as NodeView).Node,
                //        (edge.input.node as NodeView).Node,
                //        edge.output.portName,
                //        edge.input.portName, false),
                //        (SyncType)edge.userData,
                //        SetEdge));
                //    return;
                default:
                    return;
            }
        }

        /// <summary>
        /// �Ƴ���ֵ�������ֶ�
        /// </summary>
        /// <param name="value"></param>
        protected virtual void OnRemoveField(ISelectable selectable)
        {
            switch (selectable)
            {
                case NodeView node:
                    RemoveAssociatedFieldAll(node.Node);
                    return;
                //case Edge edge:
                //    RemoveAssociatedFieldAll(new SourceEdge((edge.output.node as NodeView).Node,
                //        (edge.input.node as NodeView).Node,
                //        edge.output.portName,
                //        edge.input.portName, false));
                //    return;
                default:
                    return;
            }
        }

        /// <summary>
        /// �Ƴ����Ӧֵ������������ֶ�
        /// </summary>
        /// <param name="value"></param>
        protected void RemoveAssociatedFieldAll(object value)
        {
            if (_inspector.TryGetAssociatedFieldAll(value, out var list))
            {
                foreach (var field in list)
                {
                    _inspector.RemoveFromTab(field.Value, field.Key);
                }
            }
        }
        #endregion
    }
}
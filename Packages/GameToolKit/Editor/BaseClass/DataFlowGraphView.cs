using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using GameToolKit.Utility;

namespace GameToolKit.Editor
{
    using BlackboardFieldUserData = Tuple<Domain, string>;
    public abstract class DataFlowGraphView<TGraph, TNode> : GraphView
        where TNode : BaseNode
        where TGraph : DataFlowGraph<TGraph, TNode>
    {
        protected class GraphAdapter : GraphLayoutAdapter
        {
            public override Rect[] Nodes => _nodes;

            protected Rect[] _nodes;

            public override int[,] EdgeMatrix => _edgeMatrix;

            protected int[,] _edgeMatrix;

            protected List<NodeView<TNode>> _views;

            public GraphAdapter(DataFlowGraphView<TGraph, TNode> view)
            {
                _views = GetNodeList(view);
                _nodes = new Rect[_views.Count()];
                _edgeMatrix = new int[_views.Count(), _views.Count()];
                for(int i = 0; i < _views.Count(); i++)
                {
                    var nodeview = view.nodes.AtIndex(i);
                    _nodes[i] = nodeview.layout;
                }
                foreach(var edge in view.edges)
                {
                    int startIndex = _views.IndexOf(edge.output.node as NodeView<TNode>);
                    int endIndex = _views.IndexOf(edge.input.node as NodeView<TNode>);
                    _edgeMatrix[startIndex, endIndex] = Mathf.Max(GetEdgeLevel(edge), _edgeMatrix[startIndex, endIndex]);
                }
            }

            protected virtual List<NodeView<TNode>> GetNodeList(DataFlowGraphView<TGraph, TNode> view)
            {
                var list = view.nodes.OfType<NodeView<TNode>>().ToList();
                return list;
            }

            protected virtual int GetEdgeLevel(Edge edge) => edge switch
            {
                SourceEdgeView =>1 ,
                _ => 0
            };


            public override void Finish()
            {
                for(int i = 0; i < _views.Count(); i++)
                {
                    _views[i].SetPosition(_nodes[i]);
                }
            }
        }

        #region 图表操作设置
        public override bool supportsWindowedBlackboard => true;
        protected override bool canCopySelection => false;
        protected override bool canDeleteSelection => true;
        protected override bool canPaste => false;
        protected override bool canCutSelection => false;
        public override bool canGrabFocus => true;
        protected override bool canDuplicateSelection => false;
        #endregion

        /// <summary>
        /// 监视器
        /// </summary>
        public GraphInspector Inspector;

        /// <summary>
        /// 当前操作的图表
        /// </summary>
        public TGraph Graph { get; protected set; }

        /// <summary>
        /// 附着的窗口
        /// </summary>
        public EditorWindow Window;


        protected Blackboard _blackboard;

        private bool _isBlackboardShowing = false;

        #region 图表初始化
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

            nodeCreationRequest = context => OpenSearchWindow(context.screenMousePosition);
        }

        public void OpenSearchWindow(Vector2 mousePos) =>
            SearchWindow.Open(new SearchWindowContext(mousePos), CreateNodeSearchProvider());

        protected abstract NodeSearchProvider<TGraph, TNode> CreateNodeSearchProvider();
        #endregion

        #region 图表更新
        /// <summary>
        /// 更新树，当选择的tree改变时，重新加载视图
        /// </summary>
        /// <param name="graph"></param>
        public virtual void PopulateView(TGraph graph)
        {
            //删除上一颗树的视图
            ClearView();

            Graph = graph;
            //为传入的树的原有节点生成视图
            foreach (var node in graph.Nodes)
                CreateNodeView(node);

            //为传入的树的边生成视图
            foreach (var node in graph.Nodes)
            {
                var view = FindNodeView(node);

                //修正资源边数据
                EdgeRectify(node);

                //生成资源边
                foreach (var edge in node.InputEdges)
                {
                    var sourcePort = FindNodeView(edge.SourceNode).outputContainer.Q<SourcePort>(edge.SourceField);
                    var targetPort = view.inputContainer.Q<SourcePort>(edge.TargetField);
                    //查看边是否已创建
                    var e = targetPort.ConnectTo(sourcePort);
                    e.SyncType = SyncType.Pull;
                    AddElement(e);
                }
                foreach (var edge in node.OutputEdges)
                {
                    var targetPort = FindNodeView(edge.TargetNode).inputContainer.Q<SourcePort>(edge.TargetField);
                    var sourcePort = view.outputContainer.Q<SourcePort>(edge.SourceField);
                    //查看边是否已创建
                    var e = sourcePort.ConnectTo(targetPort);
                    e.SyncType = SyncType.Push;
                    AddElement(e);
                }
            }

            _blackboard = GetBlackboard();
            Add(_blackboard);
            ShowBlackboard(_isBlackboardShowing);
        }

        /// <summary>
        /// 资源边合法性矫正
        /// </summary>
        /// <param name="edges"></param>
        protected void EdgeRectify(TNode node)
        {
            var inputEdgeToCorrect = from edge in node.InputEdges
                                     let source = FindNodeView(edge.SourceNode)
                                     let target = FindNodeView(edge.TargetNode)
                                     let isSource = source.outputContainer.Q<SourcePort>(edge.SourceField) == null
                                     let isTarget = target.inputContainer.Q<SourcePort>(edge.TargetField) == null
                                     where isSource || isTarget
                                     select (edge, isSource, isTarget);
            foreach (var (edge, isSource, isTarget) in inputEdgeToCorrect)
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
                                      let isSource = source.outputContainer.Q<SourcePort>(edge.SourceField) == null
                                      let isTarget = target.inputContainer.Q<SourcePort>(edge.TargetField) == null
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
        /// 当图发生变化时
        /// </summary>
        /// <param name="graphViewChange"></param>
        /// <returns></returns>
        protected virtual GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            //当元素被移除
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    RemoveFromSelection(elem);
                    //移除点
                    var nodeView = elem as NodeView<TNode>;
                    if (nodeView != null) OnNodeToRemove(nodeView);
                    //移除边
                    Edge edge = elem as Edge;
                    if (edge != null) OnEdgeToRemove(edge);
                });
            }
            //当边被创建
            if (graphViewChange.edgesToCreate != null)
                graphViewChange.edgesToCreate.ForEach(edge => OnEdgeToCreate(edge));
            return graphViewChange;
        }

        /// <summary>
        /// 处理图的节点移除
        /// </summary>
        /// <param name="nodeView"></param>
        protected virtual void OnNodeToRemove(NodeView<TNode> nodeView) =>
            Graph.RemoveNode(nodeView.Node);

        /// <summary>
        /// 处理图的边移除
        /// </summary>
        /// <param name="edge"></param>
        protected virtual void OnEdgeToRemove(Edge edge)
        {
            switch (edge)
            {
                case SourceEdgeView e:
                    var parentView = edge.output.node as NodeView<TNode>;
                    var childView = edge.input.node as NodeView<TNode>;
                    switch (e.SyncType)
                    {
                        case SyncType.Pull:
                            childView.Node.RemoveInputEdge(parentView.Node, edge.output.name, edge.input.name);
                            break;
                        case SyncType.Push:
                            parentView.Node.RemoveOutputEdge(childView.Node, edge.output.name, edge.input.name);
                            break;
                    }
                    return;
                default:
                    Debug.LogWarning($"Unknow Edge Type:{edge.GetType().Name}");
                    return;
            }
        }

        /// <summary>
        /// 处理图的边增加
        /// </summary>
        /// <param name="edge"></param>
        protected virtual void OnEdgeToCreate(Edge edge)
        {
            switch (edge)
            {
                case SourceEdgeView e:
                    var parentView = edge.output.node as NodeView<TNode>;
                    var childView = edge.input.node as NodeView<TNode>;
                    e.SyncType = SyncType.Pull;
                    childView.Node.AddInputEdge(parentView.Node, edge.output.name, edge.input.name);
                    break;
                default:
                    Debug.LogWarning($"Unknow Edge Type:{edge.GetType().Name}");
                    return;
            }
        }

        /// <summary>
        /// 清除组件
        /// </summary>
        public virtual void ClearView()
        {
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;

            Inspector.ClearTabAll();

            if (_blackboard != null)
                ReleaseBlackboard(_blackboard);

            Graph = null;
        }

        /// <summary>
        /// 根据传入节点创建节点视图
        /// </summary>
        /// <param name="node"></param>
        protected virtual NodeView<TNode> CreateNodeView(TNode node)
        {
            var nodeView = new NodeView<TNode>(node);
            AddElement(nodeView);
            return nodeView;
        }
        #endregion

        #region 图表操作
        /// <summary>
        /// 创建节点
        /// </summary>
        /// <param name="type"></param>
        public NodeView<TNode> CreateNode(Type type)
        {
            TNode node = Graph.CreateNode(type);
            return CreateNodeView(node);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            switch (startPort)
            {
                case SourcePort:
                    var typeList = startPort.userData as HashSet<Type>;
                    bool isAcceptAll = startPort.direction == Direction.Input && typeList.Contains(typeof(object));
                    var list = ports.OfType<SourcePort>()
                        .Where(endPort =>
                            endPort.direction != startPort.direction
                            && endPort.node != startPort.node
                            && (isAcceptAll || (endPort.userData as HashSet<Type>).Overlaps(typeList)))
                        .ToList<Port>();
                    return list;
                default: throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 设置指定边的数据传输方式
        /// </summary>
        /// <param name = "edge" ></ param >
        /// < param name="type"></param>
        protected void SetSourceEdge(SourceEdge edge, SyncType type)
        {
            //查找对应的边视图
            var edgeView = edges.OfType<SourceEdgeView>().First(e =>
                    (e.output.node as NodeView<TNode>).Node == edge.SourceNode &&
                    (e.input.node as NodeView<TNode>).Node == edge.TargetNode &&
                    e.output.name == edge.SourceField &&
                    e.input.name == edge.TargetField);
            edgeView.SyncType = type;
            switch (type)
            {
                case SyncType.Pull:
                    edge.SourceNode.RemoveOutputEdge(edge.TargetNode, edge.SourceField, edge.TargetField);
                    edge.TargetNode.AddInputEdge(edge.SourceNode, edge.SourceField, edge.TargetField);
                    break;
                case SyncType.Push:
                    edge.TargetNode.RemoveInputEdge(edge.SourceNode, edge.SourceField, edge.TargetField);
                    edge.SourceNode.AddOutputEdge(edge.TargetNode, edge.SourceField, edge.TargetField);
                    break;
            }
        }
        #endregion

        #region 黑板相关
        /// <summary>
        /// 显示黑板
        /// </summary>
        /// <param name="visible"></param>
        public void ShowBlackboard(bool visible)
        {
            _isBlackboardShowing = visible;
            if (_blackboard != null)
                _blackboard.style.display = _isBlackboardShowing ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public override Blackboard GetBlackboard()
        {
            TGraph graph = Graph;

            //设置黑板
            var blackboard = new Blackboard(this);
            blackboard.title = typeof(TGraph).Name;
            blackboard.subTitle = "Blackboard";
            blackboard.scrollable = true;
            blackboard.windowed = false;
            blackboard.visible = true;
            var global = new Foldout() { text = "Global", name = "Global" };
            blackboard.Add(global);
            Foldout prototype = null;
            if (graph.Blackboard.HasPrototypeDomain)
            {
                prototype = new Foldout() { text = "Prototype", name = "Prototype" };
                blackboard.Add(prototype);
            }
            var local = new Foldout() { text = "Local", name = "Local" };
            blackboard.Add(local);

            //创建变量视图
            Action<string, BlackboardVariable, Domain> addBlackVariable = (id, variable, domain) =>
            {
                var section = domain switch
                {
                    Domain.Global => global,
                    Domain.Prototype => prototype,
                    Domain.Local => local,
                    _ => throw new NotImplementedException(),
                };

                //生成解析字段
                var obj = ScriptableObject.CreateInstance<InspectorHelper>();
                obj.InspectorData = variable;
                var editor = UnityEditor.Editor.CreateEditor(obj, typeof(BlackVariableEditor));
                var field = new InspectorElement(editor);

                //生成blackboardfield
                var title = new BlackboardField();
                title.name = id;
                title.text = Graph.Blackboard.GUIDManager.ID2Name(id);
                title.typeText = variable.TypeOfValue.Name;

                var row = new BlackboardRow(title, field);
                row.name = id;
                BlackboardFieldUserData data = new BlackboardFieldUserData(domain, id);
                title.userData = data;
                title.RegisterCallback<ContextualMenuPopulateEvent>(e =>
                {
                    e.menu.AppendAction("Delete", a =>
                    {
                        Graph.Blackboard.RemoveVariable(id, domain);
                    }, domain == Domain.Global ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal);
                });

                section.Add(row);
            };

            //移除变量视图
            Action<string, BlackboardVariable, Domain> removeBlackVariable = (id, variable, domain) =>
            {
                var section = domain switch
                {
                    Domain.Global => global,
                    Domain.Prototype => prototype,
                    Domain.Local => local,
                    _ => throw new NotImplementedException(),
                };
                section.Remove(section.Q<BlackboardRow>(id));
            };

            //生成变量字段
            foreach (var (domain, id, variable) in Graph.Blackboard)
                addBlackVariable(id, variable, domain);

            //拖动更新函数
            if (graph.Blackboard.HasPrototypeDomain)
            {
                EventCallback<DragUpdatedEvent> dragUpdateHandler = (DragUpdatedEvent e) =>
                {
                    if (DragAndDrop.GetGenericData("DragSelection") != null && blackboard.selection.Count > 0 &&
                        blackboard.selection.All(e => !global.Contains(e as VisualElement)))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    }
                };
                local.RegisterCallback<DragUpdatedEvent>(dragUpdateHandler);
                prototype.RegisterCallback<DragUpdatedEvent>(dragUpdateHandler);
            }

            //放置函数
            if (graph.Blackboard.HasPrototypeDomain)
            {
                EventCallback<DragPerformEvent, Domain> dragPerformHandler = (DragPerformEvent e, Domain targetDomain) =>
                {
                    foreach (BlackboardField field in blackboard.selection.ToList())
                    {
                        var (domain, id) = (BlackboardFieldUserData)field.userData;
                        if (domain == targetDomain) continue;

                        //更改数据
                        Graph.Blackboard.ChangeVariableDomain(id, targetDomain);
                    }
                };
                local.RegisterCallback<DragPerformEvent, Domain>(dragPerformHandler, Domain.Local);
                prototype.RegisterCallback<DragPerformEvent, Domain>(dragPerformHandler, Domain.Prototype);
            }

            //绑定变量名更新函数
            blackboard.editTextRequested += (Blackboard board, VisualElement elem, string newName) =>
            {
                var field = elem as BlackboardField;
                Graph.Blackboard.GUIDManager.ChangeName(field.text, newName);
                field.text = newName;
            };

            //绑定添加变量函数
            blackboard.addItemRequested += board =>
            {
                var provider = ScriptableObject.CreateInstance<BlackboardSearchProvider>();
                provider.Init(graph.Blackboard);
                var position = Window.position.position + blackboard.GetPosition().position;
                SearchWindow.Open(new SearchWindowContext(position), provider);
            };

            blackboard.RegisterCallback<AttachToPanelEvent>(e =>
            {
                graph.Blackboard.VariableWithDomainAdded += addBlackVariable;
                graph.Blackboard.VariableWithDomainRemoved += removeBlackVariable;
            });
            blackboard.RegisterCallback<DetachFromPanelEvent>(e =>
            {
                graph.Blackboard.VariableWithDomainAdded -= addBlackVariable;
                graph.Blackboard.VariableWithDomainRemoved -= removeBlackVariable;
            });

            return blackboard;
        }

        public override void ReleaseBlackboard(Blackboard toRelease)
        {
            RemoveElement(toRelease);
            _blackboard = null;
        }
        #endregion

        #region 其他
        /// <summary>
        /// 图布局
        /// </summary>
        protected virtual void SortGraph()
        {
            var adapter = new GraphAdapter(this);
            GraphLayoutUtility.TreeLayout(adapter, Enumerable.Range(0, nodes.Count()).ToList());
        }

        public void Search(string text)
        {
            ClearSelection();
            var list = nodes.ToList();
            foreach (var node in list.Where(n => n.name.Contains(text, StringComparison.CurrentCultureIgnoreCase)))
                AddToSelection(node);
        }

        /// <summary>
        /// 查找对应节点的视图
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public NodeView<TNode> FindNodeView(BaseNode node)
        {
            return GetNodeByGuid(node.Id.ToString()) as NodeView<TNode>;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (Graph != null)
            {
                base.BuildContextualMenu(evt);
                if(evt.target == this)
                {
                    evt.menu.AppendAction("Sort Graph", e => SortGraph());
                }
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
        /// 清除当前监视器上的字段
        /// </summary>
        protected void ClearCurrentInspector()
        {
            Inspector.ClearTab();
        }

        /// <summary>
        /// 朝监视器添加字段
        /// </summary>
        /// <param name="field"></param>
        protected virtual void OnAddFieldToInspector(ISelectable selectable)
        {
            switch (selectable)
            {
                case NodeView<TNode> node:
                    Inspector.AddToTab(OnCreateNodeField(node));
                    return;
                case SourceEdgeView edge:
                    Inspector.AddToTab(OnCreateSourceEdgeField(edge));
                    return;
                default:
                    return;
            }
        }

        /// <summary>
        /// 当创建监视器的节点字段
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected NodeField OnCreateNodeField(NodeView<TNode> node) =>
            new NodeField(node.Node, node.name + $"({node.Node.GetType().Name.Split('.').Last()})");

        /// <summary>
        /// 当创建监视器的资源边字段
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected SourceEdgeField OnCreateSourceEdgeField(SourceEdgeView edge) =>
            new SourceEdgeField(new SourceEdge((edge.output.node as NodeView<TNode>).Node, (edge.input.node as NodeView<TNode>).Node, 
                edge.output.name, edge.input.name),
                edge.SyncType,
                SetSourceEdge);

        /// <summary>
        /// 移除与值关联的字段
        /// </summary>
        /// <param name="value"></param>
        protected virtual void OnRemoveField(ISelectable selectable)
        {
            switch (selectable)
            {
                case NodeView<TNode> node:
                    RemoveAssociatedFieldAll(node.Node);
                    return;
                case SourceEdgeView edge:
                    RemoveAssociatedFieldAll(new SourceEdge((edge.output.node as NodeView<TNode>).Node,
                        (edge.input.node as NodeView<TNode>).Node,
                        edge.output.name,
                        edge.input.name));
                    return;
                default:
                    return;
            }
        }

        /// <summary>
        /// 移除与对应值相关联的所有字段
        /// </summary>
        /// <param name="value"></param>
        protected void RemoveAssociatedFieldAll(object value)
        {
            if (Inspector.TryGetAssociatedFieldAll(value, out var list))
            {
                foreach (var field in list)
                {
                    Inspector.RemoveFromTab(field.Value, field.Key);
                }
            }
        }
        #endregion
    }

    internal class BlackVariableEditor : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            Tree.BeginDraw(true);
            var property = Tree.GetPropertyAtPath("InspectorData");
            var children = property.Children;
            foreach (var child in children)
            {
                child.Draw();
            }
            Tree.EndDraw();
        }
    }
}
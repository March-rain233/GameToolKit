using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using GameFrame.Behavior.Tree;
using GameFrame.Editor;

namespace GameFrame.Behavior.Tree.Editor
{
    /// <summary>
    /// 行为树图
    /// </summary>
    public class TreeView : GraphView
    {
        /// <summary>
        /// 当前显示的树
        /// </summary>
        private BehaviorTree _tree;
        private Blackboard _blackboard;
        private GraphInspector _inspector;
        public override bool supportsWindowedBlackboard => true;
        protected override bool canCopySelection => false;
        protected override bool canDeleteSelection => true;
        protected override bool canPaste => false;
        protected override bool canCutSelection => false;
        public override bool canGrabFocus => true;
        protected override bool canDuplicateSelection => false;
        public TreeView()
        {
            var background = new GridBackground();
            Insert(0, background);

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/BehaviorTree/Editor/USS/BehaviorTreeEditor.uss");
            styleSheets.Add(styleSheet);

            var searchProvider = ScriptableObject.CreateInstance<NodeSearchProvider>();
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchProvider);

            RegisterCallback<DragUpdatedEvent>((e) =>
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            });
            RegisterCallback<DragPerformEvent>(e =>
            {
                Vector2 point = contentViewContainer.WorldToLocal(e.localMousePosition);
                foreach (BlackboardField field in _blackboard.selection)
                {
                    (var variableType, var domain) = (ValueTuple<Type, Domain>)field.userData;
                    Type type;
                    if(domain == Domain.Global)
                    {
                        type = typeof(GlobalVariableNode<>);
                    }
                    else
                    {
                        type = typeof(VariableNode<>);
                    }
                    type = type.MakeGenericType(variableType.BaseType.GetGenericArguments()[0]);
                    var view = CreateNode(type);
                    view.SetPosition(new Rect(point, Vector2.zero));
                    type.GetProperty("Index").SetValue(view.Node, field.text);
                    view.ChangeName(field.text);
                }
            });
            _inspector = new GraphInspector();
            Add(_inspector);
            _inspector.visible = false;
        }
        /// <summary>
        /// 获取节点视图
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        NodeView FindNodeView(Node node)
        {
            return GetNodeByGuid(node.Guid) as NodeView;
        }
        /// <summary>
        /// 更新树，当选择的tree改变时，重新加载视图
        /// </summary>
        /// <param name="tree"></param>
        internal void PopulateView(BehaviorTree tree)
        {
            _tree = tree;

            //删除上一颗树的视图
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;

            //生成黑板
            if (_blackboard != null)
            {
                ReleaseBlackboard(_blackboard);
            }
            _blackboard = GetBlackboard();

            //为传入的树的原有节点生成视图
            foreach (var node in _tree.Nodes)
            {
                CreateNodeView(node);
            }

            //为传入的树的边生成视图
            foreach (var node in _tree.Nodes)
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
                //连接行为后继
                if (node is ProcessNode)
                {
                    var children = ((ProcessNode)node).GetChildren();
                    if (children == null || children.Length == 0)
                    {
                        continue;
                    }
                    Array.ForEach(children, child =>
                    {
                        NodeView childView = FindNodeView(child);
                        Edge edge = view.outputContainer.Q<Port>("Next").ConnectTo(childView.inputContainer.Q<Port>("Pre"));
                        AddElement(edge);
                    });
                }
            }

            //修改显示参数
            _inspector.title = tree.name;
        }

        /// <summary>
        /// 清除组件
        /// </summary>
        public void ClearView()
        {
            _tree = null;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;

            if (_blackboard != null)
            {
                ReleaseBlackboard(_blackboard);
            }

            ShowInspector(false);
        }

        /// <summary>
        /// 删除指定节点
        /// </summary>
        /// <param name="node"></param>
        private void DeleteNodeView(NodeView node)
        {
            var list = edges.ToList();
            //移除与节点连接的边
            for (int i = list.Count - 1; i >= 0; --i)
            {
                if (list[i].input.node == node || list[i].output.node == node)
                {
                    RemoveElement(list[i]);
                }
            }
            //移除点的视图
            RemoveElement(node);
            //移除树的点
            _tree.RemoveNode(node.Node);
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
            Node node = _tree.CreateNode(type);
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
                        _tree.RemoveNode(nodeView.Node);
                    }
                    //移除边
                    Edge edge = elem as Edge;
                    if (edge != null)
                    {
                        NodeView parentView = edge.output.node as NodeView;
                        NodeView childView = edge.input.node as NodeView;
                        if (edge.input.name == "Pre" && edge.output.name == "Next")
                        {
                            (parentView.Node as ProcessNode).RemoveChild(childView.Node as ProcessNode);
                        }
                        else
                        {
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
                    if (edge.input.name == "Pre" && edge.output.name == "Next")
                    {
                        (parentView.Node as ProcessNode).AddChild(childView.Node as ProcessNode);
                    }
                    else
                    {
                        edge.userData = SyncType.Pull;
                        childView.Node.InputEdges.Add(new SourceInfo(parentView.Node, childView.Node, edge.output.name, edge.input.name));
                    }
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
        public void ShowBlackboard(bool visible)
        {
            if (_tree == null) return;
            _blackboard.visible = visible;
        }
        public override Blackboard GetBlackboard()
        {
            Dictionary<Type, string> variableTypeList = new Dictionary<Type, string>();
            {
                var list = TypeCache.GetTypesDerivedFrom(typeof(GenericVariable<>));
                foreach (var type in list)
                {
                    variableTypeList.Add(type, type.BaseType.GenericTypeArguments[0].Name);
                }
            }
            var blackboard = new Blackboard(this);
            blackboard.title = "Behavior Tree";
            blackboard.subTitle = "Blackboard";
            blackboard.scrollable = true;
            var global = new Foldout() { text = "Global" };
            var prototype = new Foldout() { text = "Prototype" };
            var local = new Foldout() { text = "Local" };
            Func<string, Type, bool, Domain, BlackboardField> getVariable = (string name, Type type, bool newItem, Domain domain) =>
            {
                var field = new BlackboardField();
                field.text = name;
                field.typeText = variableTypeList[type];
                field.userData = (Type: type, Domain: domain);
                if (domain != Domain.Global)
                {
                    bool init = newItem;
                    string oldName = name;
                    field.RegisterCallback<FocusOutEvent>((e) =>
                    {
                        if (string.IsNullOrEmpty(field.text) || 
                        (_tree.Blackboard.HasValueInTree(field.text) && field.text != oldName))
                        {
                            EditorUtility.DisplayDialog("Warring", "Name should not be empty or a variable with the same name already exists", "OK");
                            if (init)
                            {
                                _blackboard.Remove(field);
                            }
                            else
                            {
                                field.text = oldName;
                            }
                            _blackboard.ReleaseMouse();
                        }
                        else
                        {
                            if (init)
                            {
                                _tree.Blackboard.AddLocalVariable(field.text, Activator.CreateInstance(type) as BlackboardVariable);
                                init = false;
                            }
                            else
                            {
                                _tree.Blackboard.RenameValue(oldName, field.text);
                            }
                            oldName = field.text;
                        }
                    });
                    field.RegisterCallback<ContextualMenuPopulateEvent>(e =>
                    {
                        e.menu.AppendAction("Delete", e =>
                        {
                            _tree.Blackboard.RemoveValue(oldName);
                            field.RemoveFromHierarchy();
                        });
                    });
                    if (newItem)
                    {
                        field.OpenTextEditor();
                    }
                }
                else
                {
                    field.RegisterCallback<ContextualMenuPopulateEvent>(e =>
                    {
                        var count = e.menu.MenuItems().Count;
                        for(int i = count - 1; i >= 0; i--)
                        {
                            e.menu.RemoveItemAt(i);
                        }
                    });
                }
                return field;
            };
            //创建全局变量
            foreach (var variable in GlobalDatabase.Instance.GetVariables())
            {
                global.Add(getVariable(variable.Key, variable.Value.GetType(), false, Domain.Global));
            }
            foreach (var variable in _tree.Blackboard.GetLocalVariables())
            {
                local.Add(getVariable(variable.Key, variable.Value.GetType(), false, Domain.Local));
            }
            foreach(var variable in _tree.Blackboard.GetPrototypeVariables())
            {
                prototype.Add(getVariable(variable.Key, variable.Value.GetType(), false, Domain.Prototype));
            }
            //变量更改定义域
            local.RegisterCallback<DragUpdatedEvent>((e) =>
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            });
            local.RegisterCallback<DragPerformEvent>(e =>
            {
                var selection = new List<ISelectable>(blackboard.selection);
                foreach (BlackboardField field in selection)
                {
                    (var type, var domain) = (ValueTuple<Type, Domain>)field.userData;
                    if(domain == Domain.Prototype)
                    {
                        var v = _tree.Blackboard.GetVariable(field.text);
                        _tree.Blackboard.RemoveValue(field.text);
                        _tree.Blackboard.AddLocalVariable(field.text, v);
                        field.userData = (type, Domain.Local);
                        prototype.Remove(field);
                        local.Add(field);
                    }
                }
                e.StopImmediatePropagation();
            });
            prototype.RegisterCallback<DragUpdatedEvent>((e) =>
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            });
            prototype.RegisterCallback<DragPerformEvent>(e =>
            {
                var selection = new List<ISelectable>(blackboard.selection);
                foreach (BlackboardField field in selection)
                {
                    (var type, var domain) = (ValueTuple<Type, Domain>)field.userData;
                    if (domain == Domain.Local)
                    {
                        var v = _tree.Blackboard.GetVariable(field.text);
                        _tree.Blackboard.RemoveValue(field.text);
                        _tree.Blackboard.AddPrototypeVariable(field.text, v);
                        field.userData = (type, Domain.Prototype);
                        local.Remove(field);
                        prototype.Add(field);
                    }
                }
                e.StopImmediatePropagation();
            });
            blackboard.addItemRequested = (blackboard) =>
            {
                var menu = new GenericMenu();
                foreach (var item in variableTypeList)
                {
                    string name = item.Value;
                    menu.AddItem(new GUIContent(name), false, () => local.Add(getVariable("", item.Key, true, Domain.Local)));
                }
                menu.ShowAsContext();
            };
            Add(blackboard);
            blackboard.Add(global);
            blackboard.Add(prototype);
            blackboard.Add(local);
            blackboard.visible = true;
            return blackboard;
        }
        public override void ReleaseBlackboard(Blackboard toRelease)
        {
            RemoveElement(toRelease);
        }
        public void ShowInspector(bool visible)
        {
            if (_tree == null) return;
            _inspector.visible = visible;
        }
        /// <summary>
        /// 整理全图
        /// </summary>
        public void SortGraph()
        {
            if (_tree == null) return;
            const float marginWidth = 20f;
            const float marginHeight = 20f;
            Dictionary<ProcessNode, Rect> treeRects = new Dictionary<ProcessNode, Rect>();
            //生成子树的矩形大小
            Utility.LambdaUtility.Fix<ProcessNode, Rect>(f => node =>
            {
                Rect rect = Rect.zero;
                var children = node.GetChildren();
                foreach (var child in node.GetChildren())
                {
                    var cr = f(child);
                    rect.width = Mathf.Max(cr.width, rect.width);
                    rect.height += cr.height;
                }
                var view = FindNodeView(node);
                rect.height += marginHeight * (children.Count() - 1);
                rect.width += children.Count() > 0 ? marginWidth : 0;
                rect.width += view.layout.width;
                rect.height = Mathf.Max(view.layout.height, rect.height);
                return treeRects[node] = rect;
            })(_tree.RootNode);
            //设置子树位置
            Utility.LambdaUtility.Fix<ProcessNode, bool>(f => node =>
            {
                var view = FindNodeView(node);
                view.SetPosition(treeRects[node]);
                Vector2 pos = treeRects[node].position + new Vector2(view.layout.width + marginWidth, -treeRects[node].height / 2);
                var children = node.GetChildren();
                foreach (var child in node.GetChildren())
                {
                    var temp = treeRects[child];
                    temp.position += pos + new Vector2(0, treeRects[child].height / 2);
                    treeRects[child] = temp;
                    pos.y += treeRects[child].height + marginHeight;
                    f(child);
                }
                return true;
            })(_tree.RootNode);
        }
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (_tree != null)
            {
                base.BuildContextualMenu(evt);
                if(evt.target is NodeView && (evt.target as NodeView).Node is RootNode)
                {
                    var list = evt.menu.MenuItems();
                    for(int i = 0; i < list.Count; i++)
                    {
                        var action = list[i] as DropdownMenuAction;
                        if(action != null && action.name == "Delete")
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
            var field = selectable as BlackboardField;
            if (field != null)
            {
                (var type, var domain) = (ValueTuple<Type, Domain>)field.userData;
                if (domain == Domain.Global)
                {
                    _inspector.Add(new VariableField(GlobalDatabase.Instance.GetVariable(field.text), field.text));
                }
                else
                {
                    _inspector.Add(new VariableField(_tree.Blackboard.GetVariable(field.text), field.text));
                }
            }
            var edge = selectable as Edge;
            if(edge != null && edge.userData != null)
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
            var field = selectable as BlackboardField;
            if (field != null)
            {
                (var type, var domain) = (ValueTuple<Type, Domain>)field.userData;
                BlackboardVariable v;
                if (domain == Domain.Global)
                {
                    v = GlobalDatabase.Instance.GetVariable(field.text);
                }
                else
                {
                    v = _tree.Blackboard.GetVariable(field.text);
                }
                foreach (var child in _inspector.Children())
                {
                    var n = child as VariableField;
                    if (n != null && n.Variable == v)
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
        public void Search(string text)
        {
            ClearSelection();
            var list = nodes.ToList();
            foreach(var node in list)
            {
                if (node.name.Contains(text, StringComparison.CurrentCultureIgnoreCase))
                {
                    AddToSelection(node);
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
                if(edge.TargetNode.InputEdges.Find(e=>e==actual) == default)
                {
                    edge.TargetNode.InputEdges.Add(actual);
                }
            }
            else
            {
                var temp = edge.TargetNode.InputEdges.Find(e=>e==actual);
                if(temp != default)
                {
                    edge.TargetNode.InputEdges.Remove(temp);
                }
            }
            if((type & SyncType.Push) != 0)
            {
                if(edge.SourceNode.OutputEdges.Find(e=>e==actual) == default)
                {
                    edge.SourceNode.OutputEdges.Add(actual);
                }
            }
            else
            {
                var temp = edge.SourceNode.OutputEdges.Find(e=>e==actual);
                if(temp != default)
                {
                    edge.SourceNode.OutputEdges.Remove(temp);
                }
            }
            if(type == 0)
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
        /// <summary>
        /// 变量定义域
        /// </summary>
        private enum Domain
        {
            Global,
            Prototype,
            Local
        }
        public new class UxmlFactory : UxmlFactory<TreeView, UxmlTraits> { }
    }
}
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

namespace GameFrame.Behavior.Tree.Editor
{
    /// <summary>
    /// ��Ϊ��ͼ
    /// </summary>
    public class TreeView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<TreeView, UxmlTraits> { }
        /// <summary>
        /// ��ǰ��ʾ����
        /// </summary>
        private BehaviorTree _tree;
        private Blackboard _blackboard;
        private TreeInspector _inspector;
        public override bool supportsWindowedBlackboard => true;
        public TreeView()
        {
            var background = new GridBackground();
            Insert(0, background);

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/NPC/BehaviorTree/Editor/USS/BehaviorTreeEditor.uss");
            styleSheets.Add(styleSheet);

            var searchProvider = new NodeSearchProvider();
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchProvider);

            RegisterCallback<DragUpdatedEvent>((e) =>
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            });
            RegisterCallback<DragPerformEvent>(e =>
            {
                Vector2 point =  contentViewContainer.WorldToLocal(e.localMousePosition);
                foreach(var field in _blackboard.selection)
                {
                    var type = typeof(VariableNode<>);
                    type = type.MakeGenericType(((field as BlackboardField).userData as Type).GenericTypeArguments[0]);
                    var view = CreateNode(type);
                    view.SetPosition(new Rect(point, Vector2.zero));
                    type.GetField("Index").SetValue(view.Node, (field as BlackboardField).text);
                    view.ChangeName((field as BlackboardField).text);
                }
            });

            _inspector = new TreeInspector();
            Add(_inspector);
            _inspector.visible = false;
        }
        /// <summary>
        /// ��ȡ�ڵ���ͼ
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        NodeView FindNodeView(BaseNode node)
        {
            return GetNodeByGuid(node.Guid) as NodeView;
        }
        /// <summary>
        /// ����������ѡ���tree�ı�ʱ�����¼�����ͼ
        /// </summary>
        /// <param name="tree"></param>
        internal void PopulateView(BehaviorTree tree)
        {
            _tree = tree;

            //ɾ����һ��������ͼ
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;

            if (_blackboard != null) ReleaseBlackboard(_blackboard);
            _blackboard = GetBlackboard();

            //Ϊ���������ԭ�нڵ�������ͼ
            Array.ForEach(_tree.GetNodes(), n => CreateNodeView(n));

            //Ϊ��������ı�������ͼ
            Array.ForEach(_tree.GetNodes(), node =>
             {
                 var view = FindNodeView(node);
                 //������Դ��
                 var sources = typeof(BaseNode).GetField("_sources", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(node) as List<SourceInfo>;
                 foreach(var source in sources)
                 {
                     var sourcePort = FindNodeView(source.SourceNode).outputContainer.Q<Port>(source.TargetField);
                     var edge = view.inputContainer.Q<Port>(source.SourceField).ConnectTo(sourcePort);
                     AddElement(edge);
                 }
                 //������Ϊ���
                 if(node is Node)
                 {
                     var children = ((Node)node).GetChildren();
                     if (children == null || children.Length == 0) return;

                     Array.ForEach(children, child =>
                     {
                         NodeView childView = FindNodeView(child);
                         Edge edge = view.outputContainer.Q<Port>("Next").ConnectTo(childView.inputContainer.Q<Port>("Pre"));
                         AddElement(edge);
                     });
                 }
             });

            _inspector.title = tree.name;
        }
        /// <summary>
        /// ɾ��ָ���ڵ�
        /// </summary>
        /// <param name="node"></param>
        private void DeleteNodeView(NodeView node)
        {
            var list = edges.ToList();
            //�Ƴ���ڵ����ӵı�
            for (int i = list.Count - 1; i >= 0; --i)
            {
                if (list[i].input.node == node || list[i].output.node == node)
                {
                    RemoveElement(list[i]);
                }
            }
            //�Ƴ������ͼ
            RemoveElement(node);
            //�Ƴ����ĵ�
            _tree.RemoveNode(node.Node);
        }
        /// <summary>
        /// ���ݴ���ڵ㴴���ڵ���ͼ
        /// </summary>
        /// <param name="node"></param>
        private NodeView CreateNodeView(BaseNode node)
        {
            NodeView nodeView = new NodeView(node);
            nodeView.OnDeleted = DeleteNodeView;
            AddElement(nodeView);
            return nodeView;
        }
        /// <summary>
        /// �����ڵ�
        /// </summary>
        /// <param name="type"></param>
        public NodeView CreateNode(Type type)
        {
            BaseNode node = _tree.CreateNode(type);
            return CreateNodeView(node);
        }
        /// <summary>
        /// ��ͼ�����仯ʱ
        /// </summary>
        /// <param name="graphViewChange"></param>
        /// <returns></returns>
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            //��Ԫ�ر��Ƴ�
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    //�Ƴ���
                    NodeView nodeView = elem as NodeView;
                    if (nodeView != null)
                    {
                        _tree.RemoveNode(nodeView.Node);
                    }

                    //�Ƴ���
                    Edge edge = elem as Edge;
                    if (edge != null)
                    {
                        NodeView parentView = edge.output.node as NodeView;
                        NodeView childView = edge.input.node as NodeView;
                        if (edge.input.name == "Pre" && edge.output.name == "Next")
                        {
                            _tree.DisconnectNode(parentView.Node as Node, childView.Node as Node);
                        }
                        else
                        {
                            _tree.DisconnectSource(childView.Node, new SourceInfo(parentView.Node, edge.output.name, edge.input.name));
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
                    if (edge.input.name == "Pre" && edge.output.name == "Next")
                    {
                        _tree.ConnectNode(parentView.Node as Node, childView.Node as Node);
                    }
                    else
                    {
                        _tree.ConnectSource(childView.Node, new SourceInfo(parentView.Node, edge.output.name, edge.input.name));
                    }
                });
            }

            return graphViewChange;
        }
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var list = ports.ToList().Where(endPort =>
                endPort.direction != startPort.direction
                && endPort.node != startPort.node
                && endPort.portType == startPort.portType
                ).ToList();
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
            Action<string, Type, bool, bool> addVariable = (string name, Type type, bool newItem, bool isLocal) =>
            {
                var field = new BlackboardField();
                blackboard.Add(field);
                field.text = name;
                field.typeText = variableTypeList[type];
                field.userData = type;
                bool init = newItem;
                string oldName = name;
                field.RegisterCallback<FocusOutEvent>((e) =>
                {
                    if (_tree.Blackboard.HasValueInTree(field.text) && field.text != oldName)
                    {
                        EditorUtility.DisplayDialog("Warring", "A variable with the same name already exists", "OK");
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
                if (newItem)
                {
                    field.OpenTextEditor();
                }
            };
            //����ȫ�ֱ���
            foreach (var variable in typeof(GlobalVariable).GetField("_variables", BindingFlags.NonPublic|BindingFlags.Instance).GetValue(GlobalVariable.Instance) as Dictionary<string, BlackboardVariable>)
            {
                var field = new BlackboardField();
                global.Add(field);
                field.text = variable.Key;
                field.typeText = variableTypeList[variable.Value.GetType()];
                field.userData = variable.Value.GetType();
            }
            blackboard.addItemRequested = (blackboard) =>
            {
                var menu = new GenericMenu();
                foreach (var item in variableTypeList)
                {
                    string name = item.Value;
                    menu.AddItem(new GUIContent(name), false, () => addVariable("", item.Key, true, true));
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
        /// ����ȫͼ
        /// </summary>
        public void SortGraph()
        {
            if (_tree == null) return;
            const float marginWidth = 25f;
            const float marginHeight = 20f;
            Dictionary<Node, Rect> treeRects = new Dictionary<Node, Rect>();
            //���������ľ��δ�С
            Utility.LambdaUtility.Fix<Node, Rect>(f => node =>
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
            //��������λ��
            Utility.LambdaUtility.Fix<Node, bool>(f => node =>
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
            if(_tree != null)
            {
                base.BuildContextualMenu(evt);
            }
        }
        public override void AddToSelection(ISelectable selectable)
        {
            base.AddToSelection(selectable);
            var node = selectable as NodeView;
            if(node != null)
            {
                _inspector.Add(new NodeField(node.Node));
            }
            var field = selectable as BlackboardField;
            if(field != null)
            {
                _inspector.Add(new VariableField(_tree.Blackboard.GetVariable(field.text), name));
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
                foreach(var child in _inspector.Children())
                {
                    var n = child as NodeField;
                    if(n != null && n.Node == node.Node)
                    {
                        _inspector.Remove(child);
                        break;
                    }
                }
            }
            var field = selectable as BlackboardField;
            if (field != null)
            {
                var v = _tree.Blackboard.GetVariable(field.text);
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
        }
        public void Search(string text)
        {
            nodes.ForEach(node =>
            {
                if (node.name.Contains(text))
                { 
                    AddToSelection(node);
                }
                else
                {
                    RemoveFromSelection(node);
                }
            });
        }
    }
}
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
using GameFrame.Interface;

namespace GameFrame.Editor
{
    public class TreeView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<TreeView, UxmlTraits> { }

        /// <summary>
        /// ��ǰ��ʾ����
        /// </summary>
        private ITree _tree;

        /// <summary>
        /// ����Ԫ�ر�ѡ��ʱ
        /// </summary>
        internal event Action<GraphElement> OnElementSelected;

        /// <summary>
        /// ��ѡ�е�Ԫ��
        /// </summary>
        public GraphElement SelectElement { get; private set; }

        public TreeView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviorTree/BehaviorTreeEditor.uss");
            styleSheets.Add(styleSheet);
        }

        /// <summary>
        /// ��ȡ�ڵ���ͼ
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        NodeView FindNodeView(INode node)
        {
            return GetNodeByGuid(node.Guid) as NodeView;
        }

        /// <summary>
        /// ����������ѡ���tree�ı�ʱ�����¼�����ͼ
        /// </summary>
        /// <remarks>
        /// �ú��������а�ȫ��飬�ڵ��øú�����ʱ����ȷ��������
        /// </remarks>
        /// <param name="tree"></param>
        internal void PopulateView(ITree tree)
        {
            _tree = tree;
            _tree.CorrectnessChecking();

            //ɾ����һ��������ͼ
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;

            //Ϊ���������ԭ�нڵ�������ͼ
            Array.ForEach(_tree.GetNodes(), n => CreateNodeView(n));

            //Ϊ��������ı�������ͼ
            Array.ForEach(_tree.GetNodes(), n =>
             {
                 //�ų����ӽڵ�Ľڵ�
                 var children = n.GetChildren();
                 if (children == null || children.Length == 0) return;


                 NodeView parentView = FindNodeView(n);
                 for (int i = 0; i < children.Length; ++i)
                 {
                     if (children[i] == null)
                     {
                         Debug.LogError($"{name}������Ϊ{i}���ӽڵ�����ȱʧ");
                         continue;
                     }

                     //����
                     NodeView childView = FindNodeView(children[i]);
                     Edge edge = parentView.Output[i].ConnectTo(childView.Input);
                     AddElement(edge);
                 }
             });
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
        private NodeView CreateNodeView(INode node)
        {
            NodeView nodeView = new NodeView(node);
            nodeView.OnNodeSelected = OnElementSelected;
            nodeView.OnDeleted = DeleteNodeView;
            AddElement(nodeView);
            return nodeView;
        }

        /// <summary>
        /// �����ڵ�
        /// </summary>
        /// <param name="type"></param>
        private NodeView CreateNode(Type type)
        {
            INode node = _tree.CreateNode(type);
            return CreateNodeView(node);
        }

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="subnode"></param>
        public void AddSubtree(INode subnode)
        {
            //��ȡ�����Ľڵ��б�
            List<INode> nodes = new List<INode>();
            Stack<INode> stack = new Stack<INode>();
            stack.Push(subnode);
            while (stack.Count > 0)
            {
                var node = stack.Pop();
                Array.ForEach(node.GetChildren(), child => { if (!nodes.Contains(child)) { stack.Push(child); } });
                nodes.Add(node);
            }

            //���ڵ���ӽ�����������ͼ
            nodes.ForEach(n =>
            {
                _tree.AddNode(n);
                CreateNodeView(n);
            });

            //���ӽڵ�
            nodes.ForEach(n =>
            {
                var children = n.GetChildren();
                if (children == null || children.Length == 0) return;
                NodeView parentView = FindNodeView(n);
                for (int i = 0; i < children.Length; ++i)
                {
                    if (children[i] == null) return; 
                    NodeView childView = FindNodeView(children[i]);

                    Edge edge = parentView.Output[i].ConnectTo(childView.Input);
                    AddElement(edge);
                }
            });
        }

        /// <summary>
        /// ����ͼ��
        /// </summary>
        /// <remarks>
        /// ���㼶��������
        /// </remarks>
        public void SortDown()
        {
            Vector2 ori = Vector2.zero;
            float yDifference = 150;
            float xDifference = 250;
            int lenth = 1;
            int ylenth = 0;

            List<INode> sorted = new List<INode>();

            Queue<INode> queue = new Queue<INode>();
            Queue<INode> next = new Queue<INode>();
            queue.Enqueue(_tree.RootNode);
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                Array.ForEach(node.GetChildren(), child => { if (!sorted.Contains(child)) { next.Enqueue(child); } });
                sorted.Add(node);
                FindNodeView(node).SetPosition(new Rect(ori.x + xDifference * lenth, ori.y + yDifference * ylenth++, 0, 0));
                if (queue.Count <= 0)
                {
                    queue = next;
                    next = new Queue<INode>();
                    ylenth = 0;
                    ++lenth;
                }
            }
        }

        /// <summary>
        /// ����ͼ��
        /// </summary>
        /// <remarks>
        /// ���㼶��������
        /// </remarks>
        public void SortMiddle()
        {
            Vector2 ori = Vector2.zero;
            float yDifference = 150;
            float xDifference = 250;
            int lenth = 1;
            int ylenth = 0;

            List<INode> sorted = new List<INode>();

            Queue<INode> queue = new Queue<INode>();
            Queue<INode> next = new Queue<INode>();
            queue.Enqueue(_tree.RootNode);
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                Array.ForEach(node.GetChildren(), child => { if (!sorted.Contains(child)) { next.Enqueue(child); } });
                sorted.Add(node);
                FindNodeView(node).SetPosition(new Rect(ori.x + xDifference * lenth, ori.y + yDifference * ylenth++, 0, 0));
                if (queue.Count <= 0)
                {
                    queue = next;
                    next = new Queue<INode>();
                    ylenth = -(queue.Count / 2);
                    ++lenth;
                }
            }
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
                        _tree.DisconnectNode(parentView.Node, childView.Node);
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
                    _tree.ConnectNode(parentView.Node, childView.Node);
                });
            }

            return graphViewChange;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var list = ports.ToList().Where(endPort =>
            endPort.direction != startPort.direction
            && endPort.node != startPort.node).ToList();
            return list;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (_tree == null) { return; }

            //�����ڵ������б��󶨻ص�����
            var list = _tree.GetNodeTypeTree();
            foreach(var pair in list)
            {
                evt.menu.AppendAction(pair.Value, e => CreateNode(pair.Key));
            }
        }
    }
}
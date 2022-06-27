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
        /// 当前显示的树
        /// </summary>
        private ITree _tree;

        /// <summary>
        /// 当有元素被选中时
        /// </summary>
        internal event Action<GraphElement> OnElementSelected;

        /// <summary>
        /// 被选中的元素
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
        /// 获取节点视图
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        NodeView FindNodeView(INode node)
        {
            return GetNodeByGuid(node.Guid) as NodeView;
        }

        /// <summary>
        /// 更新树，当选择的tree改变时，重新加载视图
        /// </summary>
        /// <remarks>
        /// 该函数不进行安全检查，在调用该函数的时候，请确保树存在
        /// </remarks>
        /// <param name="tree"></param>
        internal void PopulateView(ITree tree)
        {
            _tree = tree;
            _tree.CorrectnessChecking();

            //删除上一颗树的视图
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;

            //为传入的树的原有节点生成视图
            Array.ForEach(_tree.GetNodes(), n => CreateNodeView(n));

            //为传入的树的边生成视图
            Array.ForEach(_tree.GetNodes(), n =>
             {
                 //排除无子节点的节点
                 var children = n.GetChildren();
                 if (children == null || children.Length == 0) return;


                 NodeView parentView = FindNodeView(n);
                 for (int i = 0; i < children.Length; ++i)
                 {
                     if (children[i] == null)
                     {
                         Debug.LogError($"{name}的索引为{i}的子节点引用缺失");
                         continue;
                     }

                     //连接
                     NodeView childView = FindNodeView(children[i]);
                     Edge edge = parentView.Output[i].ConnectTo(childView.Input);
                     AddElement(edge);
                 }
             });
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
        private NodeView CreateNodeView(INode node)
        {
            NodeView nodeView = new NodeView(node);
            nodeView.OnNodeSelected = OnElementSelected;
            nodeView.OnDeleted = DeleteNodeView;
            AddElement(nodeView);
            return nodeView;
        }

        /// <summary>
        /// 创建节点
        /// </summary>
        /// <param name="type"></param>
        private NodeView CreateNode(Type type)
        {
            INode node = _tree.CreateNode(type);
            return CreateNodeView(node);
        }

        /// <summary>
        /// 添加子树
        /// </summary>
        /// <param name="subnode"></param>
        public void AddSubtree(INode subnode)
        {
            //获取子树的节点列表
            List<INode> nodes = new List<INode>();
            Stack<INode> stack = new Stack<INode>();
            stack.Push(subnode);
            while (stack.Count > 0)
            {
                var node = stack.Pop();
                Array.ForEach(node.GetChildren(), child => { if (!nodes.Contains(child)) { stack.Push(child); } });
                nodes.Add(node);
            }

            //将节点添加进树并创建视图
            nodes.ForEach(n =>
            {
                _tree.AddNode(n);
                CreateNodeView(n);
            });

            //连接节点
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
        /// 整理图表
        /// </summary>
        /// <remarks>
        /// 按层级向下排列
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
        /// 整理图标
        /// </summary>
        /// <remarks>
        /// 按层级居中排列
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
                        _tree.DisconnectNode(parentView.Node, childView.Node);
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

            //创建节点类型列表并绑定回调函数
            var list = _tree.GetNodeTypeTree();
            foreach(var pair in list)
            {
                evt.menu.AppendAction(pair.Value, e => CreateNode(pair.Key));
            }
        }
    }
}
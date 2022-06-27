//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Sirenix.OdinInspector;
//using Sirenix.Serialization;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif

//namespace GameFrame.Behavior.Tree
//{
//    /// <summary>
//    /// 重载节点的行为树
//    /// </summary>
//    [CreateAssetMenu(fileName = "行为树控制器Override", menuName = "角色/行为树控制器Override")]
//    public class BehaviorTreeOverride : SerializedScriptableObject, ICreateTree
//    {
//        [System.Serializable]
//        private class NodeSlot
//        {
//            /// <summary>
//            /// 替换后的节点
//            /// </summary>
//            public Node Node;
//#if UNITY_EDITOR
//            /// <summary>
//            /// 原型节点
//            /// </summary>
//            private Node _prototype;
//            private BehaviorTreeOverride _asset;

//            public NodeSlot(Node prototype, BehaviorTreeOverride asset)
//            {
//                _prototype = prototype;
//                _asset = asset;
//            }

//            [Button("复制对应节点")]
//            public void Copy()
//            {
//                Node = Instantiate(_prototype);
//                AssetDatabase.AddObjectToAsset(Node, _asset);
//                AssetDatabase.SaveAssets();
//            }
//#endif
//        }

//        /// <summary>
//        /// 原型行为树
//        /// </summary>
//        [OdinSerialize]
//        private BehaviorTree _prototype;
//        /// <summary>
//        /// 节点槽列表
//        /// </summary>
//        private Dictionary<Node, NodeSlot> _nodes;

//        public BehaviorTree Create(BehaviorTreeRunner runner)
//        {
//            var tree = _prototype.Create(runner);
//            foreach (var name in Nodes.Keys)
//            {
//                var node = tree._prototype.Nodes.Find(node => node.Name == (name + "(Clone)"));
//                if (node is RootNode) { continue; }

//                var replace = Nodes[name].Clone(true);
//                if (replace is CompositeNode)
//                {
//                    (replace as CompositeNode).Childrens = new List<Node>((node as CompositeNode).Childrens);
//                }
//                if (replace is DecoratorNode)
//                {
//                    (replace as DecoratorNode).Child = (node as DecoratorNode).Child;
//                }

//                var parent = tree._prototype.FindParent(node, tree._prototype.RootNode as Node);
//                if (parent is CompositeNode)
//                {
//                    var c = parent as CompositeNode;
//                    int n = c.Childrens.FindIndex(n => n == node);
//                    c.Childrens[n] = replace;
//                }
//                else if (parent is DecoratorNode)
//                {
//                    var c = parent as DecoratorNode;
//                    c.Child = replace;
//                }

//                tree._prototype.Nodes.Remove(node);
//                tree._prototype.Nodes.Add(replace);
//            }

//            return tree;
//        }

//#if UNITY_EDITOR
//        private void OnValidate()
//        {
//            RefreshList();
//        }

//        public void RefreshList()
//        {
//            if(_nodes == null)
//            {
//                _nodes = new Dictionary<Node, NodeSlot>();
//            }
//            var toRemove = new List<Node>();
//            var toAdd = new List<Node>();

//            if(_prototype == null)
//            {
//                foreach(var node in _nodes)
//                {
//                    if (!node.Value.Node) continue;
//                    AssetDatabase.RemoveObjectFromAsset(node.Value.Node);
//                    AssetDatabase.SaveAssets();
//                }
//                _nodes = null;
//                return;
//            }

//            foreach(var remove in _nodes.Keys)
//            {
//                if(_prototype.Nodes.Find(node=>node != remove) == null)
//                {
//                    toRemove.Add(remove);
//                }
//            }
//            foreach(var add in _prototype.Nodes)
//            {
//                if (!_nodes.ContainsKey(add))
//                {
//                    toAdd.Add(add);
//                }
//            }

//            toRemove.ForEach(remove => 
//            {
//                AssetDatabase.RemoveObjectFromAsset(_nodes[remove].Node);
//                _nodes.Remove(remove);
//                AssetDatabase.SaveAssets();
//            });
//            toAdd.ForEach(add =>
//            {
//                _nodes.Add(add, new NodeSlot(add, this));
//            });
//        }
//#endif
//    }
//}
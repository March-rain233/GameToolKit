using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Text;
using Cinemachine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

namespace GameFrame.Behavior.Tree
{
    using NodeStatus = Node.NodeStatus;
    /// <summary>
    /// 行为树
    /// </summary>
    [CreateAssetMenu(fileName = "行为树控制器", menuName = "角色/行为树控制器")]
    public class BehaviorTree : SerializedScriptableObject
    {
        [Serializable]
        public class TreeBlackboard : IBlackboard
        {
#if UNITY_EDITOR
            private Dictionary<string, Hashtable> _callbacks = new Dictionary<string, Hashtable>();
            public void RegisterCallback<T>(string name, Action<T> callback) where T : IBlackboard.BlackboardEventBase
            {
                (_callbacks[name][typeof(T)] as List<Action<T>>).Add(callback);
            }
            public void RemoveValue(string name)
            {
                Dictionary<string, BlackboardVariable> target;
                if (_local.ContainsKey(name))
                {
                    target = _local;
                }
                else if (_prototype.ContainsKey(name))
                {
                    target = _prototype;
                }
                else
                {
                    GlobalVariable.Instance.RemoveValue(name);
                    return;
                }
                target.Remove(name);
                var e = new IBlackboard.ValueRemoveEvent(this, name);
                foreach (var callback in _callbacks[name][typeof(IBlackboard.ValueRemoveEvent)] as List<Action<IBlackboard.ValueRemoveEvent>>)
                {
                    callback(e);
                }
                _callbacks.Remove(name);
            }
            public void RenameValue(string name, string newName)
            {
                Dictionary<string, BlackboardVariable> target;
                if (_local.ContainsKey(name))
                {
                    target = _local;
                }
                else if (_prototype.ContainsKey(name))
                {
                    target = _prototype;
                }
                else
                {
                    GlobalVariable.Instance.RenameValue(name, newName);
                    return;
                }
                {
                    var temp = target[name];
                    target.Remove(name);
                    target.Add(newName, temp);
                }
                var e = new IBlackboard.NameChangedEvent(this, name, newName);
                foreach (var callback in _callbacks[name][typeof(IBlackboard.NameChangedEvent)] as List<Action<IBlackboard.NameChangedEvent>>)
                {
                    callback(e);
                }
                {
                    var temp = _callbacks[name];
                    _callbacks.Remove(name);
                    _callbacks.Add(name, temp);
                }
            }
            public BlackboardVariable GetVariable(string name)
            {
                if (_local.ContainsKey(name))
                {
                    return _local[name];
                }
                else if (_prototype.ContainsKey(name))
                {
                    return _prototype[name];
                }
                else
                {
                    return GlobalVariable.Instance.GetVariable(name);
                }
            }
#endif
            /// <summary>
            /// 本地域
            /// </summary>
            /// <remarks>
            /// 每一个创建的实例独享该变量库
            /// </remarks>
            [SerializeField]
            private Dictionary<string, BlackboardVariable> _local = new Dictionary<string, BlackboardVariable>();
            /// <summary>
            /// 树域
            /// </summary>
            /// <remarks>
            /// 所有由同一棵树为模板创建的实例共享该变量库
            /// </remarks>
            [SerializeField]
            private Dictionary<string, BlackboardVariable> _prototype = new Dictionary<string, BlackboardVariable>();
            public T GetValue<T>(string name)
            {
                if (_local.ContainsKey(name))
                {
                    return (T)_local[name].Value;
                }
                else if (_prototype.ContainsKey(name))
                {
                    return (T)_prototype[name].Value;
                }
                else
                {
                    return GlobalVariable.Instance.GetValue<T>(name);
                }
            }
            public bool HasValue(string name)
            {
                return HasValueInTree(name) || GlobalVariable.Instance.HasValue(name);
            }
            /// <summary>
            /// 判断树内是否存在变量（不检查全局变量）
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public bool HasValueInTree(string name)
            {
                return _local.ContainsKey(name) || _prototype.ContainsKey(name);
            }
            public void SetValue(string name, object value)
            {
                Dictionary<string, BlackboardVariable> target;
                if (_local.ContainsKey(name))
                {
                    target = _local;
                }
                else if (_prototype.ContainsKey(name))
                {
                    target = _prototype;
                }
                else
                {
                    GlobalVariable.Instance.SetValue(name, value);
                    return;
                }
                target[name].Value = value;
            }
            public void AddLocalVariable(string name, BlackboardVariable variable)
            {
                _local.Add(name, variable);
#if UNITY_EDITOR
                if (EditorApplication.isPlaying) return;
                _callbacks.Add(name, new Hashtable()
                    {
                        {typeof(IBlackboard.NameChangedEvent),new List<Action<IBlackboard.NameChangedEvent>>()},
                        {typeof(IBlackboard.ValueRemoveEvent),new List<Action<IBlackboard.ValueRemoveEvent>>()},
                    });
#endif
            }
            public void AddPrototypeVariable(string name, BlackboardVariable variable)
            {
                _prototype.Add(name, variable);
#if UNITY_EDITOR
                if (EditorApplication.isPlaying) return;
                _callbacks.Add(name, new Hashtable()
                    {
                        {typeof(IBlackboard.NameChangedEvent),new List<Action<IBlackboard.NameChangedEvent>>()},
                        {typeof(IBlackboard.ValueRemoveEvent),new List<Action<IBlackboard.ValueRemoveEvent>>()},
                    });
#endif
            }
            public TreeBlackboard Clone()
            {
                var bb = new TreeBlackboard();
                foreach(var item in _local)
                {
                    bb.AddLocalVariable(item.Key, item.Value.Clone());
                }
                bb._prototype = _prototype;
                return bb;
            }
        }

        #region 编辑器相关成员
#if UNITY_EDITOR
        /// <summary>
        /// 移除节点
        /// </summary>
        /// <param name="node"></param>
        public void RemoveNode(BaseNode node)
        {
            Nodes.Remove(node);

            if (node is Node)
            {
                //移除与父节点的连接
                Node parent = null;
                for (int i = 0; i < Nodes.Count; ++i)
                {
                    if (Nodes[i] == node) { continue; }
                    parent = FindParent(node as Node, Nodes[i] as Node);
                    if (parent != null) 
                    {
                        DisconnectNode(parent, node as Node);
                        break; 
                    }
                }
            }
            //移除以该节点为源的资源边
            Nodes.ForEach(i =>
            {
                i.RemoveSource(node);
            });
        }
        /// <summary>
        /// 寻找指定行为节点的父节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Node FindParent(Node target, Node start)
        {
            if (target == start) { return start; }
            foreach (var child in start.GetChildren())
            {
                if (child as Node == target)
                {
                    return start;
                }
                else
                {
                    var p = FindParent(target, child as Node);
                    if (p != null)
                    {
                        return p;
                    }
                }
            }
            return null;
        }
        public BaseNode[] GetNodes()
        {
            return Nodes.ToArray();
        }
        public BaseNode CreateNode(Type type)
        {
            var node = Activator.CreateInstance(type) as BaseNode;

            int count = Nodes.FindAll(node => node.Name == type.Name).Count;
            string newName = type.Name;
            if (count > 0)
            {
                newName = newName + $"({count})";
            }
            node.Name = newName;
            node.Guid = GUID.Generate().ToString();
            Nodes.Add(node);
            return node;
        }
        public void ConnectNode(Node parent, Node child)
        {
            (parent as Node).AddChild(child as Node);
        }
        public void DisconnectNode(Node parent, Node child)
        {
            (parent as Node).RemoveChild(child as Node);
        }
        public void ConnectSource(BaseNode target, SourceInfo info)
        {
            target.AddSource(info);
        }
        public void DisconnectSource(BaseNode target, SourceInfo info)
        {
            target.RemoveSource(info);
        }
#endif
        #endregion

        /// <summary>
        /// 节点列表
        /// </summary>
        public List<BaseNode> Nodes = new List<BaseNode>();
        /// <summary>
        /// 根节点
        /// </summary>
        [SerializeField]
        private RootNode _rootNode = new RootNode();
        public RootNode RootNode => _rootNode;
        /// <summary>
        /// 变量字典
        /// </summary>
        [SerializeField]
        [NoSaveDuringPlay]
        public TreeBlackboard Blackboard = new TreeBlackboard();
        /// <summary>
        /// 当前树的运行对象
        /// </summary>
        public BehaviorTreeRunner Runner { get; private set; }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        public NodeStatus Tick()
        {
            if (_rootNode.Status == NodeStatus.Running ||_rootNode.Status == NodeStatus.None)
            {
                _rootNode.Tick();
            }
            return _rootNode.Status;
        }
        /// <summary>
        /// 建造运行树
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        public BehaviorTree Create(BehaviorTreeRunner runner)
        {
            //todo:更改树的创建算法
            var tree = Instantiate(this);
            tree.Nodes.Clear();
            //将参与运行的节点进行复制
            tree._rootNode = _rootNode.Clone(tree) as RootNode;
            //复制变量表
            tree.Blackboard = Blackboard.Clone();
            return tree;
        }
    }
}

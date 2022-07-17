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
using System.Linq;
using System.Text.RegularExpressions;

namespace GameFrame.Behavior.Tree
{
    using NodeStatus = ProcessNode.NodeStatus;
    /// <summary>
    /// 行为树
    /// </summary>
    [CreateAssetMenu(fileName = "BTree", menuName = "Behavior/Behavior Tree")]
    public class BehaviorTree : SerializedScriptableObject
    {
        [Serializable]
        public class TreeBlackboard : IBlackboard
        {
            [OdinSerialize]
            IBlackboard.CallBackList _callBackList = new IBlackboard.CallBackList();
            /// <summary>
            /// 本地域
            /// </summary>
            /// <remarks>
            /// 每一个创建的实例独享该变量库
            /// </remarks>
            [OdinSerialize]
            Dictionary<string, BlackboardVariable> _local = new Dictionary<string, BlackboardVariable>();
            /// <summary>
            /// 树域
            /// </summary>
            /// <remarks>
            /// 所有由同一棵树为模板创建的实例共享该变量库
            /// </remarks>
            [OdinSerialize]
            Dictionary<string, BlackboardVariable> _prototype = new Dictionary<string, BlackboardVariable>();
            #region 事件注册
            public void RegisterCallback<T>(string name, Action<T> callback) where T : IBlackboard.BlackboardEventBase
            {                    
                if (HasValueInTree(name))
                {

                    _callBackList.RegisterCallback(name, callback);
                }
                else
                {
                    GlobalDatabase.Instance.RegisterCallback(name, callback);
                }
            }

            public void UnregisterCallback<TEventType>(string name, Action<TEventType> callback) where TEventType : IBlackboard.BlackboardEventBase
            {
                if (HasValueInTree(name))
                {
                    _callBackList.UnregisterCallback(name, callback);
                }
                else
                {
                    GlobalDatabase.Instance.UnregisterCallback(name, callback);
                }
            }

            #endregion

            #region 变量增删查改
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
                    GlobalDatabase.Instance.RemoveValue(name);
                    return;
                }
                target[name].ValueChanged -= Variable_ValueChanged;
                target.Remove(name);
                var e = new IBlackboard.ValueRemoveEvent(this, name);
                _callBackList.Invoke(name, e);
                _callBackList.RemoveItem(name);
            }

            public void RenameValue(string oldName, string newName)
            {
                Dictionary<string, BlackboardVariable> target;
                if (_local.ContainsKey(oldName))
                {
                    target = _local;
                }
                else if (_prototype.ContainsKey(oldName))
                {
                    target = _prototype;
                }
                else
                {
                    GlobalDatabase.Instance.RenameValue(oldName, newName);
                    return;
                }
                {
                    var temp = target[oldName];
                    target.Remove(oldName);
                    target.Add(newName, temp);
                }
                var e = new IBlackboard.NameChangedEvent(this, newName, oldName);
                _callBackList.RenameItem(oldName, newName);
                _callBackList.Invoke(newName, e);
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
                    return GlobalDatabase.Instance.GetVariable(name);
                }
            }

            public Dictionary<string, BlackboardVariable> GetLocalVariables()
            {
                return new Dictionary<string, BlackboardVariable>(_local);
            }

            public Dictionary<string, BlackboardVariable> GetPrototypeVariables()
            {
                return new Dictionary<string, BlackboardVariable>(_prototype);
            }

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
                    return GlobalDatabase.Instance.GetValue<T>(name);
                }
            }

            public bool HasValue(string name)
            {
                return HasValueInTree(name) || GlobalDatabase.Instance.HasValue(name);
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
                    GlobalDatabase.Instance.SetValue(name, value);
                    return;
                }
                target[name].Value = value;
            }

            /// <summary>
            /// 添加本地域的变量
            /// </summary>
            /// <param name="name"></param>
            /// <param name="variable"></param>
            public void AddLocalVariable(string name, BlackboardVariable variable)
            {
                _local.Add(name, variable);
                variable.ValueChanged += Variable_ValueChanged;
            }

            /// <summary>
            /// 添加树域的变量
            /// </summary>
            /// <param name="name"></param>
            /// <param name="variable"></param>
            public void AddPrototypeVariable(string name, BlackboardVariable variable)
            {
                _prototype.Add(name, variable);
                variable.ValueChanged += Variable_ValueChanged;
            }

            /// <summary>
            /// 默认添加入本地域
            /// </summary>
            /// <param name="name"></param>
            /// <param name="variable"></param>
            /// <exception cref="NotImplementedException"></exception>
            public void AddVariable(string name, BlackboardVariable variable)
            {
                AddLocalVariable(name, variable);
            }

            private void Variable_ValueChanged(BlackboardVariable sender, object newValue, object oldValue)
            {
                foreach (var variable in _local)
                {
                    if (variable.Value == sender)
                    {
                        var e = new IBlackboard.ValueChangeEvent(this, variable.Key, newValue, oldValue);
                        _callBackList.Invoke(variable.Key, e);
                        return;
                    }
                }
                foreach (var variable in _prototype)
                {
                    if (variable.Value == sender)
                    {
                        var e = new IBlackboard.ValueChangeEvent(this, variable.Key, newValue, oldValue);
                        _callBackList.Invoke(variable.Key, e);
                        return;
                    }
                }
            }
            #endregion
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
        public void RemoveNode(Node node)
        {
            Nodes.Remove(node);
            //移除与父节点的连接
            var n = node as ProcessNode;
            if (n != null)
            {
                foreach(ProcessNode item in Nodes)
                {
                    var children = item.GetChildren();
                    if (children.Contains(node))
                        n.RemoveChild(n);
                }
            }
            //移除以该节点的资源边
            foreach(var child in Nodes)
            {
                for(int i = child.InputEdges.Count - 1; i >= 0; --i)
                    if(child.InputEdges[i].SourceNode == node)
                        child.InputEdges.RemoveAt(i);
                for(int i = child.OutputEdges.Count - 1; i >= 0; --i)
                    if(child.OutputEdges[i].TargetNode == node)
                        child.OutputEdges.RemoveAt(i);
            }
        }
        public Node CreateNode(Type type)
        {
            var node = Activator.CreateInstance(type) as Node;
            string newName = type.Name;
            int count = Nodes.FindAll(node => Regex.IsMatch(node.Name, @$"{newName}(\(\d+\))?$")).Count;
            if (count > 0)
            {
                newName = newName + $"({count})";
            }
            node.Name = newName;
            node.Guid = GUID.Generate().ToString();
            typeof(Node).GetProperty("BehaviorTree").SetValue(node, this);
            Nodes.Add(node);
            return node;
        }
#endif
        #endregion

        /// <summary>
        /// 节点列表
        /// </summary>
        public List<Node> Nodes = new List<Node>();
        /// <summary>
        /// 根节点
        /// </summary>
        [SerializeField]
        private RootNode _rootNode = new RootNode();
        public RootNode RootNode => _rootNode;
        /// <summary>
        /// 变量字典
        /// </summary>
        [NonSerialized, OdinSerialize]
        [NoSaveDuringPlay]
        public TreeBlackboard Blackboard = new TreeBlackboard();
        /// <summary>
        /// 当前树的运行对象
        /// </summary>
        public BehaviorTreeRunner Runner { get; private set; }
        public BehaviorTree()
        {
            typeof(Node).GetProperty("BehaviorTree").SetValue(_rootNode, this);
            Nodes.Add(_rootNode);
        }
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
            var tree = CreateInstance<BehaviorTree>();
            tree.Nodes.Clear();
            //复制黑板
            tree.Blackboard = Blackboard.Clone();
            foreach(var var in runner.Variables)
            {
                tree.Blackboard.RemoveValue(var.Key);
                tree.Blackboard.AddLocalVariable(var.Key, var.Value);
            }
            //复制全部节点
            foreach(var node in Nodes)
            {
                var n = node.Clone();
                tree.Nodes.Add(n);
                var r = n as RootNode;
                if (r != null)
                {
                    tree._rootNode = r;
                }
            }
            //连接节点
            foreach(var node in Nodes)
            {
                var clone = tree.FindNode(node.Guid);
                //连接输入/出边
                foreach(var edge in node.InputEdges)
                {
                    clone.InputEdges.Add(new SourceInfo(tree.FindNode(edge.SourceNode.Guid), clone, edge.SourceField, edge.TargetField));
                }
                foreach (var edge in node.OutputEdges)
                {
                    clone.OutputEdges.Add(new SourceInfo(clone, tree.FindNode(edge.TargetNode.Guid), edge.SourceField, edge.TargetField));
                } 
                //连接行为链
                var ori = node as ProcessNode;
                if (ori != null)
                {
                    var cn = clone as ProcessNode;
                    foreach (var child in ori.GetChildren())
                    {
                        cn.AddChild(tree.FindNode(child.Guid) as ProcessNode);
                    }
                }
            }
            //初始化节点
            foreach(var node in tree.Nodes)
            {
                node.Init(tree);
            }
            return tree;
        }
        /// <summary>
        /// 根据guid查找节点
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public Node FindNode(string guid)
        {
            return Nodes.Find(n=>n.Guid==guid);
        }
    }
}

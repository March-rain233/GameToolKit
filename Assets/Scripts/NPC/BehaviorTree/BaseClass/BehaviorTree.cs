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
    /// ��Ϊ��
    /// </summary>
    [CreateAssetMenu(fileName = "��Ϊ��������", menuName = "��ɫ/��Ϊ��������")]
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
            /// ������
            /// </summary>
            /// <remarks>
            /// ÿһ��������ʵ������ñ�����
            /// </remarks>
            [SerializeField]
            private Dictionary<string, BlackboardVariable> _local = new Dictionary<string, BlackboardVariable>();
            /// <summary>
            /// ����
            /// </summary>
            /// <remarks>
            /// ������ͬһ����Ϊģ�崴����ʵ������ñ�����
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
            /// �ж������Ƿ���ڱ����������ȫ�ֱ�����
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

        #region �༭����س�Ա
#if UNITY_EDITOR
        /// <summary>
        /// �Ƴ��ڵ�
        /// </summary>
        /// <param name="node"></param>
        public void RemoveNode(BaseNode node)
        {
            Nodes.Remove(node);

            if (node is Node)
            {
                //�Ƴ��븸�ڵ������
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
            //�Ƴ��Ըýڵ�ΪԴ����Դ��
            Nodes.ForEach(i =>
            {
                i.RemoveSource(node);
            });
        }
        /// <summary>
        /// Ѱ��ָ����Ϊ�ڵ�ĸ��ڵ�
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
        /// �ڵ��б�
        /// </summary>
        public List<BaseNode> Nodes = new List<BaseNode>();
        /// <summary>
        /// ���ڵ�
        /// </summary>
        [SerializeField]
        private RootNode _rootNode = new RootNode();
        public RootNode RootNode => _rootNode;
        /// <summary>
        /// �����ֵ�
        /// </summary>
        [SerializeField]
        [NoSaveDuringPlay]
        public TreeBlackboard Blackboard = new TreeBlackboard();
        /// <summary>
        /// ��ǰ�������ж���
        /// </summary>
        public BehaviorTreeRunner Runner { get; private set; }
        /// <summary>
        /// ����
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
        /// ����������
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        public BehaviorTree Create(BehaviorTreeRunner runner)
        {
            //todo:�������Ĵ����㷨
            var tree = Instantiate(this);
            tree.Nodes.Clear();
            //���������еĽڵ���и���
            tree._rootNode = _rootNode.Clone(tree) as RootNode;
            //���Ʊ�����
            tree.Blackboard = Blackboard.Clone();
            return tree;
        }
    }
}

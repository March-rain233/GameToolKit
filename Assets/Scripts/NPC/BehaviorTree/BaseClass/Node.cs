using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using GameFrame.Interface;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace GameFrame.Behavior.Tree
{
    public abstract class Node : ScriptableObject, INode
    {
        /// <summary>
        /// 节点状态
        /// </summary>
        [System.Serializable]
        public enum NodeStatus
        {
            /// <summary>
            /// 节点尚未运行过
            /// </summary>
            None,
            /// <summary>
            /// 节点运行成功
            /// </summary>
            Success,
            /// <summary>
            /// 节点运行失败
            /// </summary>
            Failure,
            /// <summary>
            /// 节点正在运行
            /// </summary>
            Running,
            /// <summary>
            /// 节点运行被打断
            /// </summary>
            Aborting,
        }

        #region 编辑器相关属性
#if UNITY_EDITOR
        [OdinSerialize, ReadOnly]
        public string Guid { get => _guid; set => _guid = value; }
        private string _guid;

        public string Name { get => name; set => name = value; }

        public Vector2 ViewPosition { get => _viewPosition; set => _viewPosition = value; }

        public abstract INode.PortType Input { get; }

        public abstract INode.PortType Output { get; }

        private Vector2 _viewPosition;

        public event Action<Color> OnColorChanged;
#endif
        #endregion

        /// <summary>
        /// 当前状态
        /// </summary>
        [OdinSerialize]
        public NodeStatus Status
        {
            get => _status;
            private set
            {
                _status = value;
#if UNITY_EDITOR
                switch (_status)
                {
                    case NodeStatus.Success:
                        OnColorChanged?.Invoke(Color.green);
                        break;
                    case NodeStatus.Failure:
                        OnColorChanged?.Invoke(Color.red);
                        break;
                    case NodeStatus.Running:
                        OnColorChanged?.Invoke(Color.blue);
                        break;
                    case NodeStatus.Aborting:
                        OnColorChanged?.Invoke(Color.yellow);
                        break;
                    default:
                        OnColorChanged?.Invoke(Color.gray);
                        break;
                }
#endif
            }
        }
        private NodeStatus _status = NodeStatus.None;

        /// <summary>
        /// 节点类型生成时需要的共享参数列表
        /// </summary>
        private static Dictionary<Type, System.Reflection.FieldInfo[]> InitList;

        /// <summary>
        /// 储存该节点的原始容器
        /// </summary>
        public INodeContainer ModelContainer;

        /// <summary>
        /// 节点绑定的行为树
        /// </summary>
        public BehaviorTree BehaviorTree { get; private set; }

        /// <summary>
        /// 节点绑定的运行对象
        /// </summary>
        public BehaviorTreeRunner Runner { get; private set; }

        /// <summary>
        /// 初始化绑定
        /// </summary>
        public void InitBinding(BehaviorTree tree)
        {
            BehaviorTree = tree;
            Runner = tree.Runner;
            InitCustomParameters();
            foreach(var node in GetChildren() as Node[])
            {
                node.InitBinding(tree);
            }
        }

        /// <summary>
        /// 外界调用更新
        /// </summary>
        /// <returns>该节点更新后的状态</returns>
        public NodeStatus Tick()
        {
            //当节点并未处在运行状态时，进行进入状态处理
            if(Status != NodeStatus.Running)
            {
                //当节点处于被打断状态时恢复运行
                if(Status == NodeStatus.Aborting) OnResume();
                //否则节点处于已结算状态，重新进入运行状态
                else OnEnter();
                Status = NodeStatus.Running;
            }
            Status = OnUpdate();
            //当节点运行结束后，进行退出状态处理
            if (Status != NodeStatus.Running) OnExit();
            return Status;
        }

        /// <summary>
        /// 当该节点进入运行时调用
        /// </summary>
        protected virtual void OnEnter() { }

        /// <summary>
        /// 当该节点退出运行时调用
        /// </summary>
        protected virtual void OnExit() { }

        /// <summary>
        /// 当该节点被打断时调用
        /// </summary>
        protected virtual void OnAbort() { }

        /// <summary>
        /// 当该节点恢复运行时调用
        /// </summary>
        protected virtual void OnResume() { }

        /// <summary>
        /// 打断当前节点的运行状态
        /// </summary>
        public void Abort()
        {
            //当节点未在运行时被打断输出错误信息
            if (Status != NodeStatus.Running)
            {
                Debug.Log($"{BehaviorTree.name}尝试打断[{GetType()}]{name}节点，但节点处于{Status}状态");
                return;
            }
            OnAbort();
            Status = NodeStatus.Aborting;
        }

        /// <summary>
        /// 当节点处于运行状态时调用
        /// </summary>
        /// <returns>本次运行的结果</returns>
        protected abstract NodeStatus OnUpdate();

        /// <summary>
        /// 克隆当前节点及子节点
        /// </summary>
        public abstract Node Clone();

        public abstract INode[] GetChildren();

        /// <summary>
        /// 获取所有的自定义变量字段
        /// </summary>
        private System.Reflection.FieldInfo[] FindCustomParameters()
        {
            var properties = GetType().GetFields(System.Reflection.BindingFlags.Instance 
                | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            properties.Where(prop => prop.FieldType.IsAssignableFrom(typeof(CustomParameters<>)));
            return properties;
        }

        /// <summary>
        /// 给所有自定义变量初始化
        /// </summary>
        private void InitCustomParameters()
        {
            System.Reflection.FieldInfo[] propertyInfos;
            if (InitList.ContainsKey(GetType()))
            {
                propertyInfos = InitList[GetType()];
            }
            else
            {
                propertyInfos = FindCustomParameters();
                InitList[GetType()] = propertyInfos;
            }
            //获取自定义变量的字段
            Type customtype = typeof(CustomParameters<>);
            var blackBoardField = customtype.GetField("_blackBoard");
            //对每一个有索引的字段设置黑板
            foreach (var info in propertyInfos)
            {
                var prop = info.GetValue(this);
                blackBoardField.SetValue(prop, BehaviorTree.BlackBoard);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="node"></param>
        public abstract void AddChild(Node node);

        /// <summary>
        /// 移除子节点
        /// </summary>
        /// <param name="node"></param>
        public abstract void RemoveChild(Node node);

        [Button("修改节点名")]
        public void ChangeName(string newName) 
        {
            name = newName;
        }

        [Button("更改GUID")]
        public void ChangeGuid()
        {
#if UNITY_EDITOR
            Guid = GUID.Generate().ToString();
#endif
        }

        [Button("保存到")]
        private void SaveAs()
        {
#if UNITY_EDITOR
            string path = EditorUtility.SaveFilePanelInProject("保存为", name, "asset", "一切我没想到的错误操作，我拒不负责！");
            Node clone = Clone();
            AssetDatabase.CreateAsset(clone, path);
            Stack<Node> stack = new Stack<Node>();
            stack.Push(clone);
            while (stack.Count > 0)
            {
                var node = stack.Pop();
                Array.ForEach(node.GetChildren(), child => stack.Push(child as Node));
                node.name = node.name.Replace("(Clone)", "");
                if (node != clone)
                {
                    AssetDatabase.AddObjectToAsset(node, clone);
                }
            }
#endif
        }
#endif
    }
}

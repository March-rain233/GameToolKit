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
        /// �ڵ�״̬
        /// </summary>
        [System.Serializable]
        public enum NodeStatus
        {
            /// <summary>
            /// �ڵ���δ���й�
            /// </summary>
            None,
            /// <summary>
            /// �ڵ����гɹ�
            /// </summary>
            Success,
            /// <summary>
            /// �ڵ�����ʧ��
            /// </summary>
            Failure,
            /// <summary>
            /// �ڵ���������
            /// </summary>
            Running,
            /// <summary>
            /// �ڵ����б����
            /// </summary>
            Aborting,
        }

        #region �༭���������
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
        /// ��ǰ״̬
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
        /// �ڵ���������ʱ��Ҫ�Ĺ�������б�
        /// </summary>
        private static Dictionary<Type, System.Reflection.FieldInfo[]> InitList;

        /// <summary>
        /// ����ýڵ��ԭʼ����
        /// </summary>
        public INodeContainer ModelContainer;

        /// <summary>
        /// �ڵ�󶨵���Ϊ��
        /// </summary>
        public BehaviorTree BehaviorTree { get; private set; }

        /// <summary>
        /// �ڵ�󶨵����ж���
        /// </summary>
        public BehaviorTreeRunner Runner { get; private set; }

        /// <summary>
        /// ��ʼ����
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
        /// �����ø���
        /// </summary>
        /// <returns>�ýڵ���º��״̬</returns>
        public NodeStatus Tick()
        {
            //���ڵ㲢δ��������״̬ʱ�����н���״̬����
            if(Status != NodeStatus.Running)
            {
                //���ڵ㴦�ڱ����״̬ʱ�ָ�����
                if(Status == NodeStatus.Aborting) OnResume();
                //����ڵ㴦���ѽ���״̬�����½�������״̬
                else OnEnter();
                Status = NodeStatus.Running;
            }
            Status = OnUpdate();
            //���ڵ����н����󣬽����˳�״̬����
            if (Status != NodeStatus.Running) OnExit();
            return Status;
        }

        /// <summary>
        /// ���ýڵ��������ʱ����
        /// </summary>
        protected virtual void OnEnter() { }

        /// <summary>
        /// ���ýڵ��˳�����ʱ����
        /// </summary>
        protected virtual void OnExit() { }

        /// <summary>
        /// ���ýڵ㱻���ʱ����
        /// </summary>
        protected virtual void OnAbort() { }

        /// <summary>
        /// ���ýڵ�ָ�����ʱ����
        /// </summary>
        protected virtual void OnResume() { }

        /// <summary>
        /// ��ϵ�ǰ�ڵ������״̬
        /// </summary>
        public void Abort()
        {
            //���ڵ�δ������ʱ��������������Ϣ
            if (Status != NodeStatus.Running)
            {
                Debug.Log($"{BehaviorTree.name}���Դ��[{GetType()}]{name}�ڵ㣬���ڵ㴦��{Status}״̬");
                return;
            }
            OnAbort();
            Status = NodeStatus.Aborting;
        }

        /// <summary>
        /// ���ڵ㴦������״̬ʱ����
        /// </summary>
        /// <returns>�������еĽ��</returns>
        protected abstract NodeStatus OnUpdate();

        /// <summary>
        /// ��¡��ǰ�ڵ㼰�ӽڵ�
        /// </summary>
        public abstract Node Clone();

        public abstract INode[] GetChildren();

        /// <summary>
        /// ��ȡ���е��Զ�������ֶ�
        /// </summary>
        private System.Reflection.FieldInfo[] FindCustomParameters()
        {
            var properties = GetType().GetFields(System.Reflection.BindingFlags.Instance 
                | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            properties.Where(prop => prop.FieldType.IsAssignableFrom(typeof(CustomParameters<>)));
            return properties;
        }

        /// <summary>
        /// �������Զ��������ʼ��
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
            //��ȡ�Զ���������ֶ�
            Type customtype = typeof(CustomParameters<>);
            var blackBoardField = customtype.GetField("_blackBoard");
            //��ÿһ�����������ֶ����úڰ�
            foreach (var info in propertyInfos)
            {
                var prop = info.GetValue(this);
                blackBoardField.SetValue(prop, BehaviorTree.BlackBoard);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// ����ӽڵ�
        /// </summary>
        /// <param name="node"></param>
        public abstract void AddChild(Node node);

        /// <summary>
        /// �Ƴ��ӽڵ�
        /// </summary>
        /// <param name="node"></param>
        public abstract void RemoveChild(Node node);

        [Button("�޸Ľڵ���")]
        public void ChangeName(string newName) 
        {
            name = newName;
        }

        [Button("����GUID")]
        public void ChangeGuid()
        {
#if UNITY_EDITOR
            Guid = GUID.Generate().ToString();
#endif
        }

        [Button("���浽")]
        private void SaveAs()
        {
#if UNITY_EDITOR
            string path = EditorUtility.SaveFilePanelInProject("����Ϊ", name, "asset", "һ����û�뵽�Ĵ���������Ҿܲ�����");
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

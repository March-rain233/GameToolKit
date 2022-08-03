//using System;
//using System.Collections;
//using System.Collections.Generic;
//#if UNITY_EDITOR
//using UnityEditor.Experimental.GraphView;
//using UnityEditor;
//#endif
//using UnityEngine;
//using Sirenix.OdinInspector;

//namespace Dialogue
//{
//    public abstract class Node : ScriptableObject, INode
//    {
//        [SerializeField, HideInInspector]
//        public string Guid { get; set; }

//        public string Name => name;

//        public Vector2 ViewPosition { get; set; }

//        public virtual bool IsRoot => false;

//        public virtual bool IsLeaf => false;

//#if UNITY_EDITOR
//        public virtual Port.Capacity Input => Port.Capacity.Multi;

//        public virtual Port.Capacity Output => Port.Capacity.Single;
//#endif

//        private bool _isStarted = false;

//        /// <summary>
//        /// ����ýڵ������
//        /// </summary>
//        [SerializeField]
//        private Condition _condition;

//        public event Action<string> OnNameChanged;
//        public event Action<Color> OnStatusChanged;

//        public abstract INode[] GetChildren();

//        /// <summary>
//        /// �жϽ���ýڵ�������Ƿ����
//        /// </summary>
//        /// <param name="tree"></param>
//        /// <returns></returns>
//        public bool JudgeCondition(DialogueTree tree)
//        {
//            if (_condition == null) { return true; }
//            return _condition.Reason(tree);
//        }

//        public virtual Node Tick(DialogueTree tree)
//        {
//            if (!_isStarted)
//            {
//                _isStarted = true;
//                OnEnter(tree);
//            }
//            var status = OnUpdate(tree);
//            if (status == NodeStatus.Running)
//            {
//                OnStatusChanged?.Invoke(Color.blue);
//                return this;
//            }
//            OnStatusChanged?.Invoke(Color.green);
//            _isStarted = false;
//            OnExit(tree);
//            return SelectChild(tree);
//        }

//        protected virtual void OnEnter(DialogueTree tree) { }

//        protected virtual void OnExit(DialogueTree tree) { }

//        protected abstract NodeStatus OnUpdate(DialogueTree tree);

//        protected abstract Node SelectChild(DialogueTree tree);

//        public virtual Node Clone()
//        {
//            var node = Instantiate(this);
//            node.name = node.name.Replace("(Clone)", "");
//            node.ViewPosition = ViewPosition;
//            node._isStarted = false;
//#if UNITY_EDITOR
//            node.Guid = GUID.Generate().ToString();
//#endif
//            return node;
//        }

//        [Button("�޸Ľڵ���")]
//        public void ChangeName(string newName)
//        {
//            name = newName;
//            OnNameChanged?.Invoke(name);
//        }

//#if UNITY_EDITOR
//        [Button("����GUID")]
//        public void ChangeGuid()
//        {
//            Guid = GUID.Generate().ToString();
//        }
//#endif
//    }
//}

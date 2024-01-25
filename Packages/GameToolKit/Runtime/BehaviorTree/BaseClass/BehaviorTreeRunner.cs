using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GameToolKit.Behavior.Tree
{
    /// <summary>
    /// ��Ϊ�����нű�
    /// </summary>
    [AddComponentMenu("AI/BehaviorTreeRunner")]
    public class BehaviorTreeRunner : SerializedMonoBehaviour
    {
        [OdinSerialize]
        public BehaviorTree ModelTree
        {
            get => _modelTree;
            set
            {
                SetModelTree(value);
            }
        }
        [SerializeField]
        private BehaviorTree _modelTree;

        public Dictionary<string, BlackboardVariable> Variables = new Dictionary<string, BlackboardVariable>();

        /// <summary>
        /// ��ǰ���е���
        /// </summary>
        public BehaviorTree RunTree { get; private set; } = null;

        private void Awake()
        {
            RunTree = ModelTree.CreateRunningTree(this);
        }

        private void OnEnable()
        {
            RunTree.Enable();
        }

        private void OnDisable()
        {
            RunTree.Disable();
        }

        private void Update()
        {
            if (RunTree.IsEnable && RunTree.Tick() == ProcessNode.NodeStatus.Success)
            {
                enabled = false;
            }
        }

        private void SetModelTree(BehaviorTree tree)
        {
            if (_modelTree == tree) return;
            Variables.Clear();
            _modelTree = tree;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// ��Ϊ�����нű�
    /// </summary>
    public class BehaviorTreeRunner : SerializedMonoBehaviour
    {
        public BehaviorTree ModelTreeSlot;

        public Dictionary<string, BlackboardVariable> Variables = new Dictionary<string, BlackboardVariable>();

        /// <summary>
        /// ��ǰ���е���
        /// </summary>
        public BehaviorTree RunTree { get; private set; } = null;

        private void Awake()
        {
            RunTree = ModelTreeSlot.Create(this);
        }

        private void Update()
        {
            RunTree.Tick();
        }
    }
}

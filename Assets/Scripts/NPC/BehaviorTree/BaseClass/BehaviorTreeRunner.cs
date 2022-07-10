using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// 行为树运行脚本
    /// </summary>
    public class BehaviorTreeRunner : SerializedMonoBehaviour
    {
        public BehaviorTree ModelTreeSlot;

        public Dictionary<string, BlackboardVariable> Variables = new Dictionary<string, BlackboardVariable>();

        /// <summary>
        /// 当前运行的树
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

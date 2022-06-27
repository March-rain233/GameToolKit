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
        /// <summary>
        /// 模板树放置槽
        /// </summary>
        public BehaviorTree ModelTreeSlot
        {
            get => _modelTreeSlot;
            set
            {
                RestoreLocalVariables(_modelTreeSlot, value);
            }
        }
        [SetProperty("ModelTreeSlot"),SerializeField,HideInInspector]
        private BehaviorTree _modelTreeSlot;

        /// <summary>
        /// 当前运行的树
        /// </summary>
        [HideInInspector]
        public BehaviorTree RunTree { get; private set; }

        /// <summary>
        /// 本地变量模板表
        /// </summary>
        public Dictionary<string, object> LocalVariables = new Dictionary<string, object>();

#if UNITY_EDITOR
        /// <summary>
        /// 重置本地变量库
        /// </summary>
        /// <param name="newTree"></param>
        private void RestoreLocalVariables(BehaviorTree oldTree, BehaviorTree newTree)
        {
            //清除与上一颗树的连接
            if(oldTree != null)
            {
                oldTree.ModelBlackBoard.LocalValueAdd -= AddLocalValueHandler;
                oldTree.ModelBlackBoard.LocalValueRemove -= RemoveLocalValueHandler;
                oldTree.ModelBlackBoard.LocalValueChanged -= ChangeLocalValueHandler;
            }

            //绑定下一颗树
            LocalVariables = new Dictionary<string, object>(newTree.ModelBlackBoard.LocalVariablesModel);
            newTree.ModelBlackBoard.LocalValueAdd += AddLocalValueHandler;
            newTree.ModelBlackBoard.LocalValueRemove += RemoveLocalValueHandler;
            newTree.ModelBlackBoard.LocalValueChanged += ChangeLocalValueHandler;
        }

        /// <summary>
        /// 当变量库模板变化
        /// </summary>
        /// <param name="index"></param>
        /// <param name="before"></param>
        /// <param name="after"></param>
        private void ChangeLocalValueHandler(string index, object before, object after)
        {
            //当且仅当对应的变量等于修改变量时（即对应变量未被自定义时）修改为修改后变量
            if (before.Equals(LocalVariables[index])) 
                LocalVariables[index] = after;
        }

        /// <summary>
        /// 当变量库模板变量增加时
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        private void AddLocalValueHandler(string index, object value)
        {
            LocalVariables[index] = value;
        }

        /// <summary>
        /// 但变量库模板变量移除时
        /// </summary>
        /// <param name="index"></param>
        private void RemoveLocalValueHandler(string index)
        {
            LocalVariables.Remove(index);
        }
#endif

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// 对话节点基类
    /// </summary>
    [NodeCategory("NULL")]
    [Node]
    public abstract class Node : BaseNode
    {
        /// <summary>
        /// 绑定的对话树
        /// </summary>
        [HideInGraphInspector]
        [ReadOnly]
        public DialogTree DialogTree;
        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init() =>
            OnInit();

        /// <summary>
        /// 当初始化
        /// </summary>
        protected virtual void OnInit() { }
    }
}
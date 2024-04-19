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
        [SerializeField, HideInInspector]
        public DialogGraph DialogTree;
    }
}
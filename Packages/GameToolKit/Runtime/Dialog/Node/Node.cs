using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// �Ի��ڵ����
    /// </summary>
    [NodeCategory("NULL")]
    [Node]
    public abstract class Node : BaseNode
    {
        /// <summary>
        /// �󶨵ĶԻ���
        /// </summary>
        [HideInGraphInspector]
        [ReadOnly]
        public DialogTree DialogTree;
        /// <summary>
        /// ��ʼ��
        /// </summary>
        public virtual void Init() =>
            OnInit();

        /// <summary>
        /// ����ʼ��
        /// </summary>
        protected virtual void OnInit() { }
    }
}
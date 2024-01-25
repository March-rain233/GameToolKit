using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace GameToolKit.Behavior.Tree
{
    [Serializable]
    [Node]
    [NodeColor("#E4007F")]
    [NodeCategory("NULL")]
    public abstract class Node : BaseNode
    {
        #region �༭����س�Ա
#if UNITY_EDITOR
        /// <summary>
        /// ���ڵ�߿���ɫ�ı�ʱ
        /// </summary>
        public event Action<Color> OnColorChanged;
        /// <summary>
        /// ���Ľڵ�߿���ɫ
        /// </summary>
        public void ChangeColor(Color color)
        {
            OnColorChanged?.Invoke(color);
        }
#endif
        #endregion

        #region �ֶ�������
        /// <summary>
        /// �ڵ�󶨵���Ϊ��
        /// </summary>
        [SerializeField, HideInInspector]
        public BehaviorTree BehaviorTree { get; private set; }
        #endregion

        #region �����������
        /// <summary>
        /// ��¡��ǰ�ڵ�
        /// </summary>
        /// <remarks>
        /// Ĭ��Ϊǳ��������Ҫ���ʱ���������
        /// </remarks>

        /// <summary>
        /// ������Ϊ��
        /// </summary>
        /// <param name="tree"></param>
        internal void SetTree(BehaviorTree tree)
        {
            BehaviorTree = tree;
        }

        /// <summary>
        /// ����Ϊ������ʱ
        /// </summary>
        internal protected virtual void OnEnable() { }

        /// <summary>
        /// ����Ϊ������ʱ
        /// </summary>
        internal protected virtual void OnDiable() { }
        #endregion
    }
}

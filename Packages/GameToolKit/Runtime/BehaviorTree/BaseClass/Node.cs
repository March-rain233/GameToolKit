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
        #region 编辑器相关成员
#if UNITY_EDITOR
        /// <summary>
        /// 当节点边框颜色改变时
        /// </summary>
        public event Action<Color> OnColorChanged;
        /// <summary>
        /// 更改节点边框颜色
        /// </summary>
        public void ChangeColor(Color color)
        {
            OnColorChanged?.Invoke(color);
        }
#endif
        #endregion

        #region 字段与属性
        /// <summary>
        /// 节点绑定的行为树
        /// </summary>
        [SerializeField, HideInInspector]
        public BehaviorTree BehaviorTree { get; private set; }
        #endregion

        #region 生命周期相关
        /// <summary>
        /// 设置行为树
        /// </summary>
        /// <param name="tree"></param>
        internal void SetTree(BehaviorTree tree)
        {
            BehaviorTree = tree;
        }

        /// <summary>
        /// 当行为树启用时
        /// </summary>
        internal protected virtual void OnEnable() { }

        /// <summary>
        /// 当行为树禁用时
        /// </summary>
        internal protected virtual void OnDiable() { }
        #endregion
    }
}

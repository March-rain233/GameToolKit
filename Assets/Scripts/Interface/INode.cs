using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine;

namespace GameFrame.Interface
{
    public interface INode
    {
#if UNITY_EDITOR
        /// <summary>
        /// 接口类型
        /// </summary>
        public enum PortType 
        { 
            /// <summary>
            /// 无接口
            /// </summary>
            None,
            /// <summary>
            /// 单接口
            /// </summary>
            Single,
            /// <summary>
            /// 多接口
            /// </summary>
            Multi
        }

        /// <summary>
        /// 唯一标识符
        /// </summary>
        string Guid { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 节点在视图上的位置
        /// </summary>
        Vector2 ViewPosition { get; set; }

        /// <summary>
        /// 当前节点的输入数量
        /// </summary>
        PortType Input { get; }

        /// <summary>
        /// 当前节点的输出数量
        /// </summary>
        PortType Output { get; }

        /// <summary>
        /// 当节点对应的运行颜色变化
        /// </summary>
        public event System.Action<Color> OnColorChanged;
#endif
        /// <summary>
        /// 获取子节点
        /// </summary>
        /// <returns></returns>
        INode[] GetChildren();
    }

}
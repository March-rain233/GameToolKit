using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// 定义视图中节点的固有属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NodeAttribute : Attribute
    {
        /// <summary>
        /// 流程端口类型
        /// </summary>
        public enum PortType
        {
            None,
            Single,
            Multi,
        }

        Color _color;
        /// <summary>
        /// 节点颜色
        /// </summary>
        public Color Color => _color;

        PortType _inputPort;
        /// <summary>
        /// 输入节点类型
        /// </summary>
        public PortType InputPort => _inputPort;

        PortType _outputPort;
        /// <summary>
        /// 输出节点类型
        /// </summary>
        public PortType OutputPort => _outputPort;

        public NodeAttribute(string hexColor, PortType inputPort = PortType.None, PortType outputPort = PortType.None)
        {
            ColorUtility.TryParseHtmlString(hexColor, out _color);
            _inputPort = inputPort;
            _outputPort = outputPort;
        }
    }
    /// <summary>
    /// 定义节点及子类的分类路径
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NodeCategoryAttribute : Attribute
    {
        string _category;
        public string Category => _category;
        public NodeCategoryAttribute(string category)
        {
            _category = category;
        }
    }
    /// <summary>
    /// 定义节点显示的默认名字
    /// </summary>
    /// <remarks>
    /// 如果附着该特性则默认显示类名
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NodeNameAttribute : Attribute
    {
        string _name;
        public string Name => _name;
        public NodeNameAttribute(string name)
        {
            _name = name;
        }
    }
}

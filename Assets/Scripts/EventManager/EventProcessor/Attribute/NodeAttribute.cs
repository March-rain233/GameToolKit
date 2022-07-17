using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFrame.EventProcessor
{
    /// <summary>
    /// 定义视图中节点及子节点的颜色
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NodeColorAttribute : Attribute
    {
        Color _color;
        /// <summary>
        /// 节点颜色
        /// </summary>
        public Color Color => _color;
        public NodeColorAttribute(string hexColor)
        {
            ColorUtility.TryParseHtmlString(hexColor, out _color);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// 定义视图中节点的属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NodeAttribute : Attribute
    {
        string _name;
        public string Name => _name;
        public NodeAttribute(string name)
        {
            _name = name;
        }
    }
    /// <summary>
    /// 设定节点颜色
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NodeColorAttribute : Attribute
    {
        Color _color;
        public Color Color => _color;
        public NodeColorAttribute(Color color)
        {
            _color = color;
        }
    }
}

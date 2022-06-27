using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// ������ͼ�нڵ������
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
    /// �趨�ڵ���ɫ
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

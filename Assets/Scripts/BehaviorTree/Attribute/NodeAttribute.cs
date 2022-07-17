using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// ������ͼ�нڵ�Ĺ�������
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NodeAttribute : Attribute
    {
        /// <summary>
        /// ���̶˿�����
        /// </summary>
        public enum PortType
        {
            None,
            Single,
            Multi,
        }

        Color _color;
        /// <summary>
        /// �ڵ���ɫ
        /// </summary>
        public Color Color => _color;

        PortType _inputPort;
        /// <summary>
        /// ����ڵ�����
        /// </summary>
        public PortType InputPort => _inputPort;

        PortType _outputPort;
        /// <summary>
        /// ����ڵ�����
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
    /// ����ڵ㼰����ķ���·��
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
    /// ����ڵ���ʾ��Ĭ������
    /// </summary>
    /// <remarks>
    /// ������Ÿ�������Ĭ����ʾ����
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

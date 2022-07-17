using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFrame.EventProcessor
{
    /// <summary>
    /// ������ͼ�нڵ㼰�ӽڵ����ɫ
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NodeColorAttribute : Attribute
    {
        Color _color;
        /// <summary>
        /// �ڵ���ɫ
        /// </summary>
        public Color Color => _color;
        public NodeColorAttribute(string hexColor)
        {
            ColorUtility.TryParseHtmlString(hexColor, out _color);
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

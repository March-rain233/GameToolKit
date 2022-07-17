using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameFrame
{
    /// <summary>
    /// ������ʾ�˿ڵ�����
    /// </summary>
    /// <remarks>
    /// �������дextendporttypes��������Ĭ��Ϊ����������
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class PortAttribute : Attribute
    {
        string _name;
        Direction _direction;
        Type[] _extendPortTypes;

        public string Name => _name;
        public Direction Direction => _direction; 
        public Type[] ExtendPortTypes => _extendPortTypes;
        /// <summary>
        /// �˿�����
        /// </summary>
        /// <param name="name">�˿���ʾ������</param>
        /// <param name="direction">�˿������������</param>
        /// <param name="extendPortTypes">����Ķ˿ڿ�ƥ�����������</param>
        public PortAttribute(string name, Direction direction, Type[] extendPortTypes = null)
        {
            _name = name;
            _direction = direction;
            _extendPortTypes = extendPortTypes;
        }
    }
    public enum Direction {
        Input,
        Output,
    }
}
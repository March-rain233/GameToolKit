using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameFrame
{
    /// <summary>
    /// 定义显示端口的类型
    /// </summary>
    /// <remarks>
    /// 如果不填写extendporttypes参数，则默认为变量的类型
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
        /// 端口类型
        /// </summary>
        /// <param name="name">端口显示的名字</param>
        /// <param name="direction">端口输入输出方向</param>
        /// <param name="extendPortTypes">额外的端口可匹配的数据类型</param>
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
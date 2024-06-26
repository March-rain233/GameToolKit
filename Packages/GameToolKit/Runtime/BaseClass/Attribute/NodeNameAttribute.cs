using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameToolKit
{
    /// <summary>
    /// 定义节点的默认名字
    /// </summary>
    /// <remarks>
    /// 如果未附着该特性则默认类名
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

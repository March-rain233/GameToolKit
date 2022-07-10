using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// 定义显示端口的类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class PortAttribute : Attribute
    {
        string _name;
        PortType _portType;

        public string Name => _name;
        public PortType PortType => _portType; 

        public PortAttribute(string name, PortType portType)
        {
            _name = name;
            _portType = portType;
        }
    }
    public enum PortType {
        Input,
        Output,
    }
}
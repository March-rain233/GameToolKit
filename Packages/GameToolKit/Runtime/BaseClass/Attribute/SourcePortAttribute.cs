using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameToolKit
{
    /// <summary>
    /// 定义资源端口的类型
    /// </summary>
    /// <remarks>
    /// 如果不填写groupList参数，则默认为变量的类型
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited =true)]
    public class SourcePortAttribute : PortAttribute
    {
        /// <summary>
        /// 是否显示内部属性
        /// </summary>
        public bool ShowInterior { get; private set; } = false;

        /// <summary>
        /// 端口类型
        /// </summary>
        /// <param name="name">端口显示的名字</param>
        /// <param name="direction">端口输入输出方向</param>
        /// <param name="extendPortTypes">额外的端口可匹配的数据类型</param>
        public SourcePortAttribute(string portName, PortDirection direction, string[] groupList = null, PortFilterType filterType = PortFilterType.Whitelist) 
            : base(portName, direction, direction == PortDirection.Input ? PortCapacity.Single : PortCapacity.Multi, filterType, groupList)
        {
            ShowInterior = false;
        }
        /// <summary>
        /// 端口类型
        /// </summary>
        /// <param name="portName">端口显示的名字</param>
        /// <param name="direction">端口输入输出方向</param>
        /// <param name="isMemberFields">是否是暴露内部字段而不是对象本身</param>
        public SourcePortAttribute(string portName, PortDirection direction, bool showInterior)
            : base(portName, direction, direction == PortDirection.Input ? PortCapacity.Single : PortCapacity.Multi, PortFilterType.Whitelist, null)
        {
            ShowInterior = showInterior;
        }
    }
}
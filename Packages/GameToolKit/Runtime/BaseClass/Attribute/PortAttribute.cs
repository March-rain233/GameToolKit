using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameToolKit
{
    /// <summary>
    /// 端口基类
    /// </summary>
    public abstract class PortAttribute : Attribute
    {
        /// <summary>
        /// 端口方向
        /// </summary>
        public PortDirection Direction { get; private set; }
        /// <summary>
        /// 过滤类型
        /// </summary>
        public PortFilterType FilterType { get; private set; }

        /// <summary>
        /// 端口额外分组清单
        /// </summary>
        public string[] GroupList { get; private set; }

        /// <summary>
        /// 端口显示名称
        /// </summary>
        public string PortName { get; private set; }

        /// <summary>
        /// 端口容量
        /// </summary>
        public PortCapacity Capacity { get; private set; }

        protected PortAttribute(string portName, PortDirection direction, PortCapacity capacity, PortFilterType filterType, string[] groupList)
        {
            Direction = direction;
            FilterType = filterType;
            Capacity = capacity;
            GroupList = groupList;
            PortName = portName;
        }
    }

    /// <summary>
    /// 端口过滤类型
    /// </summary>
    public enum PortFilterType
    {
        Whitelist,
        Blacklist
    }

    /// <summary>
    /// 端口方向
    /// </summary>
    public enum PortDirection
    {
        Input,
        Output,
    }

    /// <summary>
    /// 端口容量
    /// </summary>
    public enum PortCapacity
    {
        Single,
        Multi,
        None,
    }
}

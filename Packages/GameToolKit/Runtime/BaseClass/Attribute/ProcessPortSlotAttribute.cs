using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameToolKit
{
    /// <summary>
    /// 流程端口槽
    /// </summary>
    /// <remarks>拥有此Attribute的节点可作为流程节点的后继</remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited =true)]
    public class ProcessPortSlotAttribute : Attribute
    {
        public string PortName { get; private set; }

        public PortCapacity Capacity { get; private set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameToolKit
{
    /// <summary>
    /// 流程端口标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ProcessPortAttribute : PortAttribute
    {
        /// <summary>
        /// 所在组
        /// </summary>
        public string Group { get; private set; }


    }
}

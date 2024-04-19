using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameToolKit
{
    public enum Domain
    {
        /// <summary>
        /// 全局共享
        /// </summary>
        Global,
        /// <summary>
        /// 树间共享
        /// </summary>
        Prototype,
        /// <summary>
        /// 本地变量
        /// </summary>
        Local
    }
}

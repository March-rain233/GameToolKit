using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// 对话参数
    /// </summary>
    public struct DialogArgument
    {
        /// <summary>
        /// 对话正文
        /// </summary>
        public string Text;
        /// <summary>
        /// 对话角色id
        /// </summary>
        public string RoleId;
        /// <summary>
        /// 额外控制参数
        /// </summary>
        public object[] ExtendedArguments;
    }
}

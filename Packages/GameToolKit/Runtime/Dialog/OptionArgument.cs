using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// 选项参数
    /// </summary>
    public struct OptionArgument
    {
        /// <summary>
        /// 选项文本
        /// </summary>
        public string Context;

        /// <summary>
        /// 该选项是否可用
        /// </summary>
        public bool IsEnable;

        /// <summary>
        /// 当不可用时是否隐藏该选项
        /// </summary>
        public bool HideWhenDisable;

        /// <summary>
        /// 额外控制参数
        /// </summary>
        public object[] ExtendedArguments;
    }
}

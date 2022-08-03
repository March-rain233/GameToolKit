using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Dialog
{
    /// <summary>
    /// 对话参数基类
    /// </summary>
    public abstract class DialogArgument
    {
        /// <summary>
        /// 正文
        /// </summary>
        [TextArea]
        public string Text;
    }
    /// <summary>
    /// 普通的文本参数
    /// </summary>
    public class NormalText : DialogArgument { }
    /// <summary>
    /// 选择项参数
    /// </summary>
    public class ChoiceText : DialogArgument 
    {
        /// <summary>
        /// 该选项是否启用
        /// </summary>
        public bool isEnable;
    }
}

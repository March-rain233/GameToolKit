using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// 对话框接口
    /// </summary>
    public interface IDialogBox
    {
        /// <summary>
        /// 播放对话
        /// </summary>
        /// <param name="argument">对话参数</param>
        /// <param name="onDialogEnd">对话执行完毕回调</param>
        public void PlayDialog(DialogArgument argument, Action onDialogEnd);

        /// <summary>
        /// 关闭对话框
        /// </summary>
        /// <remarks>
        /// 当对话框无对话树占用时，由系统调用
        /// </remarks>
        public void CloseDialogBox();
    }
}

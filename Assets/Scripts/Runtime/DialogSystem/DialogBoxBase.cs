using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace GameFrame.Dialog
{
    /// <summary>
    /// 对话框基类
    /// </summary>
    public abstract class DialogBoxBase :SerializedMonoBehaviour
    {
        public static DialogBoxBase GetDialogBox(Type dialogBoxType)
        {
            var dialogBox = FindObjectOfType<DialogBoxBase>();
            if (dialogBox == null)
            {
                //todo:创建对话框

                dialogBox.InitDialog();
            }
            return dialogBox;
        }
        /// <summary>
        /// 初始化对话框
        /// </summary>
        protected abstract void InitDialog();

        /// <summary>
        /// 隐藏对话框
        /// </summary>
        public abstract void HideDialog();

        /// <summary>
        /// 显示对话框
        /// </summary>
        public abstract void ShowDialog();

        /// <summary>
        /// 终结对话框
        /// </summary>
        public abstract void DestoryDialog();

        /// <summary>
        /// 播放对话
        /// </summary>
        /// <param name="argument"></param>
        public abstract void PlayDialog(DialogArgument argument, System.Action onDialogEnd = null);
    }
}

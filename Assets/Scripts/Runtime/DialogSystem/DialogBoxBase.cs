using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace GameFrame.Dialog
{
    /// <summary>
    /// �Ի������
    /// </summary>
    public abstract class DialogBoxBase :SerializedMonoBehaviour
    {
        public static DialogBoxBase GetDialogBox(Type dialogBoxType)
        {
            var dialogBox = FindObjectOfType<DialogBoxBase>();
            if (dialogBox == null)
            {
                //todo:�����Ի���

                dialogBox.InitDialog();
            }
            return dialogBox;
        }
        /// <summary>
        /// ��ʼ���Ի���
        /// </summary>
        protected abstract void InitDialog();

        /// <summary>
        /// ���ضԻ���
        /// </summary>
        public abstract void HideDialog();

        /// <summary>
        /// ��ʾ�Ի���
        /// </summary>
        public abstract void ShowDialog();

        /// <summary>
        /// �ս�Ի���
        /// </summary>
        public abstract void DestoryDialog();

        /// <summary>
        /// ���ŶԻ�
        /// </summary>
        /// <param name="argument"></param>
        public abstract void PlayDialog(DialogArgument argument, System.Action onDialogEnd = null);
    }
}

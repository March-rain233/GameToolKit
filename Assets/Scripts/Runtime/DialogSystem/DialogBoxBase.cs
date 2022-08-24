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
    public abstract class DialogBoxBase : PanelBase
    {
        public static DialogBoxBase GetDialogBox(Type dialogBoxType)
        {
            var dialogBox = FindObjectOfType<DialogBoxBase>();
            if (dialogBox == null)
            {

            }
            return dialogBox;
        }

        /// <summary>
        /// ���ŶԻ�
        /// </summary>
        /// <param name="argument"></param>
        public abstract void PlayDialog(NormalText argument, System.Action onDialogEnd = null);
    }
}

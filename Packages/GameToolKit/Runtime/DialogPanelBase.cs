using GameToolKit.Dialog;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameToolKit
{
    /// <summary>
    /// �Ի���������
    /// </summary>
    public abstract class DialogPanelBase : PanelBase, IDialogBox
    {
        public abstract void CloseDialogBox();
        [Button]
        public abstract void PlayDialog(DialogArgument argument, Action onDialogEnd = null);
    }
}

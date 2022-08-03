using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFrame.Dialog
{
    /// <summary>
    /// 具有选择框的对话框基类
    /// </summary>
    public abstract class OptionalDialogBoxBase : DialogBoxBase
    {
        public abstract void ShowOptions(List<ChoiceText> options, Action<int> onSelected);
    }
}

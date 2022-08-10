using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFrame.Dialog
{
    /// <summary>
    /// ����ѡ���ĶԻ������
    /// </summary>
    public abstract class OptionalDialogBoxBase : DialogBoxBase
    {
        public abstract void ShowOptions(List<OptionText> options, Action<int> onSelected);
    }
}

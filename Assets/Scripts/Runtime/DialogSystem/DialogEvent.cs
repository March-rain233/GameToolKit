using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Dialog
{
    /// <summary>
    /// 对话事件
    /// </summary>
    /// <remarks>
    /// 通过发送该事件来通知对话系统播放对话
    /// </remarks>
    public class DialogEvent : EventBase
    {
        public DialogTree Dialog;
    }
}
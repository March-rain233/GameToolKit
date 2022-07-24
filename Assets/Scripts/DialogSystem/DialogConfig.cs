using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameFrame.Dialog
{
    [CreateAssetMenu(fileName = "Dialog Config", menuName = "Dialog/Dialog Config")]
    public class DialogConfig : SerializedScriptableObject
    {
        /// <summary>
        /// 加载入游戏内的对话
        /// </summary>
        public List<DialogTree> Dialogs;
    }
}
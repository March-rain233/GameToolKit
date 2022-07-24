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
        /// ��������Ϸ�ڵĶԻ�
        /// </summary>
        public List<DialogTree> Dialogs;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace GameFrame.Dialog {
    public class MainDialog : OptionalDialogBoxBase
    {
        /// <summary>
        /// ÎÄ±¾¿ØÖÆÆ÷
        /// </summary>
        public TextMeshPro TextController;

        public override void DestoryDialog()
        {
            throw new NotImplementedException();
        }

        public override void HideDialog()
        {
            throw new NotImplementedException();
        }

        public override void PlayDialog(DialogArgument argument, Action onDialogEnd = null)
        {
            throw new NotImplementedException();
        }

        public override void ShowDialog()
        {
            throw new NotImplementedException();
        }

        public override void ShowOptions(List<ChoiceText> options, Action<int> onSelected)
        {
            throw new NotImplementedException();
        }

        protected override void InitDialog()
        {
            throw new NotImplementedException();
        }
    }
}

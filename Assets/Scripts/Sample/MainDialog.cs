using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Sirenix.OdinInspector;

namespace GameFrame.Dialog {
    public class MainDialog : OptionalDialogBoxBase
    {
        /// <summary>
        /// ÎÄ±¾¿ØÖÆÆ÷
        /// </summary>
        public TextMeshProUGUI TextController;

        public override void DestoryDialog()
        {

        }

        public override void HideDialog()
        {
            throw new NotImplementedException();
        }

        [Button]
        public override void PlayDialog(NormalText argument, Action onDialogEnd = null)
        {
            var processor = new TextEffectProcessor(argument.Text, TextController);
            processor.OnAllCharactersVisiable += onDialogEnd;
            StopAllCoroutines();
            StartCoroutine(processor.Process());
        }

        public override void ShowDialog()
        {
            throw new NotImplementedException();
        }

        public override void ShowOptions(List<OptionText> options, Action<int> onSelected)
        {
            throw new NotImplementedException();
        }

        protected override void InitDialog()
        {
            throw new NotImplementedException();
        }
    }
}

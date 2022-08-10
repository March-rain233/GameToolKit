using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Linq;
using System.Text.RegularExpressions;
using GameFrame.Editor;

namespace GameFrame.Dialog.Editor
{
    [Tag("Layout/Line-Height", false)]
    public class LineHeightTagView : LengthTagView
    {
        public override string Tag => "line-height";

        protected override string _fieldName => "LineHeight";
    }
}

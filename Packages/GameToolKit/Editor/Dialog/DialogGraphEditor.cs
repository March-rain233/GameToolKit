using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using GameToolKit.Editor;

namespace GameToolKit.Dialog.Editor
{
    public class DialogGraphEditor : DataFlowGraphEditor<DialogGraph, Node>
    {
        [MenuItem("GameToolKit/Dialog Graph Editor")]
        public static void ShowExample()
        {
            DialogGraphEditor wnd = GetWindow<DialogGraphEditor>();
            wnd.titleContent = new GUIContent("Dialog Tree Editor");
        }
    }
}
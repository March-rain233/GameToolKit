using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameFrame.Behavior.Tree.Editor
{
    public class VariableField : GraphElement
    {
        public BlackboardVariable Variable;
        private class Assist : SerializedScriptableObject
        {
            public BlackboardVariable Variable;
            [HideInInspector]
            public string VariableName;
        }
        [CustomEditor(typeof(Assist))]
        private class AssistEditor : Sirenix.OdinInspector.Editor.OdinEditor
        {
            bool foldout = true;
            Assist assist => target as Assist;
            public override void OnInspectorGUI()
            {
                SirenixEditorGUI.BeginBox();
                SirenixEditorGUI.BeginBoxHeader();
                foldout = SirenixEditorGUI.Foldout(foldout, assist.VariableName);
                SirenixEditorGUI.EndBoxHeader();
                if(foldout)
                {
                    Tree.BeginDraw(true);
                    var children = Tree.GetPropertyAtPath("Variable").Children;
                    foreach(var child in children)
                    {
                        child.Draw();
                    }
                    Tree.EndDraw();
                }
                SirenixEditorGUI.EndBox();
            }
        }
        UnityEditor.Editor _editor;
        public VariableField(BlackboardVariable variable, string name)
        {
            Assist test = ScriptableObject.CreateInstance<Assist>();
            test.Variable = variable;
            test.VariableName = name;
            _editor = Sirenix.OdinInspector.Editor.OdinEditor.CreateEditor(test);
            IMGUIContainer container = new IMGUIContainer(() => { _editor.OnInspectorGUI(); });
            Add(container);
        }
    }
}
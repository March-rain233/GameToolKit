using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using Sirenix.OdinInspector;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
using System.Linq;

namespace GameFrame.Editor
{
    public class NodeField : GraphElement
    {
        public BaseNode Node;
        private class Assist : SerializedScriptableObject
        {
            public BaseNode Node;
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
                foldout = SirenixEditorGUI.Foldout(foldout, assist.Node.Name);
                SirenixEditorGUI.EndBoxHeader();
                if (foldout)
                {
                    Tree.BeginDraw(true);
                    var property = Tree.GetPropertyAtPath("Node");
                    var children = property.Children;
                    foreach (var child in children)
                    {
                        if(child.Attributes.Where(a=>a.GetType()==typeof(HideInTreeInspectorAttribute)).Count() > 0)
                        {
                            continue;
                        }
                        child.Draw();
                    }
                    Tree.EndDraw();
                }
                SirenixEditorGUI.EndBox();
            }
        }
        UnityEditor.Editor _editor;
        public NodeField(BaseNode node)
        {
            Assist test = ScriptableObject.CreateInstance<Assist>();
            Node = node;
            test.Node = node;
            _editor = Sirenix.OdinInspector.Editor.OdinEditor.CreateEditor(test);
            IMGUIContainer container = new IMGUIContainer(() => { _editor.OnInspectorGUI(); });
            Add(container);
        }
    }
}

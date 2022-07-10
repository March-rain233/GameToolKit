using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using Sirenix.OdinInspector;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameFrame.Behavior.Tree.Editor
{
    public class NodeField : GraphElement
    {
        public BaseNode Node;
        private class Assist : SerializedScriptableObject
        {
            [SerializeField]
            public BaseNode Node;
        }
        [CustomEditor(typeof(Assist))]
        private class AssistEditor : Sirenix.OdinInspector.Editor.OdinEditor
        {
            public override void OnInspectorGUI()
            {
                EditorGUILayout.LabelField((target as Assist).Node.Name);
                base.OnInspectorGUI();
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

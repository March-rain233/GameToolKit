using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace GameFrame.Editor
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

        private UnityEditor.Editor _editor;

        public InspectorView()
        {

        }

        internal void UpdateSelection(GraphElement element)
        {
            Clear();

            UnityEngine.Object.DestroyImmediate(_editor);

            NodeView nodeView = element as NodeView;
            if (element != null)
            {
                _editor = UnityEditor.Editor.CreateEditor(nodeView.Node as UnityEngine.Object);
                IMGUIContainer container = new IMGUIContainer(_editor.OnInspectorGUI);
                Add(container);
            }
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
namespace GameToolKit.Editor
{
    public class SourceEdgeField : GraphElementField
    {
        private class Assist : SerializedScriptableObject
        {
            [ReadOnly]
            public SourceEdge info;
            public SyncType syncType;
            public System.Action<SourceEdge, SyncType> callback;
        }
        [CustomEditor(typeof(Assist))]
        private class AssistEditor : Sirenix.OdinInspector.Editor.OdinEditor
        {
            Assist assist => target as Assist;
            bool foldout = true;
            public override void OnInspectorGUI()
            {
                SirenixEditorGUI.BeginBox();
                SirenixEditorGUI.BeginBoxHeader();
                foldout = SirenixEditorGUI.Foldout(foldout, $"{assist.info.SourceNode.Name}.{assist.info.SourceField}->{assist.info.TargetNode.Name}.{assist.info.TargetField}");
                SirenixEditorGUI.EndBoxHeader();
                if (foldout)
                {
                    var oldType = assist.syncType;
                    assist.syncType = (SyncType)SirenixEditorFields.EnumDropdown("Sync Type", assist.syncType);
                    if(oldType != assist.syncType)
                    {
                        assist.callback(assist.info, assist.syncType);
                    }
                }
                SirenixEditorGUI.EndBox();
            }
        }

        private SourceEdge _instance;
        public SourceEdgeField(SourceEdge info, SyncType type, System.Action<SourceEdge, SyncType> callback)
        {
            _instance = info;
            Assist test = ScriptableObject.CreateInstance<Assist>();
            test.callback = callback;
            test.syncType = type;
            test.info = info;
            var editor = UnityEditor.Editor.CreateEditor(test);
            IMGUIContainer container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
            Add(container);
        }

        public override bool IsAssociatedWith(object obj)
        {
            SourceEdge info = (SourceEdge)obj;
            return _instance.SourceNode == info.SourceNode && _instance.TargetNode == info.TargetNode &&
                _instance.SourceField == info.SourceField && _instance.TargetField == info.TargetField;
        }
    }
}

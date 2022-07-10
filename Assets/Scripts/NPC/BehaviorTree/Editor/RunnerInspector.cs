using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.Utilities.Editor;

namespace GameFrame.Behavior.Tree.Editor
{
    [CustomEditor(typeof(BehaviorTreeRunner))]
    public class RunnerInspector : Sirenix.OdinInspector.Editor.OdinEditor
    {
        BehaviorTreeRunner _behaviorTreeRunner => target as BehaviorTreeRunner;
        bool _foldout = true;
        GUITable _table;
        protected override void OnEnable()
        {
            base.OnEnable();
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!EditorApplication.isPlaying)
            {
                _behaviorTreeRunner.ModelTreeSlot = SirenixEditorFields.UnityObjectField(new GUIContent("BTree"), _behaviorTreeRunner.ModelTreeSlot, typeof(BehaviorTree), false) as BehaviorTree;
            }
            else
            {
                GUIHelper.PushGUIEnabled(false);
                EditorGUILayout.ObjectField(new GUIContent(), _behaviorTreeRunner.RunTree, typeof(BehaviorTreeRunner), true);
                GUIHelper.PopGUIEnabled();
            }
            if (serializedObject.FindProperty("ModelTreeSlot").objectReferenceValue != null)
            {
                SirenixEditorGUI.BeginInlineBox();
                SirenixEditorGUI.BeginBoxHeader();
                _foldout = SirenixEditorGUI.Foldout(_foldout, "Variables");
                SirenixEditorGUI.EndBoxHeader();
                if (SirenixEditorGUI.BeginFadeGroup(this, _foldout))
                {
                    var local = Tree.GetPropertyAtPath("Variables");
                    for (int i = 0; i < local.Children.Count; ++i)
                    {
                        var child = local.Children[i];
                        var rect = EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(child.Children.Get("Key").ValueEntry.WeakSmartValue as string, GUILayout.MaxWidth(50));
                        EditorGUILayout.BeginVertical();
                        child.Children.Get("Value").Draw(GUIContent.none);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                    }
                }
                SirenixEditorGUI.EndFadeGroup();
                SirenixEditorGUI.EndInlineBox();
            }
        }
    }
}
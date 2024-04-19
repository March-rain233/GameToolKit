using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace GameToolKit.Editor
{
    public class NodeField : GraphElementField
    {

        /// <summary>
        /// 辅助类编辑器
        /// </summary>
        private class NodeEditor : Sirenix.OdinInspector.Editor.OdinEditor
        {
            InspectorHelper assist => target as InspectorHelper;
            bool foldout = true;
            public override void OnInspectorGUI()
            {
                SirenixEditorGUI.BeginBox() ;
                SirenixEditorGUI.BeginBoxHeader();
                foldout = SirenixEditorGUI.Foldout(foldout, (assist.InspectorData as BaseNode).Name);
                SirenixEditorGUI.EndBoxHeader();
                if (foldout)
                {
                    Tree.BeginDraw(true);
                    var property = Tree.GetPropertyAtPath("InspectorData");
                    var children = property.Children;
                    foreach (var child in children.Where(c => c.GetAttribute<HideInGraphInspectorAttribute>() == null))
                        child.Draw();
                    Tree.EndDraw();
                }
                SirenixEditorGUI.EndBox();
            }
        }

        /// <summary>
        /// 绑定的对象
        /// </summary>
        public BaseNode Instance;

        /// <summary>
        /// 创建指定对象的字段
        /// </summary>
        /// <param name="element">对象</param>
        /// <param name="Name">对象显示的名字</param>
        public NodeField(BaseNode element, string Name)
        {
            Instance = element;
            var assist = ScriptableObject.CreateInstance<InspectorHelper>();
            assist.InspectorData = element;
            var editor = UnityEditor.Editor.CreateEditor(assist, typeof(NodeEditor));
            var inspector = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
            //var inspector = new InspectorElement(editor);
            Add(inspector);
        }

        public override bool IsAssociatedWith(object obj)
        {
            return Instance.Equals(obj);
        }
    }
}

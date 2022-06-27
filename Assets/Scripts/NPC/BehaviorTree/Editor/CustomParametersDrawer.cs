using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Text.RegularExpressions;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// 自定义变量绘制
    /// </summary>
    [CustomPropertyDrawer(typeof(CustomParametersBase), true)]
    public class CustomParametersDrawer : PropertyDrawer
    {
        /// <summary>
        /// 当前字段使用的泛型参数
        /// </summary>
        private System.Type _type;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            
            //获取当前变量的类型
            //_type = property.FindPropertyRelative("_value").objectReferenceValue.GetType();

            //创建显示界面
            VisualElement root = new VisualElement();

            //PropertyField propertyField = new PropertyField(property.FindPropertyRelative("_value"));
            Toggle isFromBlackboard = new Toggle("123");
            //PopupField<string> indexPopup = new PopupField<string>();

            //root.Add(propertyField);
            root.Add(isFromBlackboard);
            //root.Add(indexPopup);

            ////绑定控件事件
            //isFromBlackboard.RegisterValueChangedCallback(value =>
            //{
            //    if (value.newValue) FromBlackBoard(property, root);
            //    FromLocal(property, root);
            //});
            //indexPopup.RegisterValueChangedCallback(value => SetIndex(property, value.newValue));

            ////界面初始化
            ////当索引为空时使用自身的对象值
            //if (string.IsNullOrEmpty(property.FindPropertyRelative("_index").stringValue))
            //{
            //    isFromBlackboard.value = true;
            //}
            //else
            //{
            //    isFromBlackboard.value = false;
            //}

            return root;
        }

    //    /// <summary>
    //    /// 获取黑板上的所有对应变量
    //    /// </summary>
    //    /// <returns></returns>
    //    private List<string> GetIndex(SerializedProperty property)
    //    {
    //        var indexList = new List<string>();
    //        var valType = property.FindPropertyRelative("_value")
    //            .objectReferenceValue.GetType();

    //        //获取节点绑定的黑板下定义的变量列表 
    //        (property.serializedObject.FindProperty("ModelContainer")
    //            .objectReferenceValue as INodeContainer).ModelBlackBoard
    //            .GetDefineList(valType).ForEach(index => indexList.Add("Local/" + index.Key));

    //        //获取全局变量列表
    //        if (GlobalVariables.Instance)
    //        {
    //            GlobalVariables.Instance.GetDefineList(valType).ForEach(index => indexList.Add("Global/" + index.Key));
    //        }

    //        return indexList;
    //    }
        
    //    /// <summary>
    //    /// 设置属性的索引
    //    /// </summary>
    //    /// <param name="property"></param>
    //    /// <param name="index"></param>
    //    private void SetIndex(SerializedProperty property, string index)
    //    {
    //        var prop = property.FindPropertyRelative("_index");
    //        if (string.IsNullOrEmpty(prop.stringValue))
    //        {
    //            //todo:索引变化时的事件绑定
    //            //(property.serializedObject.FindProperty("ModelContainer")
    //            //    .objectReferenceValue as INodeContainer).ModelBlackBoard.IndexChanged
    //            //    += (prop. as CustomParametersBase).IndexChangeHandler; 
    //        }
    //        property.FindPropertyRelative("_index").stringValue = index;
    //    }

    //    /// <summary>
    //    /// 当变量来自黑板时更新UI
    //    /// </summary>
    //    /// <param name="property"></param>
    //    private void FromBlackBoard(SerializedProperty property, VisualElement root)
    //    {
    //        var index = root.Q<PopupField<string>>();
    //        index.visible = true;
    //        root.Q<PropertyField>().visible = false;
    //        index.choices = GetIndex(property);

    //        //当预存有索引时，更新为缓存值
    //        if (!string.IsNullOrEmpty(index.text))
    //        {
    //            property.FindPropertyRelative("_index").stringValue = index.text;
    //        }
    //    }

    //    /// <summary>
    //    /// 当变量来自本地时更新UI
    //    /// </summary>
    //    /// <param name="property"></param>
    //    private void FromLocal(SerializedProperty property, VisualElement root)
    //    {
    //        root.Q<PopupField<string>>().visible = false;
    //        root.Q<PropertyField>().visible = true;

    //        //制空索引值
    //        property.FindPropertyRelative("_index").stringValue = null;
    //    }
    }
}

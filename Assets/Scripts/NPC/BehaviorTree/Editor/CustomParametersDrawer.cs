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
    /// �Զ����������
    /// </summary>
    [CustomPropertyDrawer(typeof(CustomParametersBase), true)]
    public class CustomParametersDrawer : PropertyDrawer
    {
        /// <summary>
        /// ��ǰ�ֶ�ʹ�õķ��Ͳ���
        /// </summary>
        private System.Type _type;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            
            //��ȡ��ǰ����������
            //_type = property.FindPropertyRelative("_value").objectReferenceValue.GetType();

            //������ʾ����
            VisualElement root = new VisualElement();

            //PropertyField propertyField = new PropertyField(property.FindPropertyRelative("_value"));
            Toggle isFromBlackboard = new Toggle("123");
            //PopupField<string> indexPopup = new PopupField<string>();

            //root.Add(propertyField);
            root.Add(isFromBlackboard);
            //root.Add(indexPopup);

            ////�󶨿ؼ��¼�
            //isFromBlackboard.RegisterValueChangedCallback(value =>
            //{
            //    if (value.newValue) FromBlackBoard(property, root);
            //    FromLocal(property, root);
            //});
            //indexPopup.RegisterValueChangedCallback(value => SetIndex(property, value.newValue));

            ////�����ʼ��
            ////������Ϊ��ʱʹ������Ķ���ֵ
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
    //    /// ��ȡ�ڰ��ϵ����ж�Ӧ����
    //    /// </summary>
    //    /// <returns></returns>
    //    private List<string> GetIndex(SerializedProperty property)
    //    {
    //        var indexList = new List<string>();
    //        var valType = property.FindPropertyRelative("_value")
    //            .objectReferenceValue.GetType();

    //        //��ȡ�ڵ�󶨵ĺڰ��¶���ı����б� 
    //        (property.serializedObject.FindProperty("ModelContainer")
    //            .objectReferenceValue as INodeContainer).ModelBlackBoard
    //            .GetDefineList(valType).ForEach(index => indexList.Add("Local/" + index.Key));

    //        //��ȡȫ�ֱ����б�
    //        if (GlobalVariables.Instance)
    //        {
    //            GlobalVariables.Instance.GetDefineList(valType).ForEach(index => indexList.Add("Global/" + index.Key));
    //        }

    //        return indexList;
    //    }
        
    //    /// <summary>
    //    /// �������Ե�����
    //    /// </summary>
    //    /// <param name="property"></param>
    //    /// <param name="index"></param>
    //    private void SetIndex(SerializedProperty property, string index)
    //    {
    //        var prop = property.FindPropertyRelative("_index");
    //        if (string.IsNullOrEmpty(prop.stringValue))
    //        {
    //            //todo:�����仯ʱ���¼���
    //            //(property.serializedObject.FindProperty("ModelContainer")
    //            //    .objectReferenceValue as INodeContainer).ModelBlackBoard.IndexChanged
    //            //    += (prop. as CustomParametersBase).IndexChangeHandler; 
    //        }
    //        property.FindPropertyRelative("_index").stringValue = index;
    //    }

    //    /// <summary>
    //    /// ���������Ժڰ�ʱ����UI
    //    /// </summary>
    //    /// <param name="property"></param>
    //    private void FromBlackBoard(SerializedProperty property, VisualElement root)
    //    {
    //        var index = root.Q<PopupField<string>>();
    //        index.visible = true;
    //        root.Q<PropertyField>().visible = false;
    //        index.choices = GetIndex(property);

    //        //��Ԥ��������ʱ������Ϊ����ֵ
    //        if (!string.IsNullOrEmpty(index.text))
    //        {
    //            property.FindPropertyRelative("_index").stringValue = index.text;
    //        }
    //    }

    //    /// <summary>
    //    /// ���������Ա���ʱ����UI
    //    /// </summary>
    //    /// <param name="property"></param>
    //    private void FromLocal(SerializedProperty property, VisualElement root)
    //    {
    //        root.Q<PopupField<string>>().visible = false;
    //        root.Q<PropertyField>().visible = true;

    //        //�ƿ�����ֵ
    //        property.FindPropertyRelative("_index").stringValue = null;
    //    }
    }
}

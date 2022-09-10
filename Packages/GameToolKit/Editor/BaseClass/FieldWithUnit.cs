using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameToolKit.Editor {
    /// <summary>
    /// 带有单位的浮点数字段
    /// </summary>
    public class FieldWithUnit : VisualElement
    {
        public FloatField Field;
        public DropdownField Unit;

        public List<string> Choice
        {
            get => Unit.choices;
            set => Unit.choices = value;
        }

        public float Value
        {
            get => Field.value;
            set => Field.value = value;
        }

        public string UnitValue
        {
            get => Unit.value;
            set => Unit.value = value;
        }

        public string Label
        {
            get => Field.label;
            set => Field.label = value;
        }
        public FieldWithUnit()
        {
            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/BaseClass/UXML/FieldWithUnit.uxml");
            visualTreeAsset.CloneTree(this);
            Unit = this.Q<DropdownField>("unit");
            Field = this.Q<FloatField>("field");
        }
    }
}

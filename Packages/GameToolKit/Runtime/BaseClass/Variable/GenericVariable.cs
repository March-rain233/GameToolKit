using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace GameToolKit
{
    public abstract class GenericVariable<T> : BlackboardVariable
    {
        static readonly Type _typeOfValue = typeof(T);

        [SerializeField]
        [OnValueChanged("OnValueChangedEditor", true)]
        T _value;

        public override object Value 
        { 
            get => _value;
            set => SetValue((T)value);
        }
        public override Type TypeOfValue => _typeOfValue;

        /// <summary>
        /// 获取值
        /// </summary>
        /// <remarks>避免拆装箱</remarks>
        /// <returns></returns>
        public T GetValue() => _value;

        /// <summary>
        /// 设置值
        /// </summary>
        /// <remarks>避免拆装箱</remarks>
        /// <returns></returns>
        public void SetValue(T value)
        {
            if (IsReadOnly)
            {
                Debug.LogError("An attempt was made to assign a value to a variable, but it is read-only");
                return;
            }
            _value = value;
            InvokeValueChanged();
        }

#if UNITY_EDITOR
        T _last;
        private void OnValueChangedEditor()
        {
            InvokeValueChanged();
            _last = _value;
        }
#endif
    }
    public class ObjectVariable : GenericVariable<object> { }
    public class StringVariable : GenericVariable<string> { }
    public class BooleanVariable : GenericVariable<bool> { }
    public class DoubleVarialbe : GenericVariable<double> { }
    public class FloatVarialbe : GenericVariable<float> { }
    public class IntVarialbe : GenericVariable<int> { }
    public class Vector2Variable : GenericVariable<Vector2> { }
    public class Vector3Variable : GenericVariable<Vector3> { }
    public class Vector4Variable : GenericVariable<Vector4> { }
    public class ColorVariable : GenericVariable<Color> { }
    public class GameObjectVariable : GenericVariable<GameObject> { }
    public class ScriptableObjectVariable : GenericVariable<ScriptableObject> { }
    public class ComponentVariable : GenericVariable<Component> { }
    public class RectVariable : GenericVariable<Rect> { }
    public class BoundsVariable : GenericVariable<Bounds> { }
    public class CurveVariable : GenericVariable<AnimationCurve> { }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameFrame
{
    public abstract class GenericVariable<T> : BlackboardVariable
    {
        [SerializeField]
        T _value;
        public override object Value { get => _value; set => _value = (T)value; }
        public override BlackboardVariable Clone()
        {
            var obj = this.MemberwiseClone() as GenericVariable<T>;
            obj._value = _value;
            return obj;
        }
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

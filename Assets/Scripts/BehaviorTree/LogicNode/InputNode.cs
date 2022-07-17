using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// �ֲ������ڵ�
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class InputNode<T> : SourceNode
    {
        [Port("Output", Direction.Output)]
        [SerializeField]
        [HideInInspector]
        protected T _value = default;
        [OdinSerialize]
        public T Value
        {
            get { return _value; }
            set 
            { 
                _value = value;
                InitOutputData();
            }
        }
        protected override void OnValueUpdate()
        {
            
        }
    }
    public class IntegerNode : InputNode<int> { }
    public class FloatNode : InputNode<float> { }
    public class DoubleNode : InputNode<double> { }
    public class StringNode : InputNode<string> { }
    public class BooleanNode : InputNode<bool> { }
    public class ObjectNode : InputNode<object> { }
    public class Vector2Node : InputNode<Vector2> { }
    public class Vector3Node : InputNode<Vector3> { }
    public class Vector4Node : InputNode<Vector4> { } 
    public class QuaternionNode : InputNode<Quaternion> { }
    public class RectNode : InputNode<Rect> { }
    public class BoundsNode : InputNode<Bounds> { }
    public class ColorNode : InputNode<Color> { }
    public class GameObjectNode : InputNode<GameObject> { }
    public class ComponentNode : InputNode<Component> { }
    public class CurveNode : InputNode<AnimationCurve> { }
}
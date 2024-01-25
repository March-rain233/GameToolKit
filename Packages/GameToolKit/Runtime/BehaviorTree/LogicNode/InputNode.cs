using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
namespace GameToolKit.Behavior.Tree
{
    /// <summary>
    /// 局部变量节点
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class InputNode<TValue> : SourceNode
    {
        [Port("Output", PortDirection.Output)]
        [SerializeField]
        [HideInInspector]
        protected TValue _value = default;
        [OdinSerialize]
        public TValue Value
        {
            get { return _value; }
            set 
            { 
                _value = value;
                if (!HasInitialized) return;
                InitOutputData();
            }
        }

        protected override void OnValueUpdate()
        {
            
        }

        protected override object GetValue(string name) =>
            _value;
    }
    public sealed class IntegerNode : InputNode<int> { }
    public sealed class FloatNode : InputNode<float> { }
    public sealed class DoubleNode : InputNode<double> { }
    public sealed class StringNode : InputNode<string> { }
    public sealed class BooleanNode : InputNode<bool> { }
    public sealed class ObjectNode : InputNode<object> { }
    public sealed class Vector2Node : InputNode<Vector2> { }
    public sealed class Vector3Node : InputNode<Vector3> { }
    public sealed class Vector4Node : InputNode<Vector4> { } 
    public sealed class QuaternionNode : InputNode<Quaternion> { }
    public sealed class RectNode : InputNode<Rect> { }
    public sealed class BoundsNode : InputNode<Bounds> { }
    public sealed class ColorNode : InputNode<Color> { }
    public sealed class GameObjectNode : InputNode<GameObject> { }
    public sealed class ComponentNode : InputNode<Component> { }
    public sealed class CurveNode : InputNode<AnimationCurve> { }
}

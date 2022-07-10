using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// 局部变量节点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [NodeCategory("Logic/Input")]
    public abstract class InputNode<T> : LogicNode
    {
        [Port("Output", PortType.Output)]
        public T Value = default;
        protected override void OnUpdate()
        {
            
        }
    }
    public class ObjectNode:InputNode<object> { }
    public class StringNode:InputNode<string> { }
    public class BooleanNode:InputNode<bool> { }
    public class FloatNode:InputNode<float> { }
    public class DoubleNode:InputNode<double> { }
    public class IntegerNode:InputNode<int> { }
    public class Vector2Node:InputNode<Vector2> { }
    public class Vector3Node:InputNode<Vector3> { }
    public class Vector4Node:InputNode<Vector4> { } 
    public class QuaternionNode:InputNode<Quaternion> { }
    public class ColorNode:InputNode<Color> { }
    public class GameObjectNode:InputNode<GameObject> { }
    public class ComponentNode:InputNode<Component> { }
    public class RectNode:InputNode<Rect> { }
    public class BoundsNode:InputNode<Bounds> { }
    public class CurveNode : InputNode<AnimationCurve> { }
}

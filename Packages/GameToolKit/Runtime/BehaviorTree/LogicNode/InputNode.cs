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
    [NodeCategory("Logic/Source/Input")]
    [GenericSelector("BaseValue")]
    public class InputNode<TValue> : SourceNode
    {
        [SourcePort("Output", PortDirection.Output)]
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
                SetDirty();
            }
        }

        protected override void OnValueUpdate()
        {
            SetDirty();
        }

        protected override object GetValue(string name) =>
            _value;
    }
}

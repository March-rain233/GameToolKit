using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Behavior.Tree {
    public class AddNode : LogicNode
    {
        [SourcePort("InputA", PortDirection.Input, new System.Type[]{ typeof(int), typeof(float) })]
        public double InputA;
        [SourcePort("InputB", PortDirection.Input, new System.Type[] { typeof(int), typeof(float) })]
        public double InputB;
        [SourcePort("Result", PortDirection.Output, new System.Type[] { typeof(int), typeof(float) })]
        public double Result;
        protected override void OnValueUpdate()
        {
            Result = InputA + InputB;
            SetDirty();
        }

        protected override object GetValue(string name)
        {
            return Result;
        }
    }
}

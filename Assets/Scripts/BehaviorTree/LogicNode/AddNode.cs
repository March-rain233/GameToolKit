using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree {
    public class AddNode : LogicNode
    {
        [Port("InputA", Direction.Input, new System.Type[]{ typeof(int), typeof(float) })]
        public double InputA;
        [Port("InputB", Direction.Input, new System.Type[] { typeof(int), typeof(float) })]
        public double InputB;
        [Port("Result", Direction.Output, new System.Type[] { typeof(int), typeof(float) })]
        public double Result;
        protected override void OnValueUpdate()
        {
            Result = InputA + InputB;
        }
    }
}

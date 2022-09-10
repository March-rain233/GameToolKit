using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Behavior.Tree
{
    public abstract class ConditionNode : Leaf
    {
        public bool Invert = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameFrame.Behavior.Tree
{
    [NodeCategory("Logic/Input")]
    public abstract class SourceNode : LogicNode
    {
        protected override void OnInit()
        {
            base.OnInit();
            InitOutputData();
        }
    }
}
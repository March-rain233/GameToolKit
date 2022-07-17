using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// 连接输入的两个字符串
    /// </summary>
    public class StringConcatNode : LogicNode
    {
        [Port("Input1", Direction.Input)]
        public string Input1;
        [Port("Input2", Direction.Input)]
        public string Input2;
        [Port("Result", Direction.Output)]
        public string Result;
        protected override void OnValueUpdate()
        {
            Result = Input1 + Input2;
        }
    }
}
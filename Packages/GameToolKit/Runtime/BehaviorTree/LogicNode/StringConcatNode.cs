using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Behavior.Tree
{
    /// <summary>
    /// 连接输入的两个字符串
    /// </summary>
    public class StringConcatNode : LogicNode
    {
        [SourcePort("Input1", PortDirection.Input)]
        public string Input1;
        [SourcePort("Input2", PortDirection.Input)]
        public string Input2;
        [SourcePort("Result", PortDirection.Output)]
        public string Result;
        protected override void OnValueUpdate()
        {
            Result = Input1 + Input2;
            SetDirty();
        }

        protected override object GetValue(string name)
        {
            return Result;
        }
    }
}
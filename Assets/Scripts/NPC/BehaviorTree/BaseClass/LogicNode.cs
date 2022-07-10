using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree {

    /// <summary>
    /// 逻辑节点
    /// </summary>
    /// <remarks>
    /// 负责数据运算
    /// </remarks>
    [Node("#81ecec", NodeAttribute.PortType.None, NodeAttribute.PortType.None)]
    [NodeCategory("Logic")]
    public abstract class LogicNode : BaseNode
    {
        protected override object GetValue(string fieldInfo)
        {
            if (_lastUpdataTime != Time.time)
            {
                Init();
                OnUpdate();
                _lastUpdataTime = Time.time;
            }
            var type = GetType();
            do
            {
                var field = GetType().GetField(fieldInfo);
                if (field != null)
                {
                    return field.GetValue(this);
                }
            } while (type != typeof(BaseNode));
            return null;
        }
        /// <summary>
        /// 当更新端口数据
        /// </summary>
        protected abstract void OnUpdate();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree {

    /// <summary>
    /// �߼��ڵ�
    /// </summary>
    /// <remarks>
    /// ������������
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
        /// �����¶˿�����
        /// </summary>
        protected abstract void OnUpdate();
    }
}

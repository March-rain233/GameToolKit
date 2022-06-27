using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// ѡ��ڵ�
    /// </summary>
    public class SelectorNode : CompositeNode
    {
        /// <summary>
        /// ��ǰ���нڵ�
        /// </summary>
        private int _current;

        protected override void OnEnter()
        {
            _current = 0;
        }

        protected override NodeStatus OnUpdate()
        {
            //�����
            for (int i = 0; i < _current; ++i)
            {
                //����ýڵ���������������ȶ������ڵ㣬�����ڷ��سɹ���ֱ�Ӵ��
                if (AbortType == AbortType.Self || AbortType == AbortType.Both)
                {
                    var condition = Childrens[i] as ConditionNode;
                    if (condition && condition.Tick() == NodeStatus.Success)
                    {
                        Childrens[_current].Abort();
                        return NodeStatus.Aborting;
                    }
                    continue;
                }
                //����ӽ�Ͻڵ����ҷ��������ڷ��سɹ���ֱ�Ӵ��
                var composite = Childrens[i] as CompositeNode;
                if (composite && (composite.AbortType == AbortType.LowerPriority
                    || composite.AbortType == AbortType.Both))
                {
                    var s = composite.Tick();
                    if (s == NodeStatus.Running || s == NodeStatus.Success)
                    {
                        Childrens[_current].Abort();
                        _current = i;
                        return s;
                    }
                    continue;
                }
            }

            NodeStatus status;
            do
            {
                status = Childrens[_current].Tick();
            }
            while (status == NodeStatus.Failure && ++_current < Childrens.Count);
            return status;
        }
    }
}
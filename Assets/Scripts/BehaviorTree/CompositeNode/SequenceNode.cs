using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// ˳��ڵ�
    /// </summary>
    public class SequenceNode : CompositeNode
    {
        /// <summary>
        /// ��ǰ���нڵ�
        /// </summary>
        [SerializeField]
        private int _current = 0;

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
                if ((AbortType & AbortType.Self) != 0)
                {
                    var condition = Childrens[i] as ConditionNode;
                    if (condition != null && condition.Tick() == NodeStatus.Failure)
                    {
                        Childrens[_current].Abort();
                        return NodeStatus.Aborting;
                    }
                    continue;
                }
                //����ӽ�Ͻڵ����ҷ��������ڷ��سɹ���ֱ�Ӵ��
                var composite = Childrens[i] as CompositeNode;
                if (composite != null && (composite.AbortType & AbortType.LowerPriority) != 0)
                {
                    var s = composite.Tick();
                    if (s == NodeStatus.Running || s == NodeStatus.Failure)
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
            while (status == NodeStatus.Success && ++_current < Childrens.Count);
            return status;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// 选择节点
    /// </summary>
    public class SelectorNode : CompositeNode
    {
        /// <summary>
        /// 当前运行节点
        /// </summary>
        private int _current;

        protected override void OnEnter()
        {
            _current = 0;
        }

        protected override NodeStatus OnUpdate()
        {
            //检查打断
            for (int i = 0; i < _current; ++i)
            {
                //如果该节点打断自身则检测高优先度条件节点，若存在返回成功则直接打断
                if (AbortType == AbortType.Self || AbortType == AbortType.Both)
                {
                    var condition = Childrens[i] as ConditionNode;
                    if (condition != null && condition.Tick() == NodeStatus.Success)
                    {
                        Childrens[_current].Abort();
                        return NodeStatus.Aborting;
                    }
                    continue;
                }
                //如果子结合节点打断右方则，若存在返回成功则直接打断
                var composite = Childrens[i] as CompositeNode;
                if (composite != null && (composite.AbortType == AbortType.LowerPriority
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
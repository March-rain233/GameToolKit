using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// ȡ����
    /// </summary>
    public class InverterNode : DecoratorNode
    {
        protected override NodeStatus OnUpdate()
        {
            switch (Child.Tick())
            {
                case NodeStatus.Success:
                    return NodeStatus.Failure;
                case NodeStatus.Failure:
                    return NodeStatus.Success;
                case NodeStatus.Running:
                    break;
            }

            throw new System.Exception("������δ֪�Ĵ���");
        }
    }
}

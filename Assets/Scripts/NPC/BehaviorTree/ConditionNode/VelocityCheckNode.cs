using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// ����ٶ��Ƿ񳬳�ָ���ٶ�
    /// </summary>
    public class VelocityCheckNode : ConditionNode
    {
        /// <summary>
        /// ���ı�׼�ٶ�
        /// </summary>
        [SerializeField]
        private Vector2 _maxVelocity;

        /// <summary>
        /// ����X��
        /// </summary>
        [SerializeField]
        private bool _ignoreX;
        /// <summary>
        /// ����Y��
        /// </summary>
        [SerializeField]
        private bool _ignoreY;

        protected override NodeStatus OnUpdate()
        {
            //var rg = runner.Variables["Rigidbody"].Object as Rigidbody2D;
            //var uv = rg.velocity;
            //var mv = _maxVelocity;
            //if (!_ignoreY && mv.y < 0)
            //{
            //    mv.y = -mv.y;
            //    uv.y = -uv.y;
            //}
            //if (!_ignoreX && mv.x < 0)
            //{
            //    mv.x = -mv.x;
            //    uv.x = -uv.x;
            //}

            //if (Invert ^ ((_ignoreY || uv.y <= mv.y) && (_ignoreX || uv.x <= mv.x))) { return NodeStatus.Failure; }
            return NodeStatus.Success;
        }
    }
}

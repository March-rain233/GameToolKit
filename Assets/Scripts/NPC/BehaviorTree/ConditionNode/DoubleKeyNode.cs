using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// �жϼ���˫��
    /// </summary>
    public class DoubleKeyNode : ConditionNode
    {
        /// <summary>
        /// ���İ�������
        /// </summary>
        [SerializeField]
        private KeyType _keyType;

        /// <summary>
        /// �ж����
        /// </summary>
        [SerializeField]
        private float _judgeInterval;

        /// <summary>
        /// ��һ�ΰ�ѹʱ��
        /// </summary>
        private float _lastPressTime = 0;

        protected override NodeStatus OnUpdate()
        {
            if (UnityEngine.Input.GetKeyDown(GameManager.Instance.ControlManager.KeyDic[_keyType]))
            {
                if(Invert ^ Time.time - _lastPressTime <= _judgeInterval)
                {
                    _lastPressTime = Time.time;
                    return NodeStatus.Success;
                }
                _lastPressTime = Time.time;
            }
            return NodeStatus.Failure;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// ��ⰴ��
    /// </summary>
    public class KeyNode : ConditionNode
    {
        /// <summary>
        /// ��ѹ����
        /// </summary>
        [System.Serializable]
        private enum PressType
        {
            Down,
            Pressing,
            Up
        }

        /// <summary>
        /// ���İ�������
        /// </summary>
        [SerializeField]
        private KeyType _keyType;

        /// <summary>
        /// ���İ�ѹ����
        /// </summary>
        [SerializeField]
        private PressType _pressType;

        protected override NodeStatus OnUpdate()
        {
            switch (_pressType)
            {
                case PressType.Down:
                    if (Invert ^ UnityEngine.Input.GetKeyDown(GameManager.Instance.ControlManager.KeyDic[_keyType])) { return NodeStatus.Success; }
                    return NodeStatus.Failure;
                case PressType.Pressing:
                    if (Invert ^ UnityEngine.Input.GetKey(GameManager.Instance.ControlManager.KeyDic[_keyType])) { return NodeStatus.Success; }
                    return NodeStatus.Failure;
                case PressType.Up:
                    if (Invert ^ UnityEngine.Input.GetKeyUp(GameManager.Instance.ControlManager.KeyDic[_keyType])) { return NodeStatus.Success; }
                    return NodeStatus.Failure;
            }
            throw new System.Exception("It should never hapeen?");
        }
    }
}

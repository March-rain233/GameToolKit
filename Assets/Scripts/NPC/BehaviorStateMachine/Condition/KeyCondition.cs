//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace NPC
//{
//    [CreateAssetMenu(fileName = "��������", menuName = "��ɫ/����/��������")]
//    public class KeyCondition : BaseCondition
//    {
//        /// <summary>
//        /// ��ѹ����
//        /// </summary>
//        [System.Serializable]
//        private enum PressType
//        {
//            Down,
//            Pressing,
//            Up
//        }

//        /// <summary>
//        /// ���İ�������
//        /// </summary>
//        [SerializeField]
//        private KeyType _keyType;

//        /// <summary>
//        /// ���İ�ѹ����
//        /// </summary>
//        [SerializeField]
//        private PressType _pressType;

//        public override bool Reason(BehaviorStateMachine user)
//        {
//            switch (_pressType)
//            {
//                case PressType.Down:
//                    return Input.GetKeyDown(GameManager.Instance.ControlManager.KeyDic[_keyType]);
//                case PressType.Pressing:
//                    return Input.GetKey(GameManager.Instance.ControlManager.KeyDic[_keyType]);
//                case PressType.Up:
//                    return Input.GetKeyUp(GameManager.Instance.ControlManager.KeyDic[_keyType]);
//                default:
//                    return true;
//            }
//        }
//    }
//}

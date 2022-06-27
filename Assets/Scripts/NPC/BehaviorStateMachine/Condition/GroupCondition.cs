//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace NPC
//{
//    [CreateAssetMenu(fileName = "������", menuName = "��ɫ/����/������")]
//    public class GroupCondition : BaseCondition
//    {
//        /// <summary>
//        /// �жϵ�Ԫ
//        /// </summary>
//        [System.Serializable]
//        private class JudgeCell
//        {
//            /// <summary>
//            /// λ��������
//            /// </summary>
//            [System.Serializable]
//            internal enum MergeType
//            {
//                AND,
//                OR,
//                XOR,
//            }
//            /// <summary>
//            /// ��ǰ�ڵ�Ĺ��ɵ�Ԫ
//            /// </summary>
//            public BaseCondition Transition;
//            /// <summary>
//            /// �Ƿ�ȡ��
//            /// </summary>
//            public bool Inverse;
//            /// <summary>
//            /// ����һ�ڵ�����㷽ʽ
//            /// </summary>
//            public MergeType Type;
//        }

//        /// <summary>
//        /// ��ǰ������ı��ʽ
//        /// </summary>
//        [SerializeField]
//        private List<JudgeCell> _expression;

//        /// <summary>
//        /// �Ƿ�ȡ��
//        /// </summary>
//        public bool Inverse;
//        public override bool Reason(BehaviorStateMachine user)
//        {
//            bool value;
//            if(_expression.Count <= 0) { return Inverse; }

//            value = _expression[0].Inverse ^ _expression[0].Transition.Reason(user);
//            for(int i = 1; i < _expression.Count; ++i)
//            {
//                switch (_expression[i].Type)
//                {
//                    case JudgeCell.MergeType.AND:
//                        value &= (_expression[i].Inverse ^ _expression[i].Transition.Reason(user));
//                        break;
//                    case JudgeCell.MergeType.OR:
//                        value |= (_expression[i].Inverse ^ _expression[i].Transition.Reason(user));
//                        break;
//                    case JudgeCell.MergeType.XOR:
//                        value ^= (_expression[i].Inverse ^ _expression[i].Transition.Reason(user));
//                        break;
//                }
//            }
//            return value;
//        }
//    }
//}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace NPC
//{
//    /// <summary>
//    /// ����
//    /// </summary>
//    public class Transition : ScriptableObject
//    {
//        [System.Serializable]
//        public struct ConditionInfo 
//        {
//            public bool Inverse;
//            public BaseCondition Condition;
//        }

//        /// <summary>
//        /// ��ʼ״̬
//        /// </summary>
//        public BaseState StartState;

//        /// <summary>
//        /// Ŀ��״̬
//        /// </summary>
//        public BaseState EndState;

//        /// <summary>
//        /// ����
//        /// </summary>
//        public List<ConditionInfo> Conditions;

//        /// <summary>
//        /// �ж�
//        /// </summary>
//        /// <param name="user"></param>
//        /// <returns></returns>
//        public bool Reason(BehaviorStateMachine user)
//        {
//            bool value = true;
//            foreach(var condition in Conditions)
//            {
//                value &= condition.Inverse ^ condition.Condition.Reason(user);
//            }
//            return value;
//        }
//    }
//}

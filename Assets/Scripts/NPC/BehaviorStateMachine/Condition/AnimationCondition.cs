//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace NPC
//{
//    [CreateAssetMenu(fileName = "����ʱ������", menuName = "��ɫ/����/����ʱ������")]
//    public class AnimationCondition : BaseCondition
//    {
//        /// <summary>
//        /// ���Ķ���״̬������
//        /// </summary>
//        [SerializeField]
//        private string _animationName;

//        /// <summary>
//        /// ���Ķ���״̬�Ĳ��Ž���
//        /// </summary>
//        [SerializeField]
//        private float _animationTime;

//        public override bool Reason(BehaviorStateMachine user)
//        {
//            var info = user.Animator.GetCurrentAnimatorStateInfo(0);
//            if(info.IsName(_animationName) && info.normalizedTime >= _animationTime)
//            {
//                return true;
//            }
//            return false;
//        }
//    }
//}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace NPC
//{
//    [CreateAssetMenu(fileName = "�������״̬", menuName = "��ɫ/״̬/�������")]
//    public class MindHold : BaseState
//    {
//        /// <summary>
//        /// �����Ծ����
//        /// </summary>
//        [SerializeField]
//        private float _maxJumpScale;

//        /// <summary>
//        /// ��С��Ծ����
//        /// </summary>
//        [SerializeField]
//        private float _minJumpScale;

//        /// <summary>
//        /// ���������Чʱ��
//        /// </summary>
//        [SerializeField]
//        private float _holdingTime;

//        public override void OnEnter(BehaviorStateMachine user)
//        {
//            base.OnEnter(user);
//            user.Animator.SetBool("IsHold", true);
//            user.Model.RigidBody.velocity = new Vector2(0, user.Model.RigidBody.velocity.y);
//        }

//        public override void OnExit(BehaviorStateMachine user)
//        {
//            base.OnExit(user);
//            user.Animator.SetBool("IsHold", false);

//            float rate = Mathf.Clamp(user.RunTime, 0, _holdingTime);
//            rate = rate / _holdingTime;
//            rate = Mathf.Lerp(_minJumpScale, _maxJumpScale, rate);

//            user.Model.RigidBody.AddForce(Vector2.up * rate * user.Model.JumpHeight);
//        }

//        public override void OnUpdate(BehaviorStateMachine user)
//        {
            
//        }
//    }
//}

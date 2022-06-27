//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace NPC
//{
//    [CreateAssetMenu(fileName = "�ٶ�����", menuName = "��ɫ/����/��������")]
//    public class VelocityCondition : BaseCondition
//    {
//        /// <summary>
//        /// ���ı�׼�ٶ�
//        /// </summary>
//        [SerializeField]
//        private Vector2 _maxVelocity;

//        /// <summary>
//        /// ����X��
//        /// </summary>
//        [SerializeField]
//        private bool _ignoreX;
//        /// <summary>
//        /// ����Y��
//        /// </summary>
//        [SerializeField]
//        private bool _ignoreY;

//        public override bool Reason(BehaviorStateMachine user)
//        {
//            var uv = user.Model.RigidBody.velocity;
//            var mv = _maxVelocity;
//            if(!_ignoreY && mv.y < 0) 
//            {
//                mv.y = -mv.y;
//                uv.y = -uv.y;
//            }
//            if(!_ignoreX && mv.x < 0)
//            {
//                mv.x = -mv.x;
//                uv.x = -uv.x;
//            }

//            if((_ignoreY || uv.y<=mv.y) && (_ignoreX || uv.x <= mv.x)) { return false; }
//            return true;
//        }
//    }
//}

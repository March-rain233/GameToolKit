//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace NPC
//{
//    [CreateAssetMenu(fileName = "����ֵ�ٶ�����", menuName = "��ɫ/����/����ֵ�ٶ�����")]
//    public class AbsVelocityCondition : BaseCondition
//    {
//        /// <summary>
//        /// ���ı�׼�ٶ�
//        /// </summary>
//        private Vector2 MaxVelocity
//        {
//            get { return _maxVelocity; }
//            set
//            {
//                _maxVelocity.x = Mathf.Abs(value.x);
//                _maxVelocity.y = Mathf.Abs(value.y);
//            }
//        }
//        [SerializeField, SetProperty("MaxVelocity")]
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
//            var temp = user.Model.RigidBody.velocity;
//            if ((_ignoreY || temp.y <= _maxVelocity.y && temp.y >= -_maxVelocity.y)
//                && (_ignoreX || temp.x <= _maxVelocity.x && temp.x >= -_maxVelocity.x))
//            {
//                return false;
//            }
//            return true;
//        }
//    }
//}

//using UnityEngine;

//namespace NPC
//{
//    [CreateAssetMenu(fileName = "�������״̬", menuName = "��ɫ/״̬/�������")]
//    public class MindRise : BaseState
//    {
//        /// <summary>
//        /// �ƶ�����
//        /// </summary>
//        [SerializeField]
//        private float _moveScale;

//        public override void OnEnter(BehaviorStateMachine user)
//        {
//            base.OnEnter(user);
//            user.Animator.SetBool("IsRise", true);
//        }

//        public override void OnExit(BehaviorStateMachine user)
//        {
//            base.OnExit(user);
//            user.Animator.SetBool("IsRise", false);
//        }

//        public override void OnUpdate(BehaviorStateMachine user)
//        {
//            var pair = GameManager.Instance.ControlManager.KeyDic;
//            //��-1 ��0 ��1
//            int horizontal =
//                System.Convert.ToInt32(UnityEngine.Input.GetKey(pair[KeyType.Right]))
//                - System.Convert.ToInt32(UnityEngine.Input.GetKey(pair[KeyType.Left]));

//            if (horizontal == 0) { return; }
//            if (horizontal == 1)
//            {
//                user.Model.FaceDir = 1;
//            }
//            else
//            {
//                user.Model.FaceDir = -1;
//            }
//            user.Model.RigidBody.velocity = new Vector2(horizontal * _moveScale * user.Model.Speed
//                , user.Model.RigidBody.velocity.y);
//        }
//    }
//}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Sirenix.OdinInspector;
//using Item;

//namespace NPC
//{
//    /// <summary>
//    /// ��ɫai������
//    /// </summary>
//    public class BehaviorStateMachine : MonoBehaviour, IHurt
//    {
//        /// <summary>
//        /// �󶨵�NPCģ��
//        /// </summary>
//        [SerializeField]
//        public NPC_Model Model
//        {
//            get
//            {
//                if(_model == null)
//                {
//                    _model = GetComponent<NPC_Model>();
//                }
//                return _model;
//            }
//        }
//        private NPC_Model _model;

//        /// <summary>
//        /// ʹ�õ�״̬�������ļ�
//        /// </summary>
//        [SerializeField, LabelText("������")]
//        private StateMachineController _controller;

//        public Animator Animator
//        {
//            get
//            {
//                return Model.Animator;
//            }
//        }

//        /// <summary>
//        /// ��ǰ״̬
//        /// </summary>
//        public int CurState
//        {
//            get
//            {
//                return _curState;
//            }
//            internal set
//            {
//                _curState = value;
//                RunTime = 0;
//            }
//        }
//        [SerializeField, SetProperty("CurState")]
//        private int _curState;

//        /// <summary>
//        /// ��ǰ״̬��ʼ���е�ʱ�䣨Ϊ��Ϸ����������ʱ�䣩
//        /// </summary>
//        public float StartTime
//        {
//            get
//            {
//                return _startTime;
//            }
//            set
//            {
//                _startTime = Time.time;
//                RunTime = 0;
//            }
//        }
//        private float _startTime;

//        /// <summary>
//        /// ��ǰ״̬���ܼ�����ʱ�䣨��������ͣ��
//        /// </summary>
//        public float RunTime
//        {
//            get
//            {
//                if (IsActive)
//                {
//                    RunTime = Time.time - _runTime.LastSetTime + _runTime.RunTime;
//                }
//                return _runTime.RunTime;
//            }
//            private set
//            {
//                _runTime.RunTime = value;
//                _runTime.LastSetTime = Time.time;
//            }
//        }
//        private RunTimer _runTime;

//        /// <summary>
//        /// ai�Ƿ�������
//        /// </summary>
//        public bool IsActive
//        {
//            get
//            {
//                return _isActive;
//            }
//            set
//            {
//                if(!value)
//                {
//                    //������ʱ��������ʱ�䣬��ֹͣ�ۼ�
//                    RunTime = RunTime;
//                }
//                else if (value ^ _isActive)
//                {
//                    //���ӽ���תΪ����ʱ��������ʱ����޸�ʱ������
//                    _runTime.LastSetTime = Time.time;
//                }
//                _isActive = value;
//            }
//        }
//        private bool _isActive = true;

//        public virtual void Init(StateMachineController control)
//        {
//            _controller = control;
//            _curState = -1;
//            StartTime = RunTime = 0;
//        }

//        protected virtual void Start()
//        {
//            _controller.OnEnter(this);   
//        }

//        /// <remarks>
//        /// �Ƚ���״̬���ĸ��£��ٴ�������
//        /// </remarks>
//        void Update()
//        {
//            if (!IsActive)
//            {
//                return;
//            }
//            _controller.OnUpdate(this);
//        }

//        public void Hurt(float hurt, Status status = Status.Common)
//        {
//            _controller.Hurt(this, hurt, status);
//        }

//        private struct RunTimer
//        {
//            public float RunTime;
//            public float LastSetTime;
//        }
//    }
//}

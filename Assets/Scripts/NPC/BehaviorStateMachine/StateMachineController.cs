//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Sirenix.OdinInspector;
//using Item;
//using UnityEditor;

//namespace NPC
//{
//    /// <summary>
//    /// AI����
//    /// </summary>
//    /// <remarks>
//    /// ִ��NPC����Ϊ�߼���ʵ������״̬��
//    /// </remarks>
//    [CreateAssetMenu(fileName = "��Ϊ������", menuName = "��ɫ/��ɫ��Ϊ������")]
//    public class StateMachineController : SerializedScriptableObject, IProduct
//    {
//        /// <summary>
//        /// ״̬�б�
//        /// </summary>
//        [SerializeField]
//        public List<BaseState> States;


//        /// <summary>
//        /// �κ�״̬����
//        /// </summary>
//        [SerializeField]
//        public List<Transition> AnyState;

//        /// <summary>
//        /// �����ʼ״̬
//        /// </summary>
//        [SerializeField]
//        public int EnterState;

//        /// <summary>
//        /// ɾ��״̬
//        /// </summary>
//        /// <param name="state"></param>
//        public void DeleteState(BaseState state)
//        {
//            States.Remove(state);
//#if UNITY_EDITOR
//            state.Transitions.RemoveAll(t => 
//            { 
//                AssetDatabase.RemoveObjectFromAsset(t);
//                return true; 
//            });
//            States.ForEach(s => s.Transitions.RemoveAll(t =>
//            {
//                if(t.EndState == state)
//                {
//                    AssetDatabase.RemoveObjectFromAsset(t);
//                    return true;
//                }
//                return false;
//            }));

//            AssetDatabase.RemoveObjectFromAsset(state);
//            AssetDatabase.SaveAssets();
//#endif
//        }

//        /// <summary>
//        /// ����״̬
//        /// </summary>
//        /// <param name="type"></param>
//        /// <returns></returns>
//        public BaseState CreateState(System.Type type)
//        {
//            var state = CreateInstance(type) as BaseState;
//            state.name = type.Name;
//            States.Add(state);
//#if UNITY_EDITOR
//            state.Guid = GUID.Generate().ToString();

//            AssetDatabase.AddObjectToAsset(state, this);
//            AssetDatabase.SaveAssets();
//#endif
//            return state;
//        }

//        /// <summary>
//        /// ���ӹ���
//        /// </summary>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        public void AddTransition(BaseState start, BaseState end)
//        {
//            var transition = CreateInstance<Transition>();
//            transition.name = start.name + "To" + end.name;
//            transition.StartState = start;
//            transition.EndState = end;
//            start.Transitions.Add(transition);
//#if UNITY_EDITOR
//            AssetDatabase.AddObjectToAsset(transition, start);
//            AssetDatabase.SaveAssets();
//#endif
//        }

//        /// <summary>
//        /// �Ƴ�����
//        /// </summary>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        public void RemoveTransition(BaseState start, BaseState end)
//        {
//            Transition transition = start.Transitions.Find((t) => t.EndState == end);
//            start.Transitions.Remove(transition);

//#if UNITY_EDITOR
//            AssetDatabase.RemoveObjectFromAsset(transition);
//            AssetDatabase.SaveAssets();
//#endif
//        }

//        /// <summary>
//        /// ��ʼִ��״̬��
//        /// </summary>
//        /// <param name="user"></param>
//        public void OnEnter(BehaviorStateMachine user)
//        {
//            user.CurState = EnterState;
//        }

//        /// <summary>
//        /// ����״̬��
//        /// </summary>
//        /// <param name="user"></param>
//        public void OnUpdate(BehaviorStateMachine user)
//        {
//            if(TryGetNextState(user, out BaseState nextState))
//            {
//                SetState(user, nextState);
//            }
//            States[user.CurState].OnUpdate(user);
//        }

//        /// <summary>
//        /// ��ȡ��һ��״̬
//        /// </summary>
//        /// <returns>�Ƿ�ı�״̬</returns>
//        private bool TryGetNextState(BehaviorStateMachine user, out BaseState nextState)
//        {
//            //�ȼ������״̬�����Ƿ����
//            foreach(var transition in AnyState)
//            {
//                if (transition.Reason(user))
//                {
//                    nextState = transition.EndState;
//                    return true;
//                }
//            }
//            //�ټ�鵱ǰ״̬�Ĺ����Ƿ����
//            return States[user.CurState].TryGetNextState(user, out nextState);
//        }

//        /// <summary>
//        /// ����Ϊָ��״̬
//        /// </summary>
//        /// <param name="user"></param>
//        /// <param name="state"></param>
//        private void SetState(BehaviorStateMachine user, BaseState state)
//        {
//            state.OnExit(user);
//            user.CurState = States.IndexOf(state);
//            state.OnEnter(user);
//        }

//        /// <summary>
//        /// �˺��߼�
//        /// </summary>
//        public void Hurt(BehaviorStateMachine user, float hurt, Status status)
//        {
//            States[user.CurState].Hurt(user, hurt, status);
//        }

//        public IProduct Clone()
//        {
//            return this;
//        }
//    }
//}
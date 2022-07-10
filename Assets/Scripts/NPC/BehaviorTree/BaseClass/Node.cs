using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameFrame.Behavior.Tree {
    /// <summary>
    /// ��Ϊ�ڵ����
    /// </summary>
    public abstract class Node : BaseNode
    {
#if UNITY_EDITOR
        /// <summary>
        /// ����ӽڵ�
        /// </summary>
        /// <param name="node"></param>
        public abstract void AddChild(Node node);
        /// <summary>
        /// �Ƴ��ӽڵ�
        /// </summary>
        /// <param name="node"></param>
        public abstract void RemoveChild(Node node);

        /// <summary>
        /// ��ȡ��̵���Ϊ�ڵ�
        /// </summary>
        /// <returns></returns>
        public abstract Node[] GetChildren();
#endif
        /// <summary>
        /// �ڵ�״̬
        /// </summary>
        [System.Serializable]
        public enum NodeStatus
        {
            /// <summary>
            /// �ڵ���δ���й�
            /// </summary>
            None,
            /// <summary>
            /// �ڵ����гɹ�
            /// </summary>
            Success,
            /// <summary>
            /// �ڵ�����ʧ��
            /// </summary>
            Failure,
            /// <summary>
            /// �ڵ���������
            /// </summary>
            Running,
            /// <summary>
            /// �ڵ����б����
            /// </summary>
            Aborting,
        }

        /// <summary>
        /// ��ǰ״̬
        /// </summary>
        public NodeStatus Status
        {
            get => _status;
            private set
            {
                _status = value;
#if UNITY_EDITOR
                switch (_status)
                {
                    case NodeStatus.Success:
                        ChangeColor(Color.green);
                        break;
                    case NodeStatus.Failure:
                        ChangeColor(Color.red);
                        break;
                    case NodeStatus.Running:
                        ChangeColor(Color.blue);
                        break;
                    case NodeStatus.Aborting:
                        ChangeColor(Color.yellow);
                        break;
                    default:
                        ChangeColor(Color.gray);
                        break;
                }
#endif
            }
        }
        NodeStatus _status = NodeStatus.None;

        protected override sealed object GetValue(string fieldInfo)
        {
            if (_lastUpdataTime != Time.time)
            {
                throw new ProcessException(this, $"�쳣����δ�ڵ������ϵ�{this}");
            }
            var type = GetType();
            do
            {
                var field = GetType().GetField(fieldInfo);
                if (field != null)
                {
                    return field.GetValue(this);
                }
            } while (type != typeof(BaseNode));
            return null;
        }

        /// <summary>
        /// �����ø���
        /// </summary>
        /// <returns>�ýڵ���º��״̬</returns>
        public NodeStatus Tick()
        {
            _lastUpdataTime = Time.time;
            Init();
            //���ڵ㲢δ��������״̬ʱ�����н���״̬����
            if (Status != NodeStatus.Running)
            {
                //���ڵ㴦�ڱ����״̬ʱ�ָ�����
                if (Status == NodeStatus.Aborting) OnResume();
                //����ڵ㴦���ѽ���״̬�����½�������״̬
                else OnEnter();
                Status = NodeStatus.Running;
            }
            Status = OnUpdate();
            //���ڵ����н����󣬽����˳�״̬����
            if (Status != NodeStatus.Running) OnExit();
            return Status;
        }

        /// <summary>
        /// ���ýڵ��������ʱ����
        /// </summary>
        protected virtual void OnEnter() { }

        /// <summary>
        /// ���ýڵ��˳�����ʱ����
        /// </summary>
        protected virtual void OnExit() { }

        /// <summary>
        /// ���ýڵ㱻���ʱ����
        /// </summary>
        protected virtual void OnAbort() { }

        /// <summary>
        /// ���ýڵ�ָ�����ʱ����
        /// </summary>
        protected virtual void OnResume() { }

        /// <summary>
        /// ��ϵ�ǰ�ڵ������״̬
        /// </summary>
        public void Abort()
        {
            //���ڵ�δ������ʱ��������������Ϣ
            if (Status != NodeStatus.Running)
            {
                Debug.Log($"{BehaviorTree.name}���Դ��[{GetType()}]{Name}�ڵ㣬���ڵ㴦��{Status}״̬");
                return;
            }
            OnAbort();
            Status = NodeStatus.Aborting;
        }

        /// <summary>
        /// ���ڵ㴦������״̬ʱ����
        /// </summary>
        /// <returns>�������еĽ��</returns>
        protected abstract NodeStatus OnUpdate();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEditor;
using System.Text.RegularExpressions;

namespace GameFrame.EventProcessor
{
    /// <summary>
    /// �¼�������
    /// </summary>
    /// <remarks>
    /// ��������¼����߼�����
    /// </remarks>
    [CreateAssetMenu(fileName = "EventProcessor", menuName = "EventManager/EventProcessor")]
    public class EventProcessor : CustomGraph<Node>
    {
        /// <summary>
        /// �¼�����
        /// </summary>
        public string EventName;
        /// <summary>
        /// �¼�����
        /// </summary>
        [ReadOnly, ShowInInspector]
        public Type EventType => SenderNode.EventType;
        /// <summary>
        /// ��ڽڵ�
        /// </summary>
        public EventSenderNode SenderNode;
        /// <summary>
        /// �Ƿ�ֻ����һ��
        /// </summary>
        public bool IsTrigger = false;
        /// <summary>
        /// �¼��Ƿ�־�
        /// </summary>
        public bool IsPresistent = false;
        /// <summary>
        /// ��ʼ��������
        /// </summary>
        internal void Attach()
        {
            foreach(var node in Nodes)
            {
                node.Attach();
            }
        }

        /// <summary>
        /// ���봦����
        /// </summary>
        internal void Detach()
        {
           foreach (var node in Nodes)
            {
                node.Detach();
            }
        }
        /// <summary>
        /// �����¼�
        /// </summary>
        /// <typeparam name="TEventType">�¼�����</typeparam>
        /// <param name="event">ʱ�����</param>
        internal void SendEvent<TEventType>(TEventType @event) where TEventType : EventBase
        {
            EventManager.Instance.Broadcast(EventName, @event, IsPresistent);
            if (IsTrigger)
            {
                EventProcessorManager.Instance.DetachProcessor(this);
            }
        }


        public override Node CreateNode(Type type)
        {
            var node = base.CreateNode(type);
            if (type.IsSubclassOf(typeof(EventSenderNode)))
            {
                node.Name = "Sender";
            }
            typeof(Node).GetProperty("Processor").SetValue(node, this);
            return node;
        }

        public override string ToString()
        {
            return $"{name}({EventType.Name}.{EventName})";
        }
    }
}
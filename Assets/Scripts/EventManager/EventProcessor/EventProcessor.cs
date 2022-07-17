using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
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
    public class EventProcessor : SerializedScriptableObject
    {
        /// <summary>
        /// �ڵ��б�
        /// </summary>
        public List<Node> Nodes = new List<Node>();
        /// <summary>
        /// �¼�����
        /// </summary>
        public string EventName;
        /// <summary>
        /// �¼�����
        /// </summary>
        public Type EventType;
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

        /// <summary>
        /// ����guid���ҽڵ�
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public Node FindNode(string guid)
        {
            return Nodes.Find(n => n.Guid == guid);
        }
#if UNITY_EDITOR
        /// <summary>
        /// �Ƴ��ڵ�
        /// </summary>
        /// <param name="node"></param>
        public void RemoveNode(Node node)
        {
            Nodes.Remove(node);
            //�Ƴ��Ըýڵ����Դ��
            foreach (var child in Nodes)
            {
                for (int i = child.InputEdges.Count - 1; i >= 0; --i)
                    if (child.InputEdges[i].SourceNode == node)
                        child.InputEdges.RemoveAt(i);
                for (int i = child.OutputEdges.Count - 1; i >= 0; --i)
                    if (child.OutputEdges[i].TargetNode == node)
                        child.OutputEdges.RemoveAt(i);
            }
        }
        public Node CreateNode(Type type)
        {
            var node = Activator.CreateInstance(type) as Node;
            string newName = type.Name;
            int count = Nodes.FindAll(node => Regex.IsMatch(node.Name, @$"{newName}(\(\d+\))?$")).Count;
            if (count > 0)
            {
                newName = newName + $"({count})";
            }
            node.Name = newName;
            node.Guid = GUID.Generate().ToString();
            typeof(Node).GetProperty("Processor").SetValue(node, this);
            Nodes.Add(node);
            return node;
        }
#endif
        public override string ToString()
        {
            return EventName;
        }
    }
}

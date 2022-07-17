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
    /// 事件处理器
    /// </summary>
    /// <remarks>
    /// 负责进行事件的逻辑处理
    /// </remarks>
    [CreateAssetMenu(fileName = "EventProcessor", menuName = "EventManager/EventProcessor")]
    public class EventProcessor : SerializedScriptableObject
    {
        /// <summary>
        /// 节点列表
        /// </summary>
        public List<Node> Nodes = new List<Node>();
        /// <summary>
        /// 事件名称
        /// </summary>
        public string EventName;
        /// <summary>
        /// 事件类型
        /// </summary>
        public Type EventType;
        /// <summary>
        /// 入口节点
        /// </summary>
        public EventSenderNode SenderNode;
        /// <summary>
        /// 是否只触发一次
        /// </summary>
        public bool IsTrigger = false;
        /// <summary>
        /// 事件是否持久
        /// </summary>
        public bool IsPresistent = false;
        /// <summary>
        /// 初始化处理器
        /// </summary>
        internal void Attach()
        {
            foreach(var node in Nodes)
            {
                node.Attach();
            }
        }

        /// <summary>
        /// 剥离处理器
        /// </summary>
        internal void Detach()
        {
           foreach (var node in Nodes)
            {
                node.Detach();
            }
        }
        /// <summary>
        /// 发送事件
        /// </summary>
        /// <typeparam name="TEventType">事件类型</typeparam>
        /// <param name="event">时间参数</param>
        internal void SendEvent<TEventType>(TEventType @event) where TEventType : EventBase
        {
            EventManager.Instance.Broadcast(EventName, @event, IsPresistent);
            if (IsTrigger)
            {
                EventProcessorManager.Instance.DetachProcessor(this);
            }
        }

        /// <summary>
        /// 根据guid查找节点
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public Node FindNode(string guid)
        {
            return Nodes.Find(n => n.Guid == guid);
        }
#if UNITY_EDITOR
        /// <summary>
        /// 移除节点
        /// </summary>
        /// <param name="node"></param>
        public void RemoveNode(Node node)
        {
            Nodes.Remove(node);
            //移除以该节点的资源边
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

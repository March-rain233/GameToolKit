using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFrame
{
    public sealed class EventManager
    {
        public static EventManager Instance 
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new EventManager();
                }
                return _instance;
            }
        }
        static EventManager _instance;
        private EventManager() { }
        /// <summary>
        /// 注册指定类型名称事件的回调
        /// </summary>
        /// <typeparam name="TEventType">事件类型</typeparam>
        /// <param name="name">事件名称</param>
        /// <param name="callback">回调函数</param>
        public void RegisterCallback<TEventType>(string name, Action<TEventType> callback) where TEventType : EventBase
        {
            //todo
        }
        /// <summary>
        /// 注销指定类型名称事件的回调
        /// </summary>
        /// <typeparam name="TEventType">事件类型</typeparam>
        /// <param name="name">事件名称</param>
        /// <param name="callback">回调函数</param>
        public void UnRegisterCallback<TEventType>(string name, Action<TEventType> callback) where TEventType : EventBase
        {
            //todo
        }
        /// <summary>
        /// 广播事件
        /// </summary>
        /// <typeparam name="TEventType">事件类型</typeparam>
        /// <param name="name">事件名称</param>
        /// <param name="event">事件参数</param>
        /// <param name="isPersistent">是否持久化保存在已触发事件列表中</param>
        public void Broadcast<TEventType>(string name, TEventType @event, bool isPersistent = false) where TEventType : EventBase
        {
            //todo
        }
        /// <summary>
        /// 获取已触发的事件列表
        /// </summary>
        /// <returns>事件列表</returns>
        public List<KeyValuePair<string, EventBase>> GetTriggeredEventList()
        {
            //todo
            return default;
        }
        /// <summary>
        /// 获取指定名称的已触发的事件
        /// </summary>
        /// <typeparam name="TEventType">事件类型</typeparam>
        /// <param name="name">事件名称</param>
        /// <returns>事件参数</returns>
        public TEventType GetTriggeredEvnet<TEventType>(string name) where TEventType : EventBase
        {
            //todo
            return default;
        }
    }
}

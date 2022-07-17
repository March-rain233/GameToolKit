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
        /// ע��ָ�����������¼��Ļص�
        /// </summary>
        /// <typeparam name="TEventType">�¼�����</typeparam>
        /// <param name="name">�¼�����</param>
        /// <param name="callback">�ص�����</param>
        public void RegisterCallback<TEventType>(string name, Action<TEventType> callback) where TEventType : EventBase
        {
            //todo
        }
        /// <summary>
        /// ע��ָ�����������¼��Ļص�
        /// </summary>
        /// <typeparam name="TEventType">�¼�����</typeparam>
        /// <param name="name">�¼�����</param>
        /// <param name="callback">�ص�����</param>
        public void UnRegisterCallback<TEventType>(string name, Action<TEventType> callback) where TEventType : EventBase
        {
            //todo
        }
        /// <summary>
        /// �㲥�¼�
        /// </summary>
        /// <typeparam name="TEventType">�¼�����</typeparam>
        /// <param name="name">�¼�����</param>
        /// <param name="event">�¼�����</param>
        /// <param name="isPersistent">�Ƿ�־û��������Ѵ����¼��б���</param>
        public void Broadcast<TEventType>(string name, TEventType @event, bool isPersistent = false) where TEventType : EventBase
        {
            //todo
        }
        /// <summary>
        /// ��ȡ�Ѵ������¼��б�
        /// </summary>
        /// <returns>�¼��б�</returns>
        public List<KeyValuePair<string, EventBase>> GetTriggeredEventList()
        {
            //todo
            return default;
        }
        /// <summary>
        /// ��ȡָ�����Ƶ��Ѵ������¼�
        /// </summary>
        /// <typeparam name="TEventType">�¼�����</typeparam>
        /// <param name="name">�¼�����</param>
        /// <returns>�¼�����</returns>
        public TEventType GetTriggeredEvnet<TEventType>(string name) where TEventType : EventBase
        {
            //todo
            return default;
        }
    }
}

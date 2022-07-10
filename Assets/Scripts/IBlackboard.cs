using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameFrame
{
    /// <summary>
    /// �ڰ�ӿ�
    /// </summary>
    public interface IBlackboard
    {
#if UNITY_EDITOR
        public class BlackboardEventBase
        {
            public IBlackboard Target { get; private set; }
            public string Name { get; private set; }

            public BlackboardEventBase(IBlackboard target, string name)
            {
                Target = target;
                Name = name;
            }

        }
        public class NameChangedEvent:BlackboardEventBase
        {
            public string OldName { get; private set; }
            public NameChangedEvent(IBlackboard target, string name, string oldname):base(target, name)
            {
                OldName = oldname;
            }
        }
        public class ValueRemoveEvent : BlackboardEventBase
        {
            public ValueRemoveEvent(IBlackboard target, string name):base(target, name)
            {
            }
        }
        /// <summary>
        /// ע��ص�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        public void RegisterCallback<T>(string name, Action<T> callback) where T : BlackboardEventBase;
        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="name"></param>
        public void RenameValue(string name, string newName);
        public BlackboardVariable GetVariable(string name);
#endif
        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetValue<T>(string name);
        /// <summary>
        /// ���ñ���
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetValue(string name, object value);
        /// <summary>
        /// �Ƴ�����
        /// </summary>
        /// <param name="name"></param>
        public void RemoveValue(string name);
        /// <summary>
        /// �ж��Ƿ���ڱ���
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasValue(string name);
    }
}

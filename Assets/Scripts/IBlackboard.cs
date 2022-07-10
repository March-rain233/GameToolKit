using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameFrame
{
    /// <summary>
    /// 黑板接口
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
        /// 注册回调
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        public void RegisterCallback<T>(string name, Action<T> callback) where T : BlackboardEventBase;
        /// <summary>
        /// 重命名变量
        /// </summary>
        /// <param name="name"></param>
        public void RenameValue(string name, string newName);
        public BlackboardVariable GetVariable(string name);
#endif
        /// <summary>
        /// 获取变量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetValue<T>(string name);
        /// <summary>
        /// 设置变量
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetValue(string name, object value);
        /// <summary>
        /// 移除变量
        /// </summary>
        /// <param name="name"></param>
        public void RemoveValue(string name);
        /// <summary>
        /// 判断是否存在变量
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasValue(string name);
    }
}

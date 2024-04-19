using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.Serialization;
namespace GameToolKit
{
    /// <summary>
    /// 黑板接口
    /// </summary>
    public interface IBlackboard
    {
        /// <summary>
        /// 根据id获取变量
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BlackboardVariable this[string id] { get;}

        /// <summary>
        /// 变量添加事件
        /// </summary>
        public event Action<string, BlackboardVariable> VariableAdded;

        /// <summary>
        /// 变量移除事件
        /// </summary>
        public event Action<string, BlackboardVariable> VariableRemoved;

        /// <summary>
        /// id管理器
        /// </summary>
        public IGUIDManager GUIDManager { get; }

        /// <summary>
        /// 移除变量
        /// </summary>
        /// <param name="id"></param>
        public bool RemoveVariable(string id);

        /// <summary>
        /// 增加变量
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddVariable(string name, BlackboardVariable value);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.Serialization;
namespace GameToolKit
{
    /// <summary>
    /// �ڰ�ӿ�
    /// </summary>
    public interface IBlackboard
    {
        /// <summary>
        /// ����id��ȡ����
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BlackboardVariable this[string id] { get;}

        /// <summary>
        /// id������
        /// </summary>
        public IGUIDManager GUIDManager { get; }

        /// <summary>
        /// �Ƴ�����
        /// </summary>
        /// <param name="id"></param>
        public void RemoveVariable(string id);

        /// <summary>
        /// ���ӱ���
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddVariable(string name, BlackboardVariable value);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameToolKit
{
    /// <summary>
    /// ������Դ�˿ڵ�����
    /// </summary>
    /// <remarks>
    /// �������дgroupList��������Ĭ��Ϊ����������
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited =true)]
    public class SourcePortAttribute : PortAttribute
    {
        /// <summary>
        /// �Ƿ���ʾ�ڲ�����
        /// </summary>
        public bool ShowInterior { get; private set; } = false;

        /// <summary>
        /// �˿�����
        /// </summary>
        /// <param name="name">�˿���ʾ������</param>
        /// <param name="direction">�˿������������</param>
        /// <param name="extendPortTypes">����Ķ˿ڿ�ƥ�����������</param>
        public SourcePortAttribute(string portName, PortDirection direction, string[] groupList = null, PortFilterType filterType = PortFilterType.Whitelist) 
            : base(portName, direction, direction == PortDirection.Input ? PortCapacity.Single : PortCapacity.Multi, filterType, groupList)
        {
            ShowInterior = false;
        }
        /// <summary>
        /// �˿�����
        /// </summary>
        /// <param name="portName">�˿���ʾ������</param>
        /// <param name="direction">�˿������������</param>
        /// <param name="isMemberFields">�Ƿ��Ǳ�¶�ڲ��ֶζ����Ƕ�����</param>
        public SourcePortAttribute(string portName, PortDirection direction, bool showInterior)
            : base(portName, direction, direction == PortDirection.Input ? PortCapacity.Single : PortCapacity.Multi, PortFilterType.Whitelist, null)
        {
            ShowInterior = showInterior;
        }
    }
}
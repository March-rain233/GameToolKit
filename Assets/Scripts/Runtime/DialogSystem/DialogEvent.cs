using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Dialog
{
    /// <summary>
    /// �Ի��¼�
    /// </summary>
    /// <remarks>
    /// ͨ�����͸��¼���֪ͨ�Ի�ϵͳ���ŶԻ�
    /// </remarks>
    public abstract class DialogEvent : EventBase
    {
    }
    /// <summary>
    /// �Ի���ջ�¼�
    /// </summary>
    public class DialogEnqueueEvent : DialogEvent
    {
        public DialogTree DialogTree;
    }
}
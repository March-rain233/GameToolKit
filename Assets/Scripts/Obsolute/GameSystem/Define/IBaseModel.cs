using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBaseModel
{
    /// <summary>
    /// ����֪ͨ��ͼ�����ݸ���
    /// </summary>
    /// <remarks>
    /// ��Ҫ���֪�����µ����ݣ��������ݸ���ʱ���ø��¼��������������ֵ
    /// </remarks>
    event System.Action<string, object> NotifyEvent;
}

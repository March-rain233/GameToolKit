using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ͼ�Ļ��࣬��ΪView��ܿش��ڵ���ʾ
/// </summary>
public abstract class BasePanel : BaseView
{
    public abstract PanelType Type
    {
        get;
    }
}


/// <summary>
/// ��������
/// </summary>
[System.Serializable]
public enum PanelType
{
    StatusPanel,
}
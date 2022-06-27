using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ҫͨ���������ɵĶ���Ҫ�̳�����ӿ�
/// </summary>
public interface IProduct
{
    IProduct Clone();
}

/// <summary>
/// �����װ��
/// </summary>
public class PSprite : IProduct
{
    public Sprite Sprite;
    public IProduct Clone()
    {
        return this;
    }
}

/// <summary>
/// ��ЧƬ�ΰ�װ��
/// </summary>
public class PAudioClip : IProduct
{
    public AudioClip AudioClip;
    public IProduct Clone()
    {
        return this;
    }
}
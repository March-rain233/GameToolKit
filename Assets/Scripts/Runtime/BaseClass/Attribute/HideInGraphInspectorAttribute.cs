using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameFrame
{
    /// <summary>
    /// ��ͼ�α༭���ļ�����������
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class HideInGraphInspectorAttribute : Attribute
    {

    }
}
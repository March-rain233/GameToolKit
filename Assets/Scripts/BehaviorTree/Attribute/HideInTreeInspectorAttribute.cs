using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameFrame
{
    /// <summary>
    /// �����α༭���ļ�����������
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class HideInTreeInspectorAttribute : Attribute
    {

    }
}
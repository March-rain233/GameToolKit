using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFrame
{
    /// <summary>
    /// 显示在节点拓展区
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ShowInNodeExtension : Attribute
    {
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���Ի���
/// </summary>
public abstract class BaseAssertion : ScriptableObject
{
    public abstract bool Value
    {
        get;
    }
}
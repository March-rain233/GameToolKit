using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Item
{
    /// <summary>
    /// ������������
    /// </summary>
    [Flags, Serializable]
    public enum Status : long
    {
        Common = 1,
        Fire = Common << 1,
        Break = Fire << 1,
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame
{
    /// <summary>
    /// UIϵͳ����
    /// </summary>
    public class UISetting : ScriptableSingleton<UISetting>
    {
        /// <summary>
        /// Ԥ�����ֵ�
        /// </summary>
        public Dictionary<string, GameObject> PrefabsDic;
    }
}

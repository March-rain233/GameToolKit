using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame
{
    /// <summary>
    /// UI系统设置
    /// </summary>
    public class UISetting : ScriptableSingleton<UISetting>
    {
        /// <summary>
        /// 预制体字典
        /// </summary>
        public Dictionary<string, GameObject> PrefabsDic;
    }
}

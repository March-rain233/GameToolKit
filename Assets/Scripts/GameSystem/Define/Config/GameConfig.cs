using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Config
{
    /// <summary>
    /// ȫ����Ϸ����
    /// </summary>
    [CreateAssetMenu(fileName = "ȫ������", menuName = "�����ļ�/ȫ������")]
    public class GameConfig : SerializedScriptableObject
    {
        public PathInfoConfig PathConfig;
        public SceneInfoConfig SceneInfoConfig;
    }
}
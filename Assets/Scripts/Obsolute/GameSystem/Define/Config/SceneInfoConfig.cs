using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "�������û���", menuName = "�����ļ�/�������û���")]
    public class SceneInfoConfig : SerializedScriptableObject
    {
        public Dictionary<string, SceneInfo> ScenesObject;
    }
}

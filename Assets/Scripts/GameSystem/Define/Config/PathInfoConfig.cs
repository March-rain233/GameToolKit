using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "·�����û���", menuName = "�����ļ�/·�����û���")]
    public class PathInfoConfig : SerializedScriptableObject
    {
        public Dictionary<ObjectType, PathInfo> Paths;
    }
}

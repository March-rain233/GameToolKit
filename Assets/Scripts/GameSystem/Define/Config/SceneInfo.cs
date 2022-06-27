using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Config
{
    /// <summary>
    /// ������Ʒ�����ļ�
    /// </summary>
    [CreateAssetMenu(fileName = "��������", menuName = "�����ļ�/��������")]
    public class SceneInfo : SerializedScriptableObject
    {
        public Save.ObjectInfo[] Objects;
        public Dictionary<string, Vector2> Positions;
        public string Scene;
        public string BGM;
    }
}

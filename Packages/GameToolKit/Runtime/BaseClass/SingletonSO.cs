using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEditor;

namespace GameToolKit {

    /// <summary>
    /// µ¥ÀýSO»ùÀà
    /// </summary>
    /// <typeparam name="TSingleton"></typeparam>
    public abstract class SingletonSO<TSingleton> : SerializedScriptableObject 
        where TSingleton : SingletonSO<TSingleton>
    {
        static TSingleton _instance;

        public static TSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<TSingleton>($"{typeof(TSingleton).Name}");
                }
                if (_instance == null)
                {
#if UNITY_EDITOR
                    _instance = CreateInstance<TSingleton>();
                    UnityEditor.AssetDatabase.CreateAsset(_instance, $@"Assets/Resources/{typeof(TSingleton).Name}.asset");
                    UnityEditor.AssetDatabase.SaveAssets();
                    UnityEditor.AssetDatabase.Refresh();
                    Debug.Log($"create file at Assets/Resources/{typeof(TSingleton).Name}.asset");
#else
                    Debug.LogError($"missing {typeof(TSingleton).Name}");
#endif
                }
                return _instance;
            }
        }
    }
}

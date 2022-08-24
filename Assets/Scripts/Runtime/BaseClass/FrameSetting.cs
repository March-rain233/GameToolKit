using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameFrame
{
    /// <summary>
    /// øÚº‹…Ë÷√
    /// </summary>
    [CreateAssetMenu(menuName = "Config/Frame Setting", fileName = "Frame Setting")]
    public class FrameSetting : ScriptableSingleton<FrameSetting>
    {
        [ValueDropdown("GetInitializer")]
        public System.Type Initializer;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            Debug.Log("init°≠°≠");
            ((ServiceInitializer)System.Activator.CreateInstance(Instance.Initializer)).Initialize();
        }

        IEnumerable<System.Type> GetInitializer()
        {
            return UnityEditor.TypeCache.GetTypesDerivedFrom<ServiceInitializer>();
        }
    }
}

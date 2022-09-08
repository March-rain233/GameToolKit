using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace GameToolKit.Dialog
{
    [CreateAssetMenu(fileName = "Dialog Box Config", menuName = "Dialog/Dialog Box Config")]
    public class DialogBoxConfig : SerializedScriptableObject
    {
        public Dictionary<Type, GameObject> DialogBoxPrefabs = new Dictionary<Type, GameObject>();
    }
}

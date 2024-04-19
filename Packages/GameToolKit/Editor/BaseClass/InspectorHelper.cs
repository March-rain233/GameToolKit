using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;

namespace GameToolKit.Editor
{
    /// <summary>
    /// 用于显示非unity对象
    /// </summary>
    internal class InspectorHelper : SerializedScriptableObject
    {
        public object InspectorData;
    }
}

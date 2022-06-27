using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAsset
{
    /// <summary>
    /// �Ի��ļ�
    /// </summary>
    public class Dialogue : SerializedScriptableObject
    {
        [System.Serializable]
        private class DialogueCell
        {
            /// <summary>
            /// �Ի�������ʾ������
            /// </summary>
            public string Name;

            /// <summary>
            /// �Ի����ڵ��ı�
            /// </summary>
            [TextArea]
            public string Text;

            /// <summary>
            /// �Ի�������Ӧ����Ϸ���������
            /// </summary>
            public string ObjectName;
        }


    }
}

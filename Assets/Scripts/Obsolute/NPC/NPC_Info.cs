using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace Save
{
    /// <summary>
    /// ��ɫ�浵
    /// </summary>
    [CreateAssetMenu(fileName = "��ɫ��Ϣ", menuName = "��ɫ��Ϣ")]
    [Serializable]
    public class NPC_Info : SerializedScriptableObject
    {
        /// <summary>
        /// ��ɫ���������
        /// </summary>
        public string Breed;

        /// <summary>
        /// ��ɫװ��
        /// </summary>
        public EquipSave Equip;

        /// <summary>
        /// ��Ϊģʽ������
        /// </summary>
        public string Behavior;

        /// <summary>
        /// Ԥ��������
        /// </summary>
        public string Prefabs;

        /// <summary>
        /// ��ɫѪ��
        /// </summary>
        public float Blood;

        /// <summary>
        /// ��ɫ����
        /// </summary>
        public float Energy;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Newtonsoft.Json;

namespace Save
{
    /// <summary>
    /// ��Ϸ�浵
    /// </summary>
    [System.Serializable]
    public partial class GameSave
    {
        /// <summary>
        /// ��ȡĬ�ϵĳ�ʼ��Ϸ�浵
        /// </summary>
        public static GameSave OriSave()
        {
            GameSave save = new GameSave();
            save.Inventory.Items = new Dictionary<string, int>();
            save.Story.MessagePath = new Dictionary<string, EventCenter.EventArgs>();
            save.Story.HaveTriggered = new List<string>();
            //save.Player = new Dictionary<string, NPC_Info>();
            //save.Player.Add("Shury", Resources.Load<NPC_Info>(GameManager.
            //    Instance.GameConfig.PathConfig.
            //    Paths[ObjectType.NPC_Info].PathDic["OriShury"]));
            save.Position.Scene = "��ʼ����";
            save.Position.Point = "����ĵص�";
            save.SceneIndex = -1;
            return save;
        }

        /// <summary>
        /// ��ҽ�ɫ�浵
        /// </summary>
        //public Dictionary<string, NPC_Info> Player;

        /// <summary>
        /// ���½��ȴ浵
        /// </summary>
        public ProgressSave Story;

        /// <summary>
        /// �ֿ�浵
        /// </summary>
        public InventorySave Inventory;

        public Map.MapPosition Position;


        public float Time;

        public int SceneIndex;

        //public Vector2 PlayerPosition;
    }
}

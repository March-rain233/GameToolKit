using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Dialog
{
    /// <summary>
    /// 对话系统管理器
    /// </summary>
    public class DialogManager
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static DialogManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DialogManager();
                }
                return _instance;
            }
        }
        static DialogManager _instance;

        /// <summary>
        /// 配置文件
        /// </summary>
        private DialogConfig _config;

        /// <summary>
        /// 等待运行的对话队列
        /// </summary>
        public Queue<DialogTree> WaitQueue { get; private set; } = new Queue<DialogTree> ();

        /// <summary>
        /// 正在运行的对话列表
        /// </summary>
        public List<DialogTree> RunningList { get; private set; } = new List<DialogTree> ();

        DialogManager()
        {
            _config = Resources.FindObjectsOfTypeAll<DialogConfig>()[0];
        }

        /// <summary>
        /// 查找对话树
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private DialogTree FindDialog(string name)
        {
            return _config.Dialogs.Find(elem=>elem.name == name);
        }

        /// <summary>
        /// 播放对话
        /// </summary>
        /// <param name="name"></param>
        public void PlayDialog(string name)
        {

        }
    }
}

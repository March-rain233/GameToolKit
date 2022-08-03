using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Sirenix.OdinInspector;
using UnityEditor;

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
        /// 等待运行的对话队列
        /// </summary>
        public Queue<DialogTree> WaitQueue { get; private set; } = new Queue<DialogTree> ();

        /// <summary>
        /// 正在运行的对话列表
        /// </summary>
        public List<DialogTree> RunningList { get; private set; } = new List<DialogTree> ();

        DialogManager()
        {
            EventManager.Instance.RegisterCallbackAll<DialogEvent>(e=>PlayDialog(e.Dialog));
        }

        /// <summary>
        /// 播放对话
        /// </summary>
        /// <param name="name"></param>
        public void PlayDialog(DialogTree dialog)
        {
            WaitQueue.Enqueue(dialog);
            Refresh();
        }

        /// <summary>
        /// 刷新数据，检查当前逻辑
        /// </summary>
        private void Refresh()
        {
            //不存在对话在等待，直接返回
            if(WaitQueue.Count == 0)
            {
                return;
            }

            //添加运行对话
            Action addDialog = () =>
            {
                if(WaitQueue.Count == 0)
                {
                    return;
                }

                var dialog = WaitQueue.Dequeue();

                //注册对话结束事件
                Action onDialogEnd = null;
                onDialogEnd = () =>
                {
                    dialog.DialogEnded -= onDialogEnd;
                    RunningList.Remove(dialog);
                    Refresh();
                };
                dialog.DialogEnded += onDialogEnd;

                RunningList.Add(dialog);
                dialog.Play();
            };

            //当前没有对话在运行时，加入对话
            if(RunningList.Count == 0)
            {
                addDialog();
            }

            //如果当前的对话不是顺序模式的话，加入后续的并发模式对话
            if(RunningList.Count != 0 && RunningList[0].Mode != DialogTree.DialogMode.Sequential)
            {
                while (WaitQueue.Peek().Mode != DialogTree.DialogMode.Sequential)
                {
                    addDialog();
                }
            }
        }
    }
}

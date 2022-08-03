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
    /// �Ի�ϵͳ������
    /// </summary>
    public class DialogManager
    {
        /// <summary>
        /// ʵ��
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
        /// �ȴ����еĶԻ�����
        /// </summary>
        public Queue<DialogTree> WaitQueue { get; private set; } = new Queue<DialogTree> ();

        /// <summary>
        /// �������еĶԻ��б�
        /// </summary>
        public List<DialogTree> RunningList { get; private set; } = new List<DialogTree> ();

        DialogManager()
        {
            EventManager.Instance.RegisterCallbackAll<DialogEvent>(e=>PlayDialog(e.Dialog));
        }

        /// <summary>
        /// ���ŶԻ�
        /// </summary>
        /// <param name="name"></param>
        public void PlayDialog(DialogTree dialog)
        {
            WaitQueue.Enqueue(dialog);
            Refresh();
        }

        /// <summary>
        /// ˢ�����ݣ���鵱ǰ�߼�
        /// </summary>
        private void Refresh()
        {
            //�����ڶԻ��ڵȴ���ֱ�ӷ���
            if(WaitQueue.Count == 0)
            {
                return;
            }

            //������жԻ�
            Action addDialog = () =>
            {
                if(WaitQueue.Count == 0)
                {
                    return;
                }

                var dialog = WaitQueue.Dequeue();

                //ע��Ի������¼�
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

            //��ǰû�жԻ�������ʱ������Ի�
            if(RunningList.Count == 0)
            {
                addDialog();
            }

            //�����ǰ�ĶԻ�����˳��ģʽ�Ļ�����������Ĳ���ģʽ�Ի�
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

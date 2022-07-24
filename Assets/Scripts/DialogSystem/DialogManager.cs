using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        /// �����ļ�
        /// </summary>
        private DialogConfig _config;

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
            _config = Resources.FindObjectsOfTypeAll<DialogConfig>()[0];
        }

        /// <summary>
        /// ���ҶԻ���
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private DialogTree FindDialog(string name)
        {
            return _config.Dialogs.Find(elem=>elem.name == name);
        }

        /// <summary>
        /// ���ŶԻ�
        /// </summary>
        /// <param name="name"></param>
        public void PlayDialog(string name)
        {

        }
    }
}

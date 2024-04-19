using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// �Ի�ϵͳ������
    /// </summary>
    public class DialogManager
    {
        /// <summary>
        /// �Ի���-��ʾUI�����б�
        /// </summary>
        List<(DialogGraph tree, IDialogBox box)> _dialogReferenceList = new List<(DialogGraph, IDialogBox)>();

        /// <summary>
        /// �ȴ����еĶԻ�����
        /// </summary>
        [ShowInInspector, ReadOnly, SerializeField]
        Queue<DialogGraph> _waitingQueue = new Queue<DialogGraph> ();

        /// <summary>
        /// �������еĶԻ��б�
        /// </summary>
        [ShowInInspector, ReadOnly, SerializeField]
        List<DialogGraph> _runningList = new List<DialogGraph> ();

        /// <summary>
        /// ���Ի����������ж���
        /// </summary>
        /// <param name="name"></param>
        [Button]
        public void RunDialogTree(DialogGraph dialog)
        {
            _waitingQueue.Enqueue(dialog);
            Refresh();
        }

        /// <summary>
        /// ȡ���Ի�������
        /// </summary>
        /// <remarks>
        /// �����еĶԻ����ᱻ�жϣ���δ���еĶԻ����ᱻ�Ƴ�������
        /// </remarks>
        /// <param name="dialog"></param>
        public void CancelDialogTree(DialogGraph dialog)
        {
            if (_runningList.Contains(dialog))
            {
                dialog.Abort();
                RemoveDialogTree(dialog);
            }
            else if(_waitingQueue.Contains(dialog))
                _waitingQueue = new Queue<DialogGraph>(_waitingQueue.Where(e => e != dialog));
        }

        /// <summary>
        /// �ƶ��Ի����������ж���
        /// </summary>
        private void AddDialogTree()
        {
            if (_waitingQueue.Count == 0) return;

            var dialog = _waitingQueue.Dequeue();

            Action onDialogEnd = null;
            onDialogEnd = () =>
            {
                dialog.OnDialogEnd -= onDialogEnd;

                RemoveDialogTree(dialog);

                Refresh();
            };

            dialog.OnDialogEnd += onDialogEnd;
            _runningList.Add(dialog);
            dialog.Play();
        }

        /// <summary>
        /// �Ƴ��Ի���
        /// </summary>
        /// <param name="dialog"></param>
        void RemoveDialogTree(DialogGraph dialog)
        {
            _runningList.Remove(dialog);

            //��ȡ�Ի����������
            var counts = (from item in (from pair in _dialogReferenceList
                                        group pair by pair.box)
                          select (box: item.Key, num: item.Count())).ToDictionary(p => p.box, p => p.num);
            //�ر�������Ϊ0�ĶԻ���
            for (int i = _dialogReferenceList.Count - 1; i >= 0; --i)
            {
                if (ReferenceEquals(_dialogReferenceList[i].tree, dialog) && --counts[_dialogReferenceList[i].box] == 0)
                {
                    _dialogReferenceList[i].box.CloseDialogBox();
                    _dialogReferenceList.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// ˢ�����ݣ���鵱ǰ�߼�
        /// </summary>
        private void Refresh()
        {
            //�����ڶԻ��ڵȴ���ֱ�ӷ���
            if (_waitingQueue.Count == 0) return;


            //��ǰû�жԻ�������ʱ������Ի�
            if(_runningList.Count == 0) AddDialogTree();

            //�����ǰ�ĶԻ�����˳��ģʽ�Ļ�����������Ĳ���ģʽ�Ի�
            if(_runningList.Count != 0 && _runningList[0].Mode != DialogGraph.DialogMode.Sequential)
                while (_waitingQueue.Peek().Mode != DialogGraph.DialogMode.Sequential)
                    AddDialogTree();
        }

        /// <summary>
        /// ���жԻ�
        /// </summary>
        /// <param name="boxType">�Ի�������</param>
        /// <param name="argument">�Ի�����</param>
        /// <param name="onDialogEnd">��������ص�</param>
        public void PlayDialog(string boxType, DialogArgument argument, Action onDialogEnd, DialogGraph source)
        {
            var box = ServiceAP.Instance.PanelManager.GetOrOpenPanel(boxType) as IDialogBox;
            _dialogReferenceList.Add((source, box));
            box.PlayDialog(argument, onDialogEnd);
        }

        /// <summary>
        /// ����ѡ��
        /// </summary>
        /// <param name="optionType"></param>
        /// <param name="optionArguments"></param>
        /// <param name="onSlected"></param>
        public void PlayOption(string optionType, List<OptionArgument> optionArguments, Action<int> onSlected)
        {
            var view = ServiceAP.Instance.PanelManager.GetOrOpenPanel(optionType) as IOptionView;
            view.ShowOptions(optionArguments, onSlected);
        }

        /// <summary>
        /// ɾ���Ի�������
        /// </summary>
        /// <param name="boxtype"></param>
        /// <param name="tree"></param>
        public void DeleteDialogBoxReference(string boxtype, DialogGraph tree)
        {
            ServiceAP.Instance.PanelManager.TryGetPanel(boxtype, out var panel);
            var box = panel as IDialogBox;
            int count = _dialogReferenceList.Count(pair=>ReferenceEquals(pair.box, box));
            _dialogReferenceList.Remove((tree, box));
            if(count == 1) box.CloseDialogBox();
        }
    }
}

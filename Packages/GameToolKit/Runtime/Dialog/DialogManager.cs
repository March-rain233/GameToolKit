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
    /// 对话系统管理器
    /// </summary>
    public class DialogManager
    {
        /// <summary>
        /// 对话树-显示UI引用列表
        /// </summary>
        List<(DialogGraph tree, IDialogBox box)> _dialogReferenceList = new List<(DialogGraph, IDialogBox)>();

        /// <summary>
        /// 等待运行的对话队列
        /// </summary>
        [ShowInInspector, ReadOnly, SerializeField]
        Queue<DialogGraph> _waitingQueue = new Queue<DialogGraph> ();

        /// <summary>
        /// 正在运行的对话列表
        /// </summary>
        [ShowInInspector, ReadOnly, SerializeField]
        List<DialogGraph> _runningList = new List<DialogGraph> ();

        /// <summary>
        /// 将对话树推入运行队列
        /// </summary>
        /// <param name="name"></param>
        [Button]
        public void RunDialogTree(DialogGraph dialog)
        {
            _waitingQueue.Enqueue(dialog);
            Refresh();
        }

        /// <summary>
        /// 取消对话树运行
        /// </summary>
        /// <remarks>
        /// 运行中的对话树会被中断，还未运行的对话树会被移除出队列
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
        /// 推动对话树进入运行队列
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
        /// 移除对话树
        /// </summary>
        /// <param name="dialog"></param>
        void RemoveDialogTree(DialogGraph dialog)
        {
            _runningList.Remove(dialog);

            //获取对话框的引用数
            var counts = (from item in (from pair in _dialogReferenceList
                                        group pair by pair.box)
                          select (box: item.Key, num: item.Count())).ToDictionary(p => p.box, p => p.num);
            //关闭引用数为0的对话框
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
        /// 刷新数据，检查当前逻辑
        /// </summary>
        private void Refresh()
        {
            //不存在对话在等待，直接返回
            if (_waitingQueue.Count == 0) return;


            //当前没有对话在运行时，加入对话
            if(_runningList.Count == 0) AddDialogTree();

            //如果当前的对话不是顺序模式的话，加入后续的并发模式对话
            if(_runningList.Count != 0 && _runningList[0].Mode != DialogGraph.DialogMode.Sequential)
                while (_waitingQueue.Peek().Mode != DialogGraph.DialogMode.Sequential)
                    AddDialogTree();
        }

        /// <summary>
        /// 运行对话
        /// </summary>
        /// <param name="boxType">对话框类型</param>
        /// <param name="argument">对话参数</param>
        /// <param name="onDialogEnd">输出结束回调</param>
        public void PlayDialog(string boxType, DialogArgument argument, Action onDialogEnd, DialogGraph source)
        {
            var box = ServiceAP.Instance.PanelManager.GetOrOpenPanel(boxType) as IDialogBox;
            _dialogReferenceList.Add((source, box));
            box.PlayDialog(argument, onDialogEnd);
        }

        /// <summary>
        /// 发送选项
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
        /// 删除对话框引用
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

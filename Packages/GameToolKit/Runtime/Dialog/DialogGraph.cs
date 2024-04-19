using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;

namespace GameToolKit.Dialog
{
    [CreateAssetMenu(fileName = "DialogGraph", menuName = "Dialog/Dialog Graph")]
    public class DialogGraph : DataFlowGraph<DialogGraph ,Node>
    {
        public enum DialogMode
        {
            /// <summary>
            /// ����ִ��
            /// </summary>
            /// <remarks>
            /// �öԻ��������������Ի���ִͬ��
            /// </remarks>
            Concurrent,
            /// <summary>
            /// ˳��ִ��
            /// </summary>
            /// <remarks>
            /// �öԻ�ֻ�еȴ��������жԻ�ִ����Ϻ�ŻῪʼִ�У�
            /// �Һ����Ի���Ҫ�ȵ��öԻ�ִ����ϲſ�ִ��
            /// </remarks>
            Sequential
        }
        /// <summary>
        /// ����ģʽ
        /// </summary>
        public DialogMode Mode = DialogMode.Sequential;

        /// <summary>
        /// ��ڽڵ�
        /// </summary>
        public EntryNode EntryNode;

        /// <summary>
        /// ��ֹ�ڵ�
        /// </summary>
        public ExitNode ExitNode;

        /// <summary>
        /// �����еĽڵ��б�
        /// </summary>
        public List<ProcessNode> RunningNodes = new List<ProcessNode>();

        /// <summary>
        /// ���Ի�����
        /// </summary>
        public event Action OnDialogEnd;

        private void Reset()
        {
            EntryNode = CreateNode(typeof(EntryNode)) as EntryNode;
            ExitNode = CreateNode(typeof(ExitNode)) as ExitNode;
            Blackboard = new GraphBlackboard(false);
        }

        /// <summary>
        /// ��ʼ�Ի�
        /// </summary>
        public void Play()
        {
            EntryNode.Start(null);
        }

        /// <summary>
        /// �ж϶Ի�
        /// </summary>
        public void Abort()
        {
            foreach(var node in RunningNodes)
                node.Abort();
        }

        /// <summary>
        /// �����Ի�
        /// </summary>
        public void Finish()
        {
            OnDialogEnd?.Invoke();
            Terminate();
        }

        public override Node CreateNode(Type type)
        {
            var node = base.CreateNode(type);
            node.DialogTree = this;
            return node;
        }

        public override void RemoveNode(Node node)
        {
            //�Ƴ�����
            var n = node as ProcessNode;
            if (n != null)
            {
                foreach(ProcessNode processNode in Nodes.Where(n=>n is ProcessNode))
                {
                    processNode.Parents.Remove(n);
                    processNode.Children.Remove(n);
                }
            }
            base.RemoveNode(node);
        }
    }
}

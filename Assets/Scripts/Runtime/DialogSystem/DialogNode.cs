using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using System.Linq;

namespace GameFrame.Dialog
{
    /// <summary>
    /// �Ի��ڵ����
    /// </summary>
    [NodeCategory("Dialog")]
    public abstract class DialogNodeBase : ProcessNode
    {
        /// <summary>
        /// �ٿصĶԻ�������
        /// </summary>
        [ValueDropdown("GetValidDialogBoxType")]
        public System.Type DialogBoxType;

        /// <summary>
        /// ��ȡ���ʵĶԻ�������
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<Type> GetValidDialogBoxType()
        {
#if UNITY_EDITOR
            return Sirenix.OdinInspector.Editor.UnityTypeCacheUtility.GetTypesDerivedFrom(typeof(DialogBoxBase)).Where(t => !t.IsGenericType && !t.IsAbstract);
#endif
        }
    }

    /// <summary>
    /// �Ի��ڵ�
    /// </summary>
    public class DialogNode : DialogNodeBase
    {
        /// <summary>
        /// �Ի�����
        /// </summary>
        public List<DialogArgument> Sentences = new List<DialogArgument>();
        protected override void OnPlay()
        {
            Output(0, DialogBoxBase.GetDialogBox(DialogBoxType));
        }

        /// <summary>
        /// ����Ի�
        /// </summary>
        /// <param name="i"></param>
        /// <param name="dialogBox"></param>
        protected void Output(int i, DialogBoxBase dialogBox)
        {
            if(i >= Sentences.Count)
            {
                Finish();
            }
            else
            {
                dialogBox.PlayDialog(Sentences[i], ()=>Output(i + 1, dialogBox));
            }
        }
    }

    /// <summary>
    /// ѡ��ѡ��ڵ�
    /// </summary>
    [Node(NodeAttribute.PortType.Multi, NodeAttribute.PortType.Multi)]
    public class OptionSelectorNode : DialogNodeBase
    {
        public OptionNode SelectedOption;
        protected override void OnPlay()
        {
            var Options = new List<OptionNode>();
            foreach(OptionNode option in Children)
            {
                if (option.Option.isEnable)
                {
                    Options.Add(option);
                }
            }

            (DialogBoxBase.GetDialogBox(DialogBoxType) as OptionalDialogBoxBase).
                ShowOptions(Options.Select(node=>node.Option).ToList(), selected =>
                {
                    SelectedOption = Options[selected];
                    Finish();
                });
        }
        protected override IEnumerable<Type> GetValidDialogBoxType()
        {
#if UNITY_EDITOR
            return Sirenix.OdinInspector.Editor.UnityTypeCacheUtility.GetTypesDerivedFrom(typeof(OptionalDialogBoxBase)).Where(t => !t.IsGenericType && !t.IsAbstract);
#endif
        }
        protected override void OnFinish()
        {
            SelectedOption.Play();
        }
    }

    /// <summary>
    /// ѡ�����ݽڵ�
    /// </summary>
    [NodeCategory("Dialog")]
    [Node(NodeAttribute.PortType.Single, NodeAttribute.PortType.Multi)]
    public class OptionNode : ProcessNode
    {
        [ShowInNodeExtension]
        public ChoiceText Option = new ChoiceText();
        protected override void OnPlay()
        {
            Finish();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using System.Linq;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// �Ի��ڵ����
    /// </summary>
    public abstract class DialogNodeBase : ProcessNode
    {
        /// <summary>
        /// �ٿص���ͼ����
        /// </summary>
        [ValueDropdown("GetValidViewType")]
        public string ViewType;

#if UNITY_EDITOR
        /// <summary>
        /// ��ȡ���ʵ���ͼ����
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<string> GetValidViewType();
#endif
    }

    /// <summary>
    /// �Ի��ڵ�
    /// </summary>
    public class DialogNode : DialogNodeBase
    {
        /// <summary>
        /// �Ի�����
        /// </summary>
        [ListDrawerSettings(AddCopiesLastElement = true, ShowItemCount = true)]
        public List<DialogArgument> Sentences = new List<DialogArgument>();

        /// <summary>
        /// �Ƿ񵱸ýڵ�ִ����Ϻ�رնԻ���
        /// </summary>
        /// <remarks>
        /// ʵ��Ϊ�����ԶԻ�������ã����Ի��򲻱������Ի�������ʱ�Żᱻ�ر�
        /// </remarks>
        public bool CloseDialogOnFinish = false;

        /// <summary>
        /// ��ǰ�ĶԻ�λ��
        /// </summary>
        int _current = -1;

        protected override void OnStart(ProcessNode preNode)
        {
            _current = -1;
            Output();
        }

        void Output()
        {
            if (IsAbort) return;
            if(++_current < Sentences.Count)
                ServiceAP.Instance.DialogManager.PlayDialog(ViewType, Sentences[_current], Output, DialogTree);
            else Finish();
        }

        protected override void OnFinish()
        {
            if(CloseDialogOnFinish) 
                ServiceAP.Instance.DialogManager.DeleteDialogBoxReference(ViewType, DialogTree);
            base.OnFinish();
        }

#if UNITY_EDITOR
        protected override IEnumerable<string> GetValidViewType() =>
            DialogViewConfig.Instance.DialogBoxEnums;
#endif

        protected override void OnAbort()
        {

        }
    }

    /// <summary>
    /// ѡ��ѡ��ڵ�
    /// </summary>
    [NodeCategory("Process/Option")]
    [Node(NodeAttribute.PortType.Multi, NodeAttribute.PortType.Multi)]
    public class OptionSelectorNode : DialogNodeBase
    {
        [SourcePort("SelectedOptionIndex", PortDirection.Output)]
        public int SelectedOptionIndex;

        protected override void OnStart(ProcessNode preNode)
        {
            SelectedOptionIndex = -1;
            var options = from optionNode in Children.OfType<OptionNode>() select optionNode.GetOption();
            ServiceAP.Instance.DialogManager.PlayOption(ViewType, options.ToList(), OnSelected);
        }

        void OnSelected(int i)
        {
            if (IsAbort) return;
            SelectedOptionIndex = i;
            Finish();
        }

#if UNITY_EDITOR
        protected override IEnumerable<string> GetValidViewType() =>
            DialogViewConfig.Instance.OptionViewEnums;
#endif

        protected override void RunSubsequentNode()
        {
            Children[SelectedOptionIndex].Start(this);
        }

        protected override void OnAbort()
        {

        }
    }

    /// <summary>
    /// ѡ�����ݽڵ�
    /// </summary>
    [Node(NodeAttribute.PortType.Single, NodeAttribute.PortType.Multi)]
    [NodeCategory("Process/Option")]
    public class OptionNode : ProcessNode
    {

        [SourcePort("OptionArgument", PortDirection.Input)]
        [SerializeField]
        OptionArgument Option;

        protected override void OnStart(ProcessNode preNode)
        {
            Finish();
        }

        /// <summary>
        /// ����ȡѡ���б�
        /// </summary>
        /// <returns></returns>
        public OptionArgument GetOption()
        {
            PullInputData();
            OnValueUpdate();
            return Option;
        }

        protected override void OnAbort()
        {
            throw new NotImplementedException();
        }
    }
}
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
    [NodeCategory("Dialog")]
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
    [NodeCategory("Dialog/BodyText")]
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

        /// <summary>
        /// �Ƿ��ж�
        /// </summary>
        bool _isAbort = false;

        protected override void OnStart(ProcessNode preNode)
        {
            _current = -1;
            _isAbort = false;
            Output();
        }

        void Output()
        {
            if (_isAbort) return;
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
            _isAbort = true;
        }
    }

    /// <summary>
    /// ѡ��ѡ��ڵ�
    /// </summary>
    [NodeCategory("Dialog/Option")]
    [Node(NodeAttribute.PortType.Multi, NodeAttribute.PortType.Multi)]
    public class OptionSelectorNode : DialogNodeBase
    {
        [Port("SelectedOptionIndex", PortDirection.Output)]
        public int SelectedOptionIndex;

        /// <summary>
        /// �Ƿ��ж�
        /// </summary>
        bool _isAbort = false;

        protected override void OnStart(ProcessNode preNode)
        {
            SelectedOptionIndex = -1;
            _isAbort = false;
            var options = from optionNode in Children.OfType<OptionNode>() select optionNode.GetOption();
            ServiceAP.Instance.DialogManager.PlayOption(ViewType, options.ToList(), OnSelected);
        }

        void OnSelected(int i)
        {
            if (_isAbort) return;
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
    public class OptionNode : ProcessNode
    {

        [Port("OptionArgument", PortDirection.Input)]
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
            InitInputData();
            OnValueUpdate();
            return Option;
        }

        protected override void OnAbort()
        {
            throw new NotImplementedException();
        }
    }
}
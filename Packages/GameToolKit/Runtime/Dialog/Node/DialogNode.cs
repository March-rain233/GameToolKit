using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using System.Linq;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// 对话节点基类
    /// </summary>
    public abstract class DialogNodeBase : ProcessNode
    {
        /// <summary>
        /// 操控的视图类型
        /// </summary>
        [ValueDropdown("GetValidViewType")]
        public string ViewType;

#if UNITY_EDITOR
        /// <summary>
        /// 获取合适的视图类型
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<string> GetValidViewType();
#endif
    }

    /// <summary>
    /// 对话节点
    /// </summary>
    public class DialogNode : DialogNodeBase
    {
        /// <summary>
        /// 对话数据
        /// </summary>
        [ListDrawerSettings(AddCopiesLastElement = true, ShowItemCount = true)]
        public List<DialogArgument> Sentences = new List<DialogArgument>();

        /// <summary>
        /// 是否当该节点执行完毕后关闭对话框
        /// </summary>
        /// <remarks>
        /// 实际为撤销对对话框的引用，当对话框不被其他对话树引用时才会被关闭
        /// </remarks>
        public bool CloseDialogOnFinish = false;

        /// <summary>
        /// 当前的对话位置
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
    /// 选项选择节点
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
    /// 选项数据节点
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
        /// 当获取选项列表
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
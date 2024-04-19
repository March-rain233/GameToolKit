using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// 流程节点基类
    /// </summary>
    [NodeCategory("Process")]
    [NodeColor("#74b9ff")]
    [Node(NodeAttribute.PortType.Multi, NodeAttribute.PortType.Multi)]
    public abstract class ProcessNode : Node
    {
        /// <summary>
        /// 后继节点
        /// </summary>
        [HideInGraphInspector, HideDuplicateReferenceBox, HideReferenceObjectPicker]
        public List<ProcessNode> Children = new List<ProcessNode>();

        /// <summary>
        /// 前驱节点
        /// </summary>
        [HideInGraphInspector, HideDuplicateReferenceBox, HideReferenceObjectPicker]
        public List<ProcessNode> Parents = new List<ProcessNode>();

        public bool IsAbort { get; private set; } = false;

        /// <summary>
        /// 运行
        /// </summary>
        public void Start(ProcessNode preNode)
        {
            DialogTree.RunningNodes.Add(this);
            PullInputData();
            IsAbort = false;
            OnStart(preNode);
        }

        /// <summary>
        /// 当运行时
        /// </summary>
        protected abstract void OnStart(ProcessNode preNode);

        /// <summary>
        /// 中断运行
        /// </summary>
        public void Abort()
        {
            IsAbort = true;
            OnAbort();
        }

        /// <summary>
        /// 当被中断
        /// </summary>
        protected abstract void OnAbort();

        /// <summary>
        /// 结束运行
        /// </summary>
        public void Finish()
        {
            OnFinish();
            IsAbort = false;
            DialogTree.RunningNodes.Remove(this);
            RunSubsequentNode();
        }

        /// <summary>
        /// 当结束运行
        /// </summary>
        protected virtual void OnFinish() { SetDirty(); }

        /// <summary>
        /// 运行后继节点
        /// </summary>
        protected virtual void RunSubsequentNode()
        {
            foreach (var child in Children)
                child.Start(this);
        }

        protected override sealed void OnValueUpdate() { }

        protected override sealed object PullValue(string fieldName) =>
            GetValue(fieldName);

        protected override sealed void PushValue(string fieldName, object value) =>
            SetValue(fieldName, value);
    }

    /// <summary>
    /// 进程起点
    /// </summary>
    [NodeCategory("NULL")]
    [NodeColor("#55efc4")]
    [Node(NodeAttribute.PortType.None, NodeAttribute.PortType.Multi)]
    public sealed class EntryNode : ProcessNode
    {
        protected override void OnAbort()
        {
            
        }

        protected override void OnStart(ProcessNode preNode) =>
            Finish();
    }

    /// <summary>
    /// 进程终点
    /// </summary>
    [NodeCategory("NULL")]
    [NodeColor("#ff7675")]
    [Node(NodeAttribute.PortType.Multi, NodeAttribute.PortType.None)]
    public sealed class ExitNode : ProcessNode
    {
        protected override void OnAbort()
        {

        }

        protected override void OnStart(ProcessNode preNode) =>
            Finish();

        protected override void RunSubsequentNode() =>
            DialogTree.Finish();
    }
}

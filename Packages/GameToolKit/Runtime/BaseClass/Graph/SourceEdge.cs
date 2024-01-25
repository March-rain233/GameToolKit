using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit
{
    public class SourceEdge
    {
        [SerializeField, SerializeReference]
        BaseNode _sourceNode;
        [SerializeField, SerializeReference]
        BaseNode _targetNode;

        /// <summary>
        /// 源节点
        /// </summary>
        public BaseNode SourceNode => _sourceNode;
        /// <summary>
        /// 目标节点
        /// </summary>
        public BaseNode TargetNode => _targetNode;
        /// <summary>
        /// 源数据段名称
        /// </summary>
        [SerializeField]
        public string SourceField;
        /// <summary>
        /// 目标数据段名称
        /// </summary>
        [SerializeField]
        public string TargetField;

        /// <summary>
        /// 脏标记
        /// </summary>
        public bool IsDirty { get; private set; }

        public SourceEdge(BaseNode sourceNode, BaseNode targetNode, string sourceField, string targetField)
        {
            _sourceNode = sourceNode;
            _targetNode = targetNode;
            SourceField = sourceField;
            TargetField = targetField;
            IsDirty = true;
        }

        public void ClearDirty() =>
            IsDirty = false;

        /// <summary>
        /// 注册脏标记回调
        /// </summary>
        internal void RegisterDirtyHandler()
        {
            _sourceNode.OnDirty -= DirtyHandler;
            _sourceNode.OnDirty += DirtyHandler;
        }

        /// <summary>
        /// 移除脏标记回调
        /// </summary>
        internal void UnregisterDirtyHandler()
        {
            _sourceNode.OnDirty -= DirtyHandler;
        }

        private void DirtyHandler()
        {
            if (IsDirty)
            {
                IsDirty = true;
                TargetNode.SetDirty();
            }
        }
    }
}

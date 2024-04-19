using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameToolKit
{
    public class SourceEdge
    {
        [SerializeField, SerializeReference, HideDuplicateReferenceBox, HideReferenceObjectPicker]
        BaseNode _sourceNode;
        [SerializeField, SerializeReference, HideDuplicateReferenceBox, HideReferenceObjectPicker]
        BaseNode _targetNode;

        /// <summary>
        /// Դ�ڵ�
        /// </summary>
        public BaseNode SourceNode => _sourceNode;
        /// <summary>
        /// Ŀ��ڵ�
        /// </summary>
        public BaseNode TargetNode => _targetNode;
        /// <summary>
        /// Դ���ݶ�����
        /// </summary>
        public string SourceField;
        /// <summary>
        /// Ŀ�����ݶ�����
        /// </summary>
        public string TargetField;

        /// <summary>
        /// ����
        /// </summary>
        public bool IsDirty { get; private set; } = true;

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
        /// ע�����ǻص�
        /// </summary>
        internal void RegisterDirtyHandler()
        {
            _sourceNode.OnDirty -= DirtyHandler;
            _sourceNode.OnDirty += DirtyHandler;
        }

        /// <summary>
        /// �Ƴ����ǻص�
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
                TargetNode.SpreadDirtyFlag();
            }
        }
    }
}

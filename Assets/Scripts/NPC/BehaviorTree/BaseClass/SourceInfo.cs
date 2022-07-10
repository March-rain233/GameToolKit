using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// ��Դ����Ϣ
    /// </summary>
    [System.Serializable]
    public struct SourceInfo
    {
        BaseNode _sourceNode;
        string _sourceField;
        string _targetField;

        /// <summary>
        /// Դ�ڵ�
        /// </summary>
        public BaseNode SourceNode { get { return _sourceNode; } }
        /// <summary>
        /// Դ���ݶ�����
        /// </summary>
        public string SourceField { get { return _sourceField; } }
        /// <summary>
        /// Ŀ�����ݶ�����
        /// </summary>
        public string TargetField { get { return _targetField; } }

        public SourceInfo(BaseNode sourceNode, string sourceField, string targetField)
        {
            _sourceNode = sourceNode;
            _sourceField = sourceField;
            _targetField = targetField;
        }

    }
}

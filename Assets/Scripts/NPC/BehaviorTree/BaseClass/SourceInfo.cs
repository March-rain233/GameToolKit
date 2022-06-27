using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// 资源边信息
    /// </summary>
    public struct SourceInfo
    {
        Node _sourceNode;
        string _sourceField;
        string _targetField;

        /// <summary>
        /// 源节点
        /// </summary>
        public Node SourceNode { get { return _sourceNode; } }
        /// <summary>
        /// 源数据段名称
        /// </summary>
        public string SourceField { get { return _sourceField; } }
        /// <summary>
        /// 目标数据段名称
        /// </summary>
        public string TargetField { get { return _targetField; } }

        public SourceInfo(Node sourceNode, string sourceField, string targetField)
        {
            _sourceNode = sourceNode;
            _sourceField = sourceField;
            _targetField = targetField;
        }

    }
}

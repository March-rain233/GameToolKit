using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Text.RegularExpressions;
using System.Linq;

namespace GameToolKit
{
    public class DataFlowGraph<TGraph, TNode> : SerializedScriptableObject 
        where TGraph : DataFlowGraph<TGraph, TNode>
        where TNode : BaseNode
    {
        /// <summary>
        /// �ۼƷ����id����
        /// </summary>
        [SerializeField, HideInInspector]
        private int _idNum = 0;

        /// <summary>
        /// �ڵ��б�
        /// </summary>
        public IReadOnlyList<TNode> Nodes => _nodes;
        [SerializeField, HideDuplicateReferenceBox, HideReferenceObjectPicker]
        protected List<TNode>_nodes = new List<TNode>();

        /// <summary>
        /// �����ֵ�
        /// </summary>
        [OdinSerialize, HideReferenceObjectPicker]
        public GraphBlackboard Blackboard { get; protected set; }

        /// <summary>
        /// ����id���ҽڵ�
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TNode FindNode(int id) =>
            _nodes.Find(n => n.Id == id);

        /// <summary>
        /// �Ƴ��ڵ�
        /// </summary>
        /// <param name="node"></param>
        public virtual void RemoveNode(TNode node)
        {
            _nodes.Remove(node);
            //�Ƴ��ýڵ���ص���Դ��
            foreach (var child in Nodes)
            {
                child.RemoveInputEdge(node);
                child.RemoveOutputEdge(node);
            }
            node.Dispose();
        }

        /// <summary>
        /// �������ʹ����ڵ�
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual TNode CreateNode(Type type)
        {

            //�����ڵ�
            var node = Activator.CreateInstance(type) as TNode;

#if UNITY_EDITOR
            //���ɽڵ���
            string newName;
            var attr = type.GetCustomAttributes(typeof(NodeNameAttribute), true);
            if(attr.Length > 0)
            {
                newName = (attr[0] as NodeNameAttribute).Name;
            }
            else
            {
                newName = type.Name;
                if (type.IsGenericType)
                {
                    newName = newName[..^2];
                }
            }
            //����ڵ�����ʹ�ô���
            int count = _nodes.FindAll(node => Regex.IsMatch(node.Name, @$"{newName}(\(\d+\))?$")).Count;
            if (count > 0)
            {
                newName = newName + $"({count})";
            }
            node.Name = newName;
#endif
            //���ɽڵ��Ψһ��ʶ��
            node.Id = _idNum++;
            _nodes.Add(node);
            return node;
        }

        /// <summary>
        /// �ڼ�����ʼ����ʱ����
        /// </summary>
        public virtual void Init()
        {
            Blackboard.Init();
            foreach(var node in Nodes)
                node.Init();
            foreach(var node in Nodes)
                node.Refresh();
        }

        /// <summary>
        /// ��ֹ����
        /// </summary>
        public virtual void Terminate()
        {
            foreach(var node in Nodes)
                node.Terminate();
        }

        /// <summary>
        /// ����ͼ
        /// </summary>
        /// <returns></returns>
        public virtual TGraph Clone()
        {
            var newGraph = CreateInstance<TGraph>();
            newGraph._idNum = _idNum;
            newGraph.Blackboard = Blackboard.Clone();
            newGraph._nodes = (from node in Nodes select node.Clone() as TNode).ToList();
            //������Դ��
            foreach(var node in Nodes)
            {
                var clone = newGraph.FindNode(node.Id);

                //��������/����
                foreach (var edge in node.InputEdges)
                    clone.AddInputEdge(newGraph.FindNode(edge.SourceNode.Id), edge.SourceField, edge.TargetField);
                foreach (var edge in node.OutputEdges)
                    clone.AddOutputEdge(newGraph.FindNode(edge.TargetNode.Id), edge.SourceField, edge.TargetField);
            }
            return newGraph;
        }

        protected virtual void OnDestroy()
        {
            foreach(var node in Nodes)
                node.Dispose();
        }
    }
}
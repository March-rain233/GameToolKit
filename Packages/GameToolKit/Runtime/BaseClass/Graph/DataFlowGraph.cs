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
        /// 累计分配的id数量
        /// </summary>
        [SerializeField, HideInInspector]
        private int _idNum = 0;

        /// <summary>
        /// 节点列表
        /// </summary>
        public IReadOnlyList<TNode> Nodes => _nodes;
        [SerializeField, HideDuplicateReferenceBox, HideReferenceObjectPicker]
        protected List<TNode>_nodes = new List<TNode>();

        /// <summary>
        /// 变量字典
        /// </summary>
        [OdinSerialize, HideReferenceObjectPicker]
        public GraphBlackboard Blackboard { get; protected set; }

        /// <summary>
        /// 根据id查找节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TNode FindNode(int id) =>
            _nodes.Find(n => n.Id == id);

        /// <summary>
        /// 移除节点
        /// </summary>
        /// <param name="node"></param>
        public virtual void RemoveNode(TNode node)
        {
            _nodes.Remove(node);
            //移除该节点相关的资源边
            foreach (var child in Nodes)
            {
                child.RemoveInputEdge(node);
                child.RemoveOutputEdge(node);
            }
            node.Dispose();
        }

        /// <summary>
        /// 根据类型创建节点
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual TNode CreateNode(Type type)
        {

            //创建节点
            var node = Activator.CreateInstance(type) as TNode;

#if UNITY_EDITOR
            //生成节点名
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
            //计算节点名的使用次数
            int count = _nodes.FindAll(node => Regex.IsMatch(node.Name, @$"{newName}(\(\d+\))?$")).Count;
            if (count > 0)
            {
                newName = newName + $"({count})";
            }
            node.Name = newName;
#endif
            //生成节点的唯一标识符
            node.Id = _idNum++;
            _nodes.Add(node);
            return node;
        }

        /// <summary>
        /// 在即将开始运行时调用
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
        /// 终止运行
        /// </summary>
        public virtual void Terminate()
        {
            foreach(var node in Nodes)
                node.Terminate();
        }

        /// <summary>
        /// 复制图
        /// </summary>
        /// <returns></returns>
        public virtual TGraph Clone()
        {
            var newGraph = CreateInstance<TGraph>();
            newGraph._idNum = _idNum;
            newGraph.Blackboard = Blackboard.Clone();
            newGraph._nodes = (from node in Nodes select node.Clone() as TNode).ToList();
            //连接资源边
            foreach(var node in Nodes)
            {
                var clone = newGraph.FindNode(node.Id);

                //连接输入/出边
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
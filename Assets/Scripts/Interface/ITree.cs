using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Interface
{
    /// <summary>
    /// 可树化接口
    /// </summary>
    public interface ITree
    {
        /// <summary>
        /// 根节点
        /// </summary>
        INode RootNode { get; }

#if UNITY_EDITOR

        /// <summary>
        /// 正确性检查
        /// </summary>
        public void CorrectnessChecking();

        /// <summary>
        /// 获取当前树中的所有节点
        /// </summary>
        /// <returns></returns>
        INode[] GetNodes();

        /// <summary>
        /// 创建指定类型的节点
        /// </summary>
        /// <returns></returns>
        INode CreateNode(System.Type type);

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="node"></param>
        void AddNode(INode node);

        /// <summary>
        /// 从节点列表中移除指定节点
        /// </summary>
        /// <param name="node"></param>
        void RemoveNode(INode node);

        /// <summary>
        /// 连接节点
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        void ConnectNode(INode parent, INode child);

        /// <summary>
        /// 断连节点
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        void DisconnectNode(INode parent, INode child);

        /// <summary>
        /// 生成节点列表
        /// </summary>
        /// <remarks>
        /// 生成节点的继承关系树
        /// </remarks>
        Dictionary<System.Type, string> GetNodeTypeTree();
#endif
    }
}
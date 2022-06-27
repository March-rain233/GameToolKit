using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Interface
{
    /// <summary>
    /// �������ӿ�
    /// </summary>
    public interface ITree
    {
        /// <summary>
        /// ���ڵ�
        /// </summary>
        INode RootNode { get; }

#if UNITY_EDITOR

        /// <summary>
        /// ��ȷ�Լ��
        /// </summary>
        public void CorrectnessChecking();

        /// <summary>
        /// ��ȡ��ǰ���е����нڵ�
        /// </summary>
        /// <returns></returns>
        INode[] GetNodes();

        /// <summary>
        /// ����ָ�����͵Ľڵ�
        /// </summary>
        /// <returns></returns>
        INode CreateNode(System.Type type);

        /// <summary>
        /// ��ӽڵ�
        /// </summary>
        /// <param name="node"></param>
        void AddNode(INode node);

        /// <summary>
        /// �ӽڵ��б����Ƴ�ָ���ڵ�
        /// </summary>
        /// <param name="node"></param>
        void RemoveNode(INode node);

        /// <summary>
        /// ���ӽڵ�
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        void ConnectNode(INode parent, INode child);

        /// <summary>
        /// �����ڵ�
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        void DisconnectNode(INode parent, INode child);

        /// <summary>
        /// ���ɽڵ��б�
        /// </summary>
        /// <remarks>
        /// ���ɽڵ�ļ̳й�ϵ��
        /// </remarks>
        Dictionary<System.Type, string> GetNodeTypeTree();
#endif
    }
}
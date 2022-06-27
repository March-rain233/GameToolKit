using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine;

namespace GameFrame.Interface
{
    public interface INode
    {
#if UNITY_EDITOR
        /// <summary>
        /// �ӿ�����
        /// </summary>
        public enum PortType 
        { 
            /// <summary>
            /// �޽ӿ�
            /// </summary>
            None,
            /// <summary>
            /// ���ӿ�
            /// </summary>
            Single,
            /// <summary>
            /// ��ӿ�
            /// </summary>
            Multi
        }

        /// <summary>
        /// Ψһ��ʶ��
        /// </summary>
        string Guid { get; set; }

        /// <summary>
        /// �ڵ�����
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// �ڵ�����ͼ�ϵ�λ��
        /// </summary>
        Vector2 ViewPosition { get; set; }

        /// <summary>
        /// ��ǰ�ڵ����������
        /// </summary>
        PortType Input { get; }

        /// <summary>
        /// ��ǰ�ڵ���������
        /// </summary>
        PortType Output { get; }

        /// <summary>
        /// ���ڵ��Ӧ��������ɫ�仯
        /// </summary>
        public event System.Action<Color> OnColorChanged;
#endif
        /// <summary>
        /// ��ȡ�ӽڵ�
        /// </summary>
        /// <returns></returns>
        INode[] GetChildren();
    }

}
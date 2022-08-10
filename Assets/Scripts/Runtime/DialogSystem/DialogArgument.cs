using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Dialog
{
    /// <summary>
    /// �Ի���������
    /// </summary>
    public abstract class DialogArgument
    {
    }
    /// <summary>
    /// ��ͨ���ı�����
    /// </summary>
    public class NormalText : DialogArgument 
    {
        /// <summary>
        /// ����
        /// </summary>
        [TextArea]
        public string Text;
    }
    /// <summary>
    /// ѡ�������
    /// </summary>
    public class OptionText : DialogArgument 
    {
        /// <summary>
        /// ����
        /// </summary>
        [TextArea]
        public string Option;
        /// <summary>
        /// ��ѡ���Ƿ�����
        /// </summary>
        public bool IsEnable;
    }
}

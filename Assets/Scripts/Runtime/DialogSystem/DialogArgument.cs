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
        /// <summary>
        /// ����
        /// </summary>
        [TextArea]
        public string Text;
    }
    /// <summary>
    /// ��ͨ���ı�����
    /// </summary>
    public class NormalText : DialogArgument { }
    /// <summary>
    /// ѡ�������
    /// </summary>
    public class ChoiceText : DialogArgument 
    {
        /// <summary>
        /// ��ѡ���Ƿ�����
        /// </summary>
        public bool isEnable;
    }
}

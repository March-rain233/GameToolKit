using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace GameToolKit.Dialog
{
    /// <summary>
    /// �Ի���ӿ�
    /// </summary>
    public interface IDialogBox
    {
        /// <summary>
        /// ���ŶԻ�
        /// </summary>
        /// <param name="argument">�Ի�����</param>
        /// <param name="onDialogEnd">�Ի�ִ����ϻص�</param>
        public void PlayDialog(DialogArgument argument, Action onDialogEnd);

        /// <summary>
        /// �رնԻ���
        /// </summary>
        /// <remarks>
        /// ���Ի����޶Ի���ռ��ʱ����ϵͳ����
        /// </remarks>
        public void CloseDialogBox();
    }
}

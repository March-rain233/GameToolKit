using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// �Զ����������
    /// </summary>
    /// <remarks>
    /// Ϊ�˱�֤�ܱ���ȷ���ƶ���д�Ļ���
    /// </remarks>
    [System.Serializable]
    public abstract class CustomParametersBase
    {
        /// <summary>
        /// �����ںڰ��ϵ�����
        /// </summary>
        protected string _index;

        /// <summary>
        /// �������󶨵ĺڰ�
        /// </summary>
        protected BlackBoard _blackBoard;

#if UNITY_EDITOR
        public abstract void IndexChangeHandler(string newIndex, string oldIndex);
#endif
        protected virtual void RigisterBoard(BlackBoard board)
        {
            if(_blackBoard != null)
            {
                _blackBoard.IndexChanged -= IndexChangeHandler;
            }
            _blackBoard = board;
        }
    }

    /// <summary>
    /// ������ڰ����ͬ���ı�������
    /// </summary>
    /// <remarks>
    /// �̳�ʱ����ʵ��T���ڸ����͵���ʽת��
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public abstract class CustomParameters<T>: CustomParametersBase
    {
        public T Value
        {
            get
            {
                if (string.IsNullOrEmpty(_index)) return _value;
                return (T)_blackBoard[_index];
            }
            set
            {
                if (string.IsNullOrEmpty(_index)) _value = value;
                else _blackBoard[_index] = value;
            }
        }

        /// <summary>
        /// ������������ʱ������ֵ
        /// </summary>
        private T _value;

#if UNITY_EDITOR
        /// <summary>
        /// ͬ����������
        /// </summary>
        /// <param name="index"></param>
        public override void IndexChangeHandler(string newIndex, string oldIndex)
        {
            if(_index == oldIndex) _index = newIndex;
        }
#endif
    }

    /// <summary>
    /// �����ַ���
    /// </summary>
    [System.Serializable]
    public class CustomString : CustomParameters<string>
    {
        public static explicit operator string(CustomString value)
        {
            return value.Value;
        }
    }

    /// <summary>
    /// ��������
    /// </summary>
    [System.Serializable]
    public class CustomInt : CustomParameters<int>
    {
        public static explicit operator int(CustomInt value)
        {
            return value.Value;
        }
    }

    /// <summary>
    /// ������
    /// </summary>
    [System.Serializable]
    public class CustomBool : CustomParameters<bool>
    {
        public static explicit operator bool(CustomBool value)
        {
            return value.Value;
        }
    }

    /// <summary>
    /// �������ȸ�����
    /// </summary>
    public class CustomFloat : CustomParameters<float>
    {
        public static explicit operator float(CustomFloat value)
        {
            return value.Value;
        }
    }

    /// <summary>
    /// ����˫���ȸ�����
    /// </summary>
    [System.Serializable]
    public class CustomDouble : CustomParameters<double>
    {
        public static explicit operator double(CustomDouble value)
        {
            return value.Value;
        }
    }
}
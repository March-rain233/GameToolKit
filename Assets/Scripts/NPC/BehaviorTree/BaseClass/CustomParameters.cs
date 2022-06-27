using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// 自定义变量基类
    /// </summary>
    /// <remarks>
    /// 为了保证能被正确绘制而编写的基类
    /// </remarks>
    [System.Serializable]
    public abstract class CustomParametersBase
    {
        /// <summary>
        /// 变量在黑板上的索引
        /// </summary>
        protected string _index;

        /// <summary>
        /// 变量所绑定的黑板
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
    /// 用于与黑板变量同步的变量类型
    /// </summary>
    /// <remarks>
    /// 继承时建议实现T对于该类型的隐式转换
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
        /// 当不存在索引时变量的值
        /// </summary>
        private T _value;

#if UNITY_EDITOR
        /// <summary>
        /// 同步索引处理
        /// </summary>
        /// <param name="index"></param>
        public override void IndexChangeHandler(string newIndex, string oldIndex)
        {
            if(_index == oldIndex) _index = newIndex;
        }
#endif
    }

    /// <summary>
    /// 共享字符串
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
    /// 共享整型
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
    /// 共享布尔
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
    /// 共享单精度浮点数
    /// </summary>
    public class CustomFloat : CustomParameters<float>
    {
        public static explicit operator float(CustomFloat value)
        {
            return value.Value;
        }
    }

    /// <summary>
    /// 共享双精度浮点数
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
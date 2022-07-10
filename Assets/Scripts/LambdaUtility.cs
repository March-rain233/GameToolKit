using System;

namespace GameFrame.Utility
{
    /// <summary>
    /// lambda����������
    /// </summary>
    public static class LambdaUtility
    {
        /// <summary>
        /// ���ɵݹ���õı��ʽ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Func<T, TResult> Fix<T, TResult>(Func<Func<T, TResult>, Func<T, TResult>> f)
        {
            return x => f(Fix(f))(x);
        }
        /// <summary>
        /// ���ɵݹ���õı��ʽ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Func<T1, T2, TResult> Fix<T1, T2, TResult>(Func<Func<T1, T2, TResult>, Func<T1, T2, TResult>> f)
        {
            return (x, y) => f(Fix(f))(x, y);
        }
    }
}

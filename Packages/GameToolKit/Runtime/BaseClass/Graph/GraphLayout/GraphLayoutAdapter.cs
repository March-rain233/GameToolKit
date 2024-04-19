using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Utility
{
    /// <summary>
    /// �Զ�����ͼ��������
    /// </summary>
    public abstract class GraphLayoutAdapter
    {
        /// <summary>
        /// ���벼�ֵ����нڵ�
        /// </summary>
        public abstract Rect[] Nodes { get; }

        /// <summary>
        /// �ڽӾ���
        /// </summary>
        /// <remarks>0��ʾ�����ӣ���0��ʾ�ߵ����ȼ��������ȼ��ı߲����и��Ӿۺ�</remarks>
        public abstract int [,] EdgeMatrix { get;}

        /// <summary>
        /// ��ȡָ���ڵ�ĺ�̽ڵ�
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual IEnumerable<int> GetDescendant(int index)
        {
            var list = new List<int>();
            for(int i = 0; i < Nodes.Length; i++)
                if(EdgeMatrix[index, i] != 0)
                    list.Add(i);
            return list;
        }

        /// <summary>
        /// ��ȡָ���ڵ��ǰ���ڵ�
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual IEnumerable<int> GetPrecursor(int index)
        {
            var list = new List<int>();
            for (int i = 0; i < Nodes.Length; i++)
                if (EdgeMatrix[i, index] != 0)
                    list.Add(i);
            return list;
        }

        /// <summary>
        /// �ж��Ƿ����Ӵ���ϵ
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public virtual bool IsChild(int parent, int child)
        {
            return EdgeMatrix[parent, child] != 0;
        }

        /// <summary>
        /// �������֣���Ҫʵ�����ýڵ�����
        /// </summary>
        public abstract void Finish();
    }
}

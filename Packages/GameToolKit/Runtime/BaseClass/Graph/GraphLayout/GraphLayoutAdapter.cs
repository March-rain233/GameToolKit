using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.Utility
{
    /// <summary>
    /// 自动布局图表适配器
    /// </summary>
    public abstract class GraphLayoutAdapter
    {
        /// <summary>
        /// 参与布局的所有节点
        /// </summary>
        public abstract Rect[] Nodes { get; }

        /// <summary>
        /// 邻接矩阵
        /// </summary>
        /// <remarks>0表示无连接，非0表示边的优先级，高优先级的边布局中更加聚合</remarks>
        public abstract int [,] EdgeMatrix { get;}

        /// <summary>
        /// 获取指定节点的后继节点
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
        /// 获取指定节点的前驱节点
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
        /// 判断是否是子代关系
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public virtual bool IsChild(int parent, int child)
        {
            return EdgeMatrix[parent, child] != 0;
        }

        /// <summary>
        /// 结束布局，需要实现设置节点数据
        /// </summary>
        public abstract void Finish();
    }
}

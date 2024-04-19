using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GameToolKit.Utility
{
    public class GraphLayoutUtility
    {
        /// <summary>
        /// 多叉树布局
        /// </summary>
        public static void TreeLayout(GraphLayoutAdapter graph, List<int> roots,
            Vector2 oriPosition = default, float marginWidth = 15f, float marginHeight = 2f)
        {
            if (oriPosition == default)
                oriPosition = Vector2.zero;
            bool[] visited = new bool[graph.Nodes.Length];
            Rect[] box = new Rect[graph.Nodes.Length];

            //生成子树的包围框大小
            var rectWalker = LambdaUtility.Fix<int>(f => now =>
            {
                visited[now] = true;
                float childWidth = 0;
                float childHeight = 0;

                foreach (var child in graph.GetDescendant(now).Where(i => !visited[i] 
                && graph.GetPrecursor(i).All(j => graph.EdgeMatrix[j, i] <= graph.EdgeMatrix[now, i])))
                {
                    f(child);
                    childWidth = Mathf.Max(box[child].width, childWidth);
                    childHeight += box[child].height;
                }

                box[now].width = graph.Nodes[now].width + childWidth + 2 * marginWidth;
                box[now].height = Mathf.Max(graph.Nodes[now].height + 2 * marginHeight, childHeight);
            });

            foreach(var root in roots)
                if(!visited[root])
                    rectWalker(root);

            visited = new bool[graph.Nodes.Length];

            //设置子树位置
            var positionWalker = LambdaUtility.Fix<int, Vector2>(f => (now, startPos) =>
            {
                visited[now] = true;
                graph.Nodes[now].position = startPos;

                float heightOffset = -box[now].height / 2;

                foreach (var child in graph.GetDescendant(now).Where(i => !visited[i]
                && graph.GetPrecursor(i).All(j => graph.EdgeMatrix[j, i] <= graph.EdgeMatrix[now, i])))
                {
                    f(child, startPos + new Vector2(graph.Nodes[now].width + 2 * marginWidth, heightOffset + box[child].height / 2));
                    heightOffset += box[child].height;
                }
            });

            float heightOffset = box[roots[0]].height / 2;
            foreach (var root in roots)
            {
                if (visited[root]) continue;
                heightOffset -= box[root].height / 2;
                positionWalker(root, oriPosition + new Vector2(0, heightOffset));
                heightOffset -= box[root].height / 2;
            }

            graph.Finish();
        }
    }
}

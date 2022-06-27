using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    public interface ICreateTree
    {
        /// <summary>
        /// 创建运行树
        /// </summary>
        /// <returns></returns>
        BehaviorTree Create(BehaviorTreeRunner runner);
    }
}
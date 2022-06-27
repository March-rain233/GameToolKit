using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    public interface ICreateTree
    {
        /// <summary>
        /// ����������
        /// </summary>
        /// <returns></returns>
        BehaviorTree Create(BehaviorTreeRunner runner);
    }
}
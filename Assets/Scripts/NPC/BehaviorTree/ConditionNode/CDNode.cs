using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{

    public class CDNode : ConditionNode
    {
        /// <summary>
        /// ���ʱ��
        /// </summary>
        [SerializeField]
        private float _intervalTime;

        [SerializeField]
        private string _CDName;

        private bool _firstTime;

        protected override NodeStatus OnUpdate()
        {
            //if (!runner.Variables.ContainsKey(_CDName))
            //{
            //    runner.Variables.Add(_CDName, new EventCenter.EventArgs());
            //}
            //if (Time.time - runner.Variables[_CDName].Float >= _intervalTime || _firstTime)
            //{
            //    _firstTime = false;
            //    return NodeStatus.Success;
            //}
            return NodeStatus.Failure;
        }
    }
}

//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace EventTree
//{

//    public class FilterNode : Node
//    {
//        /// <summary>
//        /// ��������
//        /// </summary>
//        [SerializeField]
//        private List<string> _filterList;

//        /// <summary>
//        /// �Ƿ�Ϊ������
//        /// </summary>
//        [SerializeField]
//        private bool _isBlack;

//        protected override void SendChildrens(string eventName, EventCenter.EventArgs eventArgs)
//        {
//            if(_filterList.Contains(eventName) ^ _isBlack)
//            {
//                base.SendChildrens(eventName, eventArgs);
//            }
//        }
//    }
//}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using NPC;
//using System;

//namespace Item
//{
//    /// <summary>
//    /// ������
//    /// </summary>
//    public abstract class Weapon : BaseItem
//    {

//        public override ItemType Type
//        {
//            get
//            {
//                return ItemType.Weapon;
//            }
//        }

//        /// <summary>
//        /// ��������
//        /// </summary>
//        /// <param name="user">������</param>
//        /// <param name="callBack">����������Ļص�����</param>
//        public abstract void Attack(BehaviorStateMachine user, Action callBack);

//        /// <summary>
//        /// ��Ϲ���
//        /// </summary>
//        public abstract void Interrupt(BehaviorStateMachine user);

//        /// <summary>
//        /// ����Ƿ���Թ�����
//        /// </summary>
//        /// <param name="user">ʹ����</param>
//        /// <param name="target">Ŀ��</param>
//        /// <returns></returns>
//        public abstract bool Check(BehaviorStateMachine user, BehaviorStateMachine target);
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// ��Ϊ�����нű�
    /// </summary>
    public class BehaviorTreeRunner : SerializedMonoBehaviour
    {
        /// <summary>
        /// ģ�������ò�
        /// </summary>
        public BehaviorTree ModelTreeSlot
        {
            get => _modelTreeSlot;
            set
            {
                RestoreLocalVariables(_modelTreeSlot, value);
            }
        }
        [SetProperty("ModelTreeSlot"),SerializeField,HideInInspector]
        private BehaviorTree _modelTreeSlot;

        /// <summary>
        /// ��ǰ���е���
        /// </summary>
        [HideInInspector]
        public BehaviorTree RunTree { get; private set; }

        /// <summary>
        /// ���ر���ģ���
        /// </summary>
        public Dictionary<string, object> LocalVariables = new Dictionary<string, object>();

#if UNITY_EDITOR
        /// <summary>
        /// ���ñ��ر�����
        /// </summary>
        /// <param name="newTree"></param>
        private void RestoreLocalVariables(BehaviorTree oldTree, BehaviorTree newTree)
        {
            //�������һ����������
            if(oldTree != null)
            {
                oldTree.ModelBlackBoard.LocalValueAdd -= AddLocalValueHandler;
                oldTree.ModelBlackBoard.LocalValueRemove -= RemoveLocalValueHandler;
                oldTree.ModelBlackBoard.LocalValueChanged -= ChangeLocalValueHandler;
            }

            //����һ����
            LocalVariables = new Dictionary<string, object>(newTree.ModelBlackBoard.LocalVariablesModel);
            newTree.ModelBlackBoard.LocalValueAdd += AddLocalValueHandler;
            newTree.ModelBlackBoard.LocalValueRemove += RemoveLocalValueHandler;
            newTree.ModelBlackBoard.LocalValueChanged += ChangeLocalValueHandler;
        }

        /// <summary>
        /// ��������ģ��仯
        /// </summary>
        /// <param name="index"></param>
        /// <param name="before"></param>
        /// <param name="after"></param>
        private void ChangeLocalValueHandler(string index, object before, object after)
        {
            //���ҽ�����Ӧ�ı��������޸ı���ʱ������Ӧ����δ���Զ���ʱ���޸�Ϊ�޸ĺ����
            if (before.Equals(LocalVariables[index])) 
                LocalVariables[index] = after;
        }

        /// <summary>
        /// ��������ģ���������ʱ
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        private void AddLocalValueHandler(string index, object value)
        {
            LocalVariables[index] = value;
        }

        /// <summary>
        /// ��������ģ������Ƴ�ʱ
        /// </summary>
        /// <param name="index"></param>
        private void RemoveLocalValueHandler(string index)
        {
            LocalVariables.Remove(index);
        }
#endif

        private void Awake()
        {
            RunTree = ModelTreeSlot.Create(this);
        }

        private void Update()
        {
            RunTree.Tick();
        }
    }
}

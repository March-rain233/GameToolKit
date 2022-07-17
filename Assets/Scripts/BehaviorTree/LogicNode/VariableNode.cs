using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
namespace GameFrame.Behavior.Tree
{
    [NodeCategory("NULL")]
    public class VariableNode<T> : InputNode<T>
    {
        [OdinSerialize, HideInInspector]
        private string _index = "";

        [OdinSerialize]
        [ValueDropdown("GetValidIndex")]
        public string Index
        {
            get { return _index; }
            set
            {
                var old = _index;
                if (!string.IsNullOrEmpty(_index))
                {
                    _blackboard.UnregisterCallback<IBlackboard.NameChangedEvent>(_index, OnIndexChanged);
                    _blackboard.UnregisterCallback<IBlackboard.ValueChangeEvent>(_index, OnValueChanged);
                }
                _index = value;
                if(_index != null && old != null)//���old����null��˵���������ڱ����л�����Ӧ���ظ���
                {
                    _blackboard.RegisterCallback<IBlackboard.NameChangedEvent>(_index, OnIndexChanged);
                    _blackboard.RegisterCallback<IBlackboard.ValueChangeEvent>(_index, OnValueChanged);
                    Value = _blackboard.GetValue<T>(_index);
                }
            }
        }

        protected virtual IBlackboard _blackboard => BehaviorTree.Blackboard;

        protected override void OnValueUpdate()
        {
            _value = _blackboard.GetValue<T>(_index);
        }

        protected override void OnInit()
        {
            _value = _blackboard.GetValue<T>(_index);
            _blackboard.RegisterCallback<IBlackboard.NameChangedEvent>(_index, OnIndexChanged);
            _blackboard.RegisterCallback<IBlackboard.ValueChangeEvent>(_index, OnValueChanged);
            //todo:���������Ƴ�ʱ���߼�
            base.OnInit();
        }

        protected void OnIndexChanged(IBlackboard.NameChangedEvent e)
        {
            _index = e.Name;
        }

        protected void OnValueChanged(IBlackboard.ValueChangeEvent e)
        {
            Value = (T)e.NewValue;
        }

        protected virtual IEnumerable<string> GetValidIndex()
        {
            var blackboard = _blackboard as BehaviorTree.TreeBlackboard;
            List<string> validIndex = new List<string>();
            var type = typeof(T);
            foreach (var blackboardItem in blackboard.GetLocalVariables())
            {
                if(blackboardItem.Value.TypeOfValue == type)
                {
                    validIndex.Add(blackboardItem.Key);
                }

            }
            foreach (var blackboardItem in blackboard.GetPrototypeVariables())
            {
                if (blackboardItem.Value.TypeOfValue == type)
                {
                    validIndex.Add(blackboardItem.Key);
                }
            }
            return validIndex;
        }
    }
}

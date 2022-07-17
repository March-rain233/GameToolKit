using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
namespace GameFrame.EventProcessor
{
    public class GlobalVariableNode<T> : InputNode<T>
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
                if(LastDataUpdataTime == 0)//当未启动时
                {
                    _index = value;
                    return;
                }
                var old = _index;
                if (!string.IsNullOrEmpty(_index))
                {
                    _blackboard.UnregisterCallback<IBlackboard.NameChangedEvent>(_index, OnIndexChanged);
                    _blackboard.UnregisterCallback<IBlackboard.ValueChangeEvent>(_index, OnValueChanged);
                }
                _index = value;
                if (_index != null && old != null)//如果old等于null，说明现在是在被序列化，不应该重复绑定
                {
                    _blackboard.RegisterCallback<IBlackboard.NameChangedEvent>(_index, OnIndexChanged);
                    _blackboard.RegisterCallback<IBlackboard.ValueChangeEvent>(_index, OnValueChanged);
                    Value = _blackboard.GetValue<T>(_index);
                }
            }
        }

        protected virtual IBlackboard _blackboard => GlobalDatabase.Instance;

        protected override void OnValueUpdate()
        {
            _value = _blackboard.GetValue<T>(_index);
        }

        protected override void OnAttach()
        {
            _value = _blackboard.GetValue<T>(_index);
            _blackboard.RegisterCallback<IBlackboard.NameChangedEvent>(_index, OnIndexChanged);
            _blackboard.RegisterCallback<IBlackboard.ValueChangeEvent>(_index, OnValueChanged);
            //todo:处理当变量移除时的逻辑
        }
        protected override void OnDetach()
        {
            _blackboard.UnregisterCallback<IBlackboard.NameChangedEvent>(_index, OnIndexChanged);
            _blackboard.UnregisterCallback<IBlackboard.ValueChangeEvent>(_index, OnValueChanged);
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
            List<string> validIndex = new List<string>();
            var type = typeof(T);
            foreach (var blackboardItem in GlobalDatabase.Instance.GetVariables())
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

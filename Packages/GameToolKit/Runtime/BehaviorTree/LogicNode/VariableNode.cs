using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using System.Linq;
using DG.Tweening;
namespace GameToolKit.Behavior.Tree
{
    [NodeCategory("Logic/Source/BlackVariableNode")]
    [GenericSelector("BaseValue")]
    public class VariableNode<T> : SourceNode
    {
        [SerializeField]
        [HideInInspector]
        private string _variableId = "";

        [OdinSerialize]
        [ValueDropdown("GetValidIndex", OnlyChangeValueOnConfirm = true)]
        [InfoBox("The index is not contained in the blackboard", InfoMessageType.Warning, "IsNotContainIndex")]
        [DelayedProperty]
        public string VariableId
        {
            get { return _variableId; }
            set
            {
                if (!HasInitialized)
                {
                    _variableId = value;
                    return;
                }
                if (!string.IsNullOrEmpty(_variableId))
                    _blackboard[_variableId].ValueChanged -= OnValueChanged;

                _variableId = value;
                _blackboard[_variableId].ValueChanged += OnValueChanged;
                OnValueChanged();
            }
        }

        [SourcePort("Value", PortDirection.Output)]
        private T _value;

        /// <summary>
        /// 是否不存在该索引
        /// </summary>
        public bool IsNotContainIndex => !_blackboard.GUIDManager.ContainID(_variableId);

        protected GraphBlackboard _blackboard => BehaviorTree.Blackboard;

        protected override void OnInit()
        {
            _blackboard[_variableId].ValueChanged -= OnValueChanged;
            _blackboard[_variableId].ValueChanged += OnValueChanged;
            _value = (T)_blackboard[_variableId].Value;
            base.OnInit();
        }

        void OnValueChanged()
        {
            _value = (T)_blackboard[_variableId].Value;
            SetDirty();
        }

        protected override void OnValueUpdate() { SetDirty(); }

#if UNITY_EDITOR
        /// <summary>
        /// 获取存在的索引列表
        /// </summary>
        /// <returns></returns>
        IEnumerable GetValidIndex()
        {
            ValueDropdownList<string> result = new ValueDropdownList<string>();
            result.AddRange(from pair in _blackboard 
                            where pair.Item3.TypeOfValue == typeof(T)
                            select new ValueDropdownItem<string>(
                                $"{pair.Item1}/{_blackboard.GUIDManager.ID2Name(pair.Item2)}", 
                                pair.Item2));
            return result;
        }
#endif
    }
}

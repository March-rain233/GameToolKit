using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using System.Linq;
using DG.Tweening;
namespace GameToolKit.Behavior.Tree
{
    public class VariableNode<T> : SourceNode
    {
        [SerializeField]
        [HideInInspector]
        private string _id = "";

        [OdinSerialize]
        [ValueDropdown("GetValidIndex", OnlyChangeValueOnConfirm = true)]
        [InfoBox("The index is not contained in the dataset", InfoMessageType.Warning, "IsNotContainIndex")]
        [DelayedProperty]
        public string ID
        {
            get { return _id; }
            set
            {
                if (!HasInitialized)
                {
                    _id = value;
                    return;
                }
                if (!string.IsNullOrEmpty(_id))
                    _blackboard[_id, Domain].ValueChanged -= OnValueChanged;

                _id = value;
                _blackboard[_id, Domain].ValueChanged += OnValueChanged;
                OnValueChanged();
            }
        }

        public Domain Domain;

        [Port("Value", PortDirection.Output)]
        private T _value;

        /// <summary>
        /// 是否不存在该索引
        /// </summary>
        public bool IsNotContainIndex => _blackboard.GUIDManager.ContainID(_id);

        protected TreeBlackboard _blackboard => BehaviorTree.Blackboard;

        protected override void OnInit()
        {
            _blackboard[_id, Domain].ValueChanged += OnValueChanged;
            _value = (T)_blackboard[_id, Domain].Value;
            base.OnInit();
        }

        void OnValueChanged()
        {
            _value = (T)_blackboard[_id, Domain].Value;
            InitOutputData();
        }

        protected override void OnValueUpdate() { }

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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameToolKit
{
    [Serializable]
    public class GraphBlackboard : IBlackboard, IEnumerable<(Domain domin, string id, BlackboardVariable variable)>
    {
        [Serializable]
        public class TreeGUIDManager : IGUIDManager
        {
            [SerializeField]
            Dictionary<string, string> _id2name = new Dictionary<string, string>();
            [SerializeField]
            Dictionary<string, string> _name2id = new Dictionary<string, string>();

            public string ID2Name(string name)
            {
                if (_id2name.TryGetValue(name, out var id)) return id;
                else return ServiceAP.Instance.GlobalBlackboard.GUIDManager.ID2Name(name);
            }

            public string Name2ID(string id)
            {
                if (_name2id.TryGetValue(id, out var name)) return name;
                else return ServiceAP.Instance.GlobalBlackboard.GUIDManager.Name2ID(id);
            }

            public string AddName(string name)
            {
                if (ContainName(name))
                {
                    Debug.Log($"{name} has already exist");
                    return null;
                }
                string guid = Guid.NewGuid().ToString("N");
                _id2name[guid] = name;
                _name2id[name] = guid;

                return guid;
            }

            public bool ContainName(string name) => _name2id.ContainsKey(name) || ServiceAP.Instance.GlobalBlackboard.GUIDManager.ContainName(name);

            public bool RemoveName(string name)
            {
                if (!_name2id.ContainsKey(name)) return ServiceAP.Instance.GlobalBlackboard.GUIDManager.RemoveName(name);
                string uid = _name2id[name];
                _id2name.Remove(uid);
                _name2id.Remove(name);
                return true;
            }

            public bool RemoveID(string id)
            {
                if (!_id2name.ContainsKey(id)) return ServiceAP.Instance.GlobalBlackboard.GUIDManager.RemoveID(id);
                string name = _id2name[id];
                _id2name.Remove(id);
                _name2id.Remove(name);
                return true;
            }

            public bool ChangeName(string oldName, string newName)
            {
                if (!_name2id.ContainsKey(oldName)) return ServiceAP.Instance.GlobalBlackboard.GUIDManager.ChangeName(oldName, newName);
                string id = _name2id[oldName];
                _id2name[id] = newName;
                _name2id.Remove(oldName);
                _name2id[newName] = id;
                return true;
            }

            public bool ContainID(string id) => _id2name.ContainsKey(id) || ServiceAP.Instance.GlobalBlackboard.GUIDManager.ContainID(id);
        }

        [SerializeField, ShowIf("HasPrototypeDomain")]
        Dictionary<string, BlackboardVariable> _prototypeVariables;
        [SerializeField]
        Dictionary<string, BlackboardVariable> _localVariables = new Dictionary<string, BlackboardVariable>();
        [SerializeField, HideReferenceObjectPicker]
        TreeGUIDManager _guidManager = new TreeGUIDManager();

        public event Action<string, BlackboardVariable> VariableAdded;
        public event Action<string, BlackboardVariable, Domain> VariableWithDomainAdded;
        public event Action<string, BlackboardVariable> VariableRemoved;
        public event Action<string, BlackboardVariable, Domain> VariableWithDomainRemoved;

        public IGUIDManager GUIDManager => _guidManager;

        [ReadOnly, HideInInspector]
        public readonly bool HasPrototypeDomain;

        public GraphBlackboard(bool hasPrototypeDomain = true)
        {
            HasPrototypeDomain = hasPrototypeDomain;
            if (hasPrototypeDomain)
                _prototypeVariables = new Dictionary<string, BlackboardVariable>();
        }

        public void Init()
        {
            ServiceAP.Instance.GlobalBlackboard.VariableAdded -= OnGlobalVariableAdded;
            ServiceAP.Instance.GlobalBlackboard.VariableRemoved -= OnGlobalVariableRemoved;
            ServiceAP.Instance.GlobalBlackboard.VariableAdded += OnGlobalVariableAdded;
            ServiceAP.Instance.GlobalBlackboard.VariableRemoved += OnGlobalVariableRemoved;
        }

        ~GraphBlackboard()
        {
            ServiceAP.Instance.GlobalBlackboard.VariableAdded -= OnGlobalVariableAdded;
            ServiceAP.Instance.GlobalBlackboard.VariableRemoved -= OnGlobalVariableRemoved;
        }

        void OnGlobalVariableAdded(string id, BlackboardVariable variable)
        {
            VariableAdded?.Invoke(id, variable);
            VariableWithDomainAdded?.Invoke(id, variable, Domain.Global);
        }

        void OnGlobalVariableRemoved(string id, BlackboardVariable variable)
        {
            VariableRemoved?.Invoke(id, variable);
            VariableWithDomainRemoved?.Invoke(id, variable, Domain.Global);
        }


        public BlackboardVariable this[string id]
        {
            get
            {
                if(_localVariables.TryGetValue(id, out var variable)) return variable;
                else if(HasPrototypeDomain && _prototypeVariables.TryGetValue(id, out variable)) return variable;
                else return ServiceAP.Instance.GlobalBlackboard[id];
            }
        }

        void IBlackboard.AddVariable(string name, BlackboardVariable value) =>
            AddVariable(name, value, Domain.Local);


        bool IBlackboard.RemoveVariable(string id)
        {
            if (_localVariables.ContainsKey(id)) return RemoveVariable(id, Domain.Local);
            else if (HasPrototypeDomain && _prototypeVariables.ContainsKey(id)) return RemoveVariable(id, Domain.Prototype);
            else return RemoveVariable(id, Domain.Global);
        }

        public BlackboardVariable this[string id, Domain domain] => domain switch
        {
            Domain.Global => ServiceAP.Instance.GlobalBlackboard[id],
            Domain.Local when HasPrototypeDomain => _localVariables[id],
            Domain.Prototype => _prototypeVariables[id],
            _ => null
        };

        /// <summary>
        /// 增加变量
        /// </summary>
        /// <param name="name">变量名</param>
        /// <param name="value">变量</param>
        /// <param name="domain">作用域</param>
        public void AddVariable(string name, BlackboardVariable value, Domain domain)
        {
            switch(domain)
            {
                case Domain.Local: 
                    _localVariables[_guidManager.AddName(name)] = value;
                    VariableAdded?.Invoke(_guidManager.Name2ID(name), value);
                    VariableWithDomainAdded?.Invoke(_guidManager.Name2ID(name), value, domain);
                    return;
                case Domain.Prototype when HasPrototypeDomain: 
                    _prototypeVariables[_guidManager.AddName(name)] = value;
                    VariableAdded?.Invoke(_guidManager.Name2ID(name), value);
                    VariableWithDomainAdded?.Invoke(_guidManager.Name2ID(name), value, domain);
                    return;
                case Domain.Global: ServiceAP.Instance.GlobalBlackboard.AddVariable(name, value);
                    return;
                default:
                    Debug.LogWarning("Domain error");
                    return;
            };
        }

        /// <summary>
        /// 移除变量
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="domain">作用域</param>
        public bool RemoveVariable(string id, Domain domain)
        {
            switch (domain)
            {
                case Domain.Local:
                    if(_localVariables.Remove(id, out var variable)){
                        GUIDManager.RemoveID(id);
                        VariableRemoved?.Invoke(id, variable);
                        VariableWithDomainRemoved?.Invoke(id, variable, domain);
                        return true;
                    }
                    return false;
                case Domain.Prototype when HasPrototypeDomain:
                    if (_prototypeVariables.Remove(id, out variable))
                    {
                        GUIDManager.RemoveID(id);
                        VariableRemoved?.Invoke(id, variable);
                        VariableWithDomainRemoved?.Invoke(id, variable, domain);
                        return true;
                    }
                    return false;
                case Domain.Global:
                    return ServiceAP.Instance.GlobalBlackboard.RemoveVariable(id);
                default:
                    return false;
            };
        }

        /// <summary>
        /// 更改变量定义域
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newDomain"></param>
        public void ChangeVariableDomain(string id, Domain newDomain)
        {
            if (newDomain == Domain.Global)
            {
                Debug.LogWarning("You should add global variable in inspector");
                return;
            }
            if(!HasPrototypeDomain && newDomain == Domain.Prototype)
            {
                Debug.LogWarning("There is not prototype domain");
                return;
            }
            BlackboardVariable variable;
            if (_localVariables.Remove(id, out variable))
            {
                VariableRemoved?.Invoke(id, variable);
                VariableWithDomainRemoved?.Invoke(id, variable, Domain.Local);
            }
            else if (HasPrototypeDomain && _prototypeVariables.Remove(id, out variable))
            {
                VariableRemoved?.Invoke(id, variable);
                VariableWithDomainRemoved?.Invoke(id, variable, Domain.Prototype);
            }
            else
            {
                Debug.LogWarning($"Variable:{id} didn't exist");
                return;
            }
            switch (newDomain)
            {
                case Domain.Local:
                    _localVariables[id] = variable;
                    VariableAdded?.Invoke(id, variable);
                    VariableWithDomainAdded?.Invoke(id, variable, Domain.Local);
                    break;
                case Domain.Prototype:
                    _prototypeVariables[id] = variable;
                    VariableAdded?.Invoke(id, variable);
                    VariableWithDomainAdded?.Invoke(id, variable, Domain.Prototype);
                    break;
            }
            
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public GraphBlackboard Clone()
        {
            var newObj = (GraphBlackboard)MemberwiseClone();
            newObj._localVariables = new Dictionary<string, BlackboardVariable>();
            //生成本地变量副本
            foreach (var (key, value) in _localVariables)
                newObj._localVariables.Add(key, value.Clone());
            if (HasPrototypeDomain)
                newObj._prototypeVariables = _prototypeVariables;
            return newObj;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<(Domain, string, BlackboardVariable)>)this).GetEnumerator();
        }

        IEnumerator<(Domain domin, string id, BlackboardVariable variable)> IEnumerable<(Domain domin, string id, BlackboardVariable variable)>.GetEnumerator()
        {
            foreach (var (id, value) in _localVariables)
                yield return (Domain.Local, id, value);
            if(HasPrototypeDomain)
                foreach (var (id, value) in _prototypeVariables)
                    yield return (Domain.Prototype, id, value);
            foreach (var (id, value) in ServiceAP.Instance.GlobalBlackboard)
                yield return (Domain.Global, id, value);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameToolKit.Behavior.Tree
{
    public class TreeBlackboard : IBlackboard, IEnumerable<(Domain, string, BlackboardVariable)>
    {
        [Serializable]
        public class TreeGUIDManager : IGUIDManager
        {
            Dictionary<string, string> _id2name = new Dictionary<string, string>();
            Dictionary<string, string> _name2id = new Dictionary<string, string>();

            public string ID2Name(string name)
            {
                if(_id2name.TryGetValue(name, out var id)) return id;
                else return GlobalGUIDManager.Instance.ID2Name(name);
            }

            public string Name2ID(string id)
            {
                if(_name2id.TryGetValue(id, out var name)) return name;
                else return GlobalGUIDManager.Instance.Name2ID(id);
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

            public bool ContainName(string name) => _name2id.ContainsKey(name) || GlobalGUIDManager.Instance.ContainName(name);

            public bool RemoveName(string name)
            {
                if (!_name2id.ContainsKey(name)) return GlobalGUIDManager.Instance.RemoveName(name);
                string uid = _name2id[name];
                _id2name.Remove(uid);
                _name2id.Remove(name);
                return true;
            }

            public bool RemoveID(string id)
            {
                if (!_id2name.ContainsKey(id)) return GlobalGUIDManager.Instance.RemoveID(id);
                string name = _name2id[id];
                _id2name.Remove(id);
                _name2id.Remove(name);
                return true;
            }

            public bool ChangeName(string oldName, string newName)
            {
                if (!_name2id.ContainsKey(oldName)) return GlobalGUIDManager.Instance.ChangeName(oldName, newName);
                string uid = _name2id[oldName];
                _id2name[uid] = newName;
                _name2id.Remove(oldName);
                _name2id[newName] = uid;
                return true;
            }

            public bool ContainID(string id) => _id2name.ContainsKey(id) || GlobalGUIDManager.Instance.ContainID(id);
        }

        Dictionary<string, BlackboardVariable> _treeVariables = new Dictionary<string, BlackboardVariable>();
        Dictionary<string, BlackboardVariable> _localVariables = new Dictionary<string, BlackboardVariable>();

        TreeGUIDManager _guidManager = new TreeGUIDManager();

        BlackboardVariable IBlackboard.this[string id]
        {
            get
            {
                if(_localVariables.TryGetValue(id, out var variable)) return variable;
                else if(_treeVariables.TryGetValue(id, out variable)) return variable;
                else return ServiceAP.Instance.GlobalBlackboard[id];
            }
        }

        public IGUIDManager GUIDManager => _guidManager;

        void IBlackboard.AddVariable(string name, BlackboardVariable value) => _localVariables[_guidManager.AddName(name)] = value;

        void IBlackboard.RemoveVariable(string id)
        {
            if (_localVariables.Remove(id))  _guidManager.RemoveID(id);
            else if (_treeVariables.Remove(id)) _guidManager.RemoveID(id);
            else ServiceAP.Instance.GlobalBlackboard.RemoveVariable(id);
        }

        public BlackboardVariable this[string id, Domain domain] => domain switch
        {
            Domain.Global => ServiceAP.Instance.GlobalBlackboard[id],
            Domain.Local => _localVariables[id],
            Domain.Tree => _treeVariables[id],
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
                case Domain.Local: _localVariables[_guidManager.AddName(name)] = value; 
                    return;
                case Domain.Tree: _treeVariables[_guidManager.AddName(name)] = value;
                    return;
                case Domain.Global: ServiceAP.Instance.GlobalBlackboard.AddVariable(name, value);
                    return;
            };
        }

        /// <summary>
        /// 移除变量
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="domain">作用域</param>
        public void RemoveVariable(string id, Domain domain)
        {
            switch (domain)
            {
                case Domain.Local:
                    _localVariables.Remove(id);
                    GUIDManager.RemoveID(id);
                    return;
                case Domain.Tree:
                    _treeVariables.Remove(id);
                    GUIDManager.RemoveID(id);
                    return;
                case Domain.Global:
                    ServiceAP.Instance.GlobalBlackboard.RemoveVariable(id);
                    return;
            };
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public TreeBlackboard Clone()
        {
            var newObj = (TreeBlackboard)MemberwiseClone();
            newObj._localVariables = new Dictionary<string, BlackboardVariable>();
            //生成本地变量副本
            foreach(var(key, value) in _localVariables) 
                newObj._localVariables.Add(key, value.Clone());
            return newObj;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<(Domain, string, BlackboardVariable)>)this).GetEnumerator();
        }

        IEnumerator<(Domain, string, BlackboardVariable)> IEnumerable<(Domain, string, BlackboardVariable)>.GetEnumerator()
        {
            foreach (var (id, value) in _localVariables)
                yield return (Domain.Local, id, value);
            foreach (var (id, value) in _treeVariables)
                yield return (Domain.Tree, id, value);
            foreach (var (id, value) in GlobalBlackboard.Instance)
                yield return (Domain.Global, id, value);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Sirenix.Serialization;

namespace GameToolKit
{
    public class GlobalBlackboard : SingletonSO<GlobalBlackboard>, 
        IBlackboard, IEnumerable<KeyValuePair<string, BlackboardVariable>>
    {
        /// <summary>
        /// 变量库
        /// </summary>
        [OdinSerialize]
        Dictionary<string, BlackboardVariable> _variables = new Dictionary<string, BlackboardVariable>();

        public BlackboardVariable this[string id] => _variables[id];

        [OdinSerialize]
        public IGUIDManager GUIDManager { get; private set; } = new GlobalGUIDManager();

        public event Action<string, BlackboardVariable> VariableAdded;
        public event Action<string, BlackboardVariable> VariableRemoved;

        public void AddVariable(string name, BlackboardVariable value)
        {
            var id = GUIDManager.AddName(name);
            _variables[id] = value;
            VariableAdded?.Invoke(id, value);
        }

        public IEnumerator<KeyValuePair<string, BlackboardVariable>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, BlackboardVariable>>)_variables).GetEnumerator();
        }

        public bool RemoveVariable(string id)
        {
            if(_variables.Remove(id, out var variable))
            {
                GUIDManager.RemoveID(id);
                VariableRemoved?.Invoke(id, variable);
                return true;
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_variables).GetEnumerator();
        }
    }
}

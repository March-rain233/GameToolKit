using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameToolKit
{
    public class GlobalBlackboard : ScriptableSingleton<GlobalBlackboard>, 
        IBlackboard, IEnumerable<KeyValuePair<string, BlackboardVariable>>
    {
        /// <summary>
        /// 变量库
        /// </summary>
        Dictionary<string, BlackboardVariable> _variables = new Dictionary<string, BlackboardVariable>();
        public BlackboardVariable this[string id] => _variables[id];

        public IGUIDManager GUIDManager => GlobalGUIDManager.Instance;

        public void AddVariable(string name, BlackboardVariable value) => _variables[GUIDManager.AddName(name)] = value;

        public IEnumerator<KeyValuePair<string, BlackboardVariable>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, BlackboardVariable>>)_variables).GetEnumerator();
        }

        public void RemoveVariable(string id)
        {
            _variables.Remove(id);
            GUIDManager.RemoveID(id);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_variables).GetEnumerator();
        }
    }
}

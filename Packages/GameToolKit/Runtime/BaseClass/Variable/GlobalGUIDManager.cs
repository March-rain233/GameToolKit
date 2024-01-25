using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameToolKit
{
    /// <summary>
    /// UID管理器
    /// </summary>
    /// <remarks>
    /// 用于转换uid与名称
    /// </remarks>
    public class GlobalGUIDManager : ScriptableSingleton<GlobalGUIDManager>, IGUIDManager
    {
        Dictionary<string, string> _id2name = new Dictionary<string, string>();
        Dictionary<string, string> _name2id = new Dictionary<string, string>();

        public string ID2Name(string name) => _id2name[name];

        public string Name2ID(string id) => _name2id[id];

        public string AddName(string name)
        {
            if (ContainName(name))
            {
                Debug.Log($"{name} has already exist");
                return null;
            }
#if UNITY_EDITOR
            Undo.RecordObject(this, $"add name {name}");
#endif

            string guid = Guid.NewGuid().ToString("N");
            _id2name[guid] = name;
            _name2id[name] = guid;

            return guid;
        }

        public bool ContainName(string name) => _name2id.ContainsKey(name);

        public bool RemoveName(string name)
        {
            if(!_name2id.ContainsKey(name)) return false;

            string uid = _name2id[name];
#if UNITY_EDITOR
            Undo.RecordObject(this, $"remove name {name}");
#endif
            _id2name.Remove(uid);
            _name2id.Remove(name);
            return true;
        }

        public bool RemoveID(string id)
        {
            if (!_id2name.ContainsKey(id)) return false;

            string name = _name2id[id];
#if UNITY_EDITOR
            Undo.RecordObject(this, $"remove name {name}");
#endif
            _id2name.Remove(id);
            _name2id.Remove(name);
            return true;
        }

        public bool ChangeName(string oldName, string newName)
        {
            if(!_name2id.ContainsKey(oldName)) return false;
            string uid = _name2id[oldName];
#if UNITY_EDITOR
            Undo.RecordObject(this, $"change name {oldName} to {newName}");
#endif
            _id2name[uid] = newName;
            _name2id.Remove(oldName);
            _name2id[newName] = uid;
            return true;
        }

        public bool ContainID(string id) => _id2name.ContainsKey(id);
    }
}

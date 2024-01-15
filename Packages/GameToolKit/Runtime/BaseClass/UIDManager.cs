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
    public class UIDManager : ScriptableSingleton<UIDManager>
    {
        Dictionary<string, string> _uid2name;
        Dictionary<string, string> _name2uid;
        UIDManager()
        {
            //解析文件
            _name2uid = new Dictionary<string, string>();
            _uid2name = new Dictionary<string, string>();
        }

        /// <summary>
        /// UID转名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string UID2Name(string name)
        {
            return _uid2name[name];
        }

        /// <summary>
        /// 名称转UID
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public string Name2UID(string uid)
        {
            return _name2uid[uid];
        }

        /// <summary>
        /// 增加名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns>生成的guid</returns>
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
            _uid2name[guid] = name;
            _name2uid[name] = guid;

            return guid;
        }

        /// <summary>
        /// 名称是否已占用
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ContainName(string name)
        {
            return _name2uid.ContainsKey(name);
        }

        /// <summary>
        /// 移除名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveName(string name)
        {
            if(_name2uid.ContainsKey(name)) return false;

            string uid = _name2uid[name];
#if UNITY_EDITOR
            Undo.RecordObject(this, $"remove name {name}");
#endif
            _uid2name.Remove(uid);
            _name2uid.Remove(name);
            return true;
        }

        /// <summary>
        /// 移除uid
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool RemoveUID(string uid)
        {
            if (_uid2name.ContainsKey(uid)) return false;

            string name = _name2uid[uid];
#if UNITY_EDITOR
            Undo.RecordObject(this, $"remove name {name}");
#endif
            _uid2name.Remove(uid);
            _name2uid.Remove(name);
            return true;
        }

        /// <summary>
        /// 更改名称
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public bool ChangeName(string oldName, string newName)
        {
            if(!_name2uid.ContainsKey(oldName)) return false;
            string uid = _name2uid[oldName];
#if UNITY_EDITOR
            Undo.RecordObject(this, $"change name {oldName} to {newName}");
#endif
            _uid2name[uid] = newName;
            _name2uid.Remove(oldName);
            _name2uid[newName] = uid;
            return true;
        }
    }
}

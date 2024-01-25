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
    public interface IGUIDManager
    {
        /// <summary>
        /// ID转名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string ID2Name(string name);

        /// <summary>
        /// 名称转ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Name2ID(string id);

        /// <summary>
        /// 增加名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns>生成的id</returns>
        public string AddName(string name);

        /// <summary>
        /// 名称是否已占用
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ContainName(string name);

        /// <summary>
        /// ID是否已存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ContainID(string id);

        /// <summary>
        /// 移除名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveName(string name);

        /// <summary>
        /// 移除id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RemoveID(string id);

        /// <summary>
        /// 更改名称
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public bool ChangeName(string oldName, string newName);
    }
}

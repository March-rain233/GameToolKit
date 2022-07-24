using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GameFrame
{
    /// <summary>
    /// 数据集管理器
    /// </summary>
    [CreateAssetMenu(fileName = "Data Set Manager", menuName = "Data Set/Data Set Manager")]
    public class DataSetManager : SerializedScriptableObject
    {
        public static DataSetManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var assets = Resources.FindObjectsOfTypeAll<DataSetManager>();
                    if(assets.Length == 0)
                    {
                        Debug.LogError("Could not find the Asset");
                    }
                    else if(assets.Length > 1)
                    {
                        Debug.LogError("Multiple Assets exist");
                    }
                    else
                    {
                        _instance = assets[0];
                    }
                }
                return _instance;
            }
        }
        static DataSetManager _instance;

        [OdinSerialize]

        private Dictionary<string, DataSet> _dataSets = new Dictionary<string, DataSet>();
        /// <summary>
        /// 数据集名称列表
        /// </summary>
        public IEnumerable<string> ExistDataSets => _dataSets.Keys;

        /// <summary>
        /// 添加数据集
        /// </summary>
        public void AddDataSet(string name, DataSet dataSet)
        {
            _dataSets.Add(name, dataSet);
        }
        /// <summary>
        /// 移除数据集
        /// </summary>
        /// <param name="name"></param>
        public void RemoveDataSet(string name)
        {
            _dataSets.Remove(name);
        }
        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public bool TryGetDataSet(string name, out DataSet dataSet)
        {
            return _dataSets.TryGetValue(name, out dataSet);
        }
        public DataSet GetDataSet(string name)
        {
            return _dataSets[name];
        }
    }
}
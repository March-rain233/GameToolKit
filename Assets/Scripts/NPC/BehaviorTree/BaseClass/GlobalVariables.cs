using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "全局变量表", menuName ="行为树/全局变量表")]
public class GlobalVariables : ScriptableObject
{
    public static GlobalVariables Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = Resources.Load<GlobalVariables>("");
                if (!_instance)
                {
#if UNITY_EDITOR
                    _instance = CreateInstance<GlobalVariables>();
                    string filePath = "Resources/BehaviorTree/Config";
                    string temp = "Assets";
                    foreach(var folder in filePath.Split('/'))
                    {
                        if (!AssetDatabase.IsValidFolder(temp + '/' + folder))
                        {
                            AssetDatabase.CreateFolder(temp, folder);
                        }
                        temp += '/' + folder;
                    }
                    AssetDatabase.CreateAsset(_instance, filePath);
                    AssetDatabase.SaveAssets();
                    Debug.Log($"已在{filePath}创建全局变量表");
#else
                    Debug.LogWarning("全局变量表缺失");
#endif
                }
                else
                {
                    Debug.Log("全局变量表成功加载");
                }
            }
            return _instance;
        }
    }
    private static GlobalVariables _instance;
    
    public Dictionary<string, object> Variables;

    private void OnEnable()
    {
        if(_instance != null && _instance != this)
        {
            Debug.LogWarning("出现了额外的全局变量表");
            return;
        }
        _instance = this;
    }
#if UNITY_EDITOR

    /// <summary>
    /// 当变量模板增加时
    /// </summary>
    internal event System.Action<string, object> ModelValueAdd;
    /// <summary>
    /// 当变量模板减少时
    /// </summary>
    internal event System.Action<string> ModelValueRemove;
    /// <summary>
    /// 当变量索引改变时
    /// </summary>
    internal event System.Action<string, string> IndexChanged;

    /// <summary>
    /// 获取对应类型的已定义的变量列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public List<KeyValuePair<string, object>> GetDefineList(System.Type type)
    {
        var list = new List<KeyValuePair<string, object>>();
        foreach (var item in Variables)
        {
            if (type.IsEquivalentTo(item.Value.GetType()))
                list.Add(new KeyValuePair<string, object>(item.Key, item.Value));
        }
        return list;
    }

    /// <summary>
    /// 获取全部已定义的变量列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public List<KeyValuePair<string, object>> GetAllDefineList()
    {
        var list = new List<KeyValuePair<string, object>>();
        foreach (var item in Variables)
        {
            list.Add(new KeyValuePair<string, object>(item.Key, item.Value));
        }
        return list;
    }
#endif
}

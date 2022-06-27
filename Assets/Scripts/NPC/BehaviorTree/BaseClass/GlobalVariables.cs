using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "ȫ�ֱ�����", menuName ="��Ϊ��/ȫ�ֱ�����")]
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
                    Debug.Log($"����{filePath}����ȫ�ֱ�����");
#else
                    Debug.LogWarning("ȫ�ֱ�����ȱʧ");
#endif
                }
                else
                {
                    Debug.Log("ȫ�ֱ�����ɹ�����");
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
            Debug.LogWarning("�����˶����ȫ�ֱ�����");
            return;
        }
        _instance = this;
    }
#if UNITY_EDITOR

    /// <summary>
    /// ������ģ������ʱ
    /// </summary>
    internal event System.Action<string, object> ModelValueAdd;
    /// <summary>
    /// ������ģ�����ʱ
    /// </summary>
    internal event System.Action<string> ModelValueRemove;
    /// <summary>
    /// �����������ı�ʱ
    /// </summary>
    internal event System.Action<string, string> IndexChanged;

    /// <summary>
    /// ��ȡ��Ӧ���͵��Ѷ���ı����б�
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
    /// ��ȡȫ���Ѷ���ı����б�
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

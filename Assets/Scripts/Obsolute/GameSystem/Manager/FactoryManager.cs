using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;
using Map;
using NPC;
using Save;

/// <summary>
/// ��Ϸ���ݴ洢�����Լ���Ϸ����ʱԭʼ���ݻ�ȡ
/// </summary>
public class FactoryManager
{
    private Dictionary<ObjectType, ObjectFactoryWithPool> _factorys = new Dictionary<ObjectType, ObjectFactoryWithPool>();

    public T LoadResource<T>(ObjectType type, string name) where T : Object
    {
        Debug.Log($"��������{type} : {name}");
        return Resources.Load<T>(GameManager.Instance.GameConfig.
            PathConfig.Paths[type].PathDic[name]);
    }

    /// <summary>
    /// ��������Ϊ�ı��ļ�����
    /// </summary>
    /// <typeparam name="T">�洢���ݵ���������</typeparam>
    /// <param name="data">�洢������</param>
    /// <param name="filePath">�ļ�·������ʽӦΪ"/�ļ���/�ļ���</param>
    /// <param name="mode">
    /// <para>mode1 ·��ΪPersistentDataPath(Ĭ��)���ڴ�ȡ�浵�ļ�</para>
    /// <para>mode2 ·��ΪstreamingAssetsPath���ڴ�ȡ�����ֻ�����б�����</para>
    /// </param>
    public void Save<T>([SerializeField] T data, string filePath, int mode = 1)
    {
        ///��������ļ���ַ���д���
        switch (mode)
        {
            case 1:
                filePath = Application.persistentDataPath + @filePath;
                break;
            case 2:
                filePath = Application.streamingAssetsPath + @filePath;
                break;
            default:
                return;
        }
        string s = filePath.Substring(0, filePath.LastIndexOf('/'));

        Directory.CreateDirectory(s);
        StreamWriter file = new StreamWriter(filePath);
        string json = JsonUtility.ToJson(data, true);
        Debug.Log(json);
        file.Write(json);
        file.Close();
    }
    /// <summary>
    /// ��������
    /// </summary>
    /// <typeparam name="T">�������ݵ���������</typeparam>
    /// <param name="data">�������ݵĴ���λ��</param>
    /// <param name="filePath">�ļ�·������ʽӦΪ"/�ļ���/�ļ���</param>
    /// <param name="mode">
    /// <para>mode1 ·��ΪPersistentDataPath(Ĭ��)���ڴ�ȡ�浵�ļ�</para>
    /// <para>mode2 ·��ΪstreamingAssetsPath���ڴ�ȡ�����ֻ�����б�����</para>
    /// </param>
    /// <returns>�Ƿ�ɹ���ȡ</returns>
    public bool Load<T>(out T data, string filePath, int mode = 1)
    {
        ///��������ļ���ַ���д���
        switch (mode)
        {
            case 1:
                filePath = Application.persistentDataPath + @filePath;
                break;
            case 2:
                filePath = Application.streamingAssetsPath + @filePath;
                break;
            default:
                data = default(T);
                return false;
        }

        if (!File.Exists(filePath))
        {
            data = default(T);
            return false;
        }
        StreamReader file = new StreamReader(filePath);
        string json = file.ReadToEnd();
        data = default(T);
        JsonUtility.FromJsonOverwrite(json, data);
        return true;
    }

    /// <summary>
    /// ���ڱ����ֵ�
    /// </summary>
    /// <see cref="Save{T}(T, string, int)"/>
    public void SaveDic<TKey, TVal>([SerializeField] Dictionary<TKey,TVal> data, string filePath, int mode = 1)
    {
        Save(new MyTools.Serialization<TKey, TVal>(data), filePath, mode);
    }

    /// <summary>
    /// ���ڶ�ȡ�ֵ�
    /// </summary>
    /// <see cref="Load{T}(out T, string, int)"/>
    public bool LoadDic<TKey, TVal>(out Dictionary<TKey, TVal> data, string filePath, int mode = 1)
    {
        bool t = Load(out MyTools.Serialization<TKey, TVal> temp, filePath);
        if (!t)
        {
            data = new Dictionary<TKey, TVal>();
            return false;
        }
        temp.OnAfterDeserialize();
        data = temp.ToDictionary();
        return t;
    }

    /// <summary>
    /// ��ӹ���
    /// </summary>
    /// <param name="type"></param>
    /// <param name="factory"></param>
    public void AddFactory(ObjectType type, ObjectFactoryWithPool factory)
    {
        _factorys.Add(type, factory);
    }

    /// <summary>
    /// �Ƴ�����
    /// </summary>
    /// <param name="type"></param>
    public void RemoveFactory(ObjectType type)
    {
        _factorys.Remove(type);
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public IProduct Create(ObjectType type, string name)
    {
        return _factorys[type].Create(name);
    }
}

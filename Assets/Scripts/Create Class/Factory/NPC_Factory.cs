//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using NPC;
//using Newtonsoft.Json;

///// <summary>
///// ��ɫ����
///// </summary>
//public class NPC_Factory
//{
//    /// <summary>
//    /// ��������
//    /// </summary>
//    private Dictionary<string, Breed> _breedPool;

//    /// <summary>
//    /// Ԥ��������
//    /// </summary>
//    private Dictionary<string, GameObject> _prefabsPool;

//    /// <summary>
//    /// ������ļ���ַ
//    /// </summary>
//    private Save.PathConfig _breedPath;

//    public NPC_Factory()
//    {
//        string path = "config/PathConfig";
//        conf
//    }

//    /// <summary>
//    /// ��ȡ����
//    /// </summary>
//    /// <param name="name"></param>
//    /// <returns></returns>
//    public Breed GetBreed(string name)
//    {
//        //����������Ƿ��Ѵ��ڶ���
//        if(_breedPool.TryGetValue(name, out Breed res))
//        {
//            return res;
//        }

//        if(!_breedPath.TryGetValue(name, out string path))
//        {
//            Debug.LogWarning($"�����ļ���{name} ȱʧ");
//            return default;
//        }

//        res = Resources.Load<Breed>(path);

//        //�������������
//        PushBreed(res);
//        return res;
//    }

//    /// <summary>
//    /// ��ȡ��ɫ
//    /// </summary>
//    public NPC_Model GetNpc(Save.NPC_Save save)
//    {
//        //����Ԥ���岢�������

//    }

//    private void PushBreed(Breed breed)
//    {
//        _breedPool[breed.Name] = breed;
//    }
//}

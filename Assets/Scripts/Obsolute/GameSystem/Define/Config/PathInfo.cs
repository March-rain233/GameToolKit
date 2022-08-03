using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "·������", menuName = "�����ļ�/·������")]
    public class PathInfo : SerializedScriptableObject
    {
        /// <summary>
        /// ֧�ֵ��ļ���׺��
        /// </summary>
        [SerializeField, LabelText("��׺���б�")]
        public static readonly string[] _extensionList =
        {
            "asset",
            "prefab",
            "png",
            "jpg",
            "mp3",
            "ogg",
            "wav"
        };

        [SerializeField]
        public Dictionary<string, string> PathDic;
        /// <summary>
        /// �ļ�����Ŀ¼
        /// </summary>
        [LabelText("�ļ�����Ŀ¼")]
        public string CurPath;

        /// <summary>
        /// ��Ŀ¼�������ļ����¶���
        /// </summary>
        [Button("����")]
        public virtual void Reset()
        {
            string path = Application.dataPath + @"\Resources\" + CurPath;
            if (!Directory.Exists(path))
            {
                Debug.LogWarning($"{CurPath}������");
                return;
            }
            PathDic = new Dictionary<string, string>();

            //�ļ���ջ��ͨ��������ȡÿһ�����ļ��е��ļ�
            Stack<string> directoryStack = new Stack<string>();
            directoryStack.Push(path);
            while (directoryStack.Count > 0)
            {
                path = directoryStack.Pop();
                Debug.Log($"��ǰ�����ļ��У�{path}");
                var dires = Directory.GetDirectories(path);
                foreach(var dire in dires)
                {
                    directoryStack.Push(dire);
                }
                for (int i = 0; i < _extensionList.Length; ++i)
                {
                    var files = Directory.GetFiles(path, "*." + _extensionList[i]);
                    //���ΪͼƬ�����ÿһ��ͼ�ֱ���
                    if (_extensionList[i] == "png")
                    {
                        foreach (var f in files)
                        {
                            var spr = Resources.LoadAll<Sprite>(path.
                                Remove(0, (Application.dataPath + 
                                @"\Resources\").Length) + @"\" + Path.
                                GetFileNameWithoutExtension(f));
                            foreach(var s in spr)
                            {
                                string fileName = s.name;
                                string filePath = path.Remove(0, (Application.dataPath
                                    + @"\Resources\").Length) + @"\" + fileName;
                                PathDic.Add(fileName, filePath);
                                Debug.Log($"¼��{fileName} ·����{filePath}");
                            }
                        }
                    }
                    else
                    {
                        foreach (var f in files)
                        {
                            string fileName = Path.GetFileNameWithoutExtension(f);
                            string filePath = path.Remove(0, (Application.dataPath
                                + @"\Resources\").Length) + @"\" + fileName;
                            if(fileName == this.name)
                            {
                                continue;
                            }
                            PathDic.Add(fileName, filePath);
                            Debug.Log($"¼��{fileName} ·����{filePath}");
                        }
                    }
                }
            }
            Debug.Log("¼�����");
        }
    }
}
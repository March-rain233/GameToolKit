using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// �ܿس�����Ʒ����������
/// </summary>
public class SceneObjectManager : MonoBehaviour
{
    /// <summary>
    /// ��ǰ��������Ʒ��Ϣ
    /// </summary>
    private Save.ObjectInfo[] CurScene
    {
        get
        {
            return GameManager.Instance.CurrentScene.Objects;
        }
    }

    /// <summary>
    /// ��δ����Ķ���
    /// </summary>
    [SerializeField]
    private List<Save.ObjectInfo> _unloadObjects;
    /// <summary>
    /// �������еĶ���
    /// </summary>
    [SerializeField]
    private List<MySceneObject> _activeObjects;
    /// <summary>
    /// �����õĶ���
    /// </summary>
    [SerializeField]
    private List<MySceneObject> _unactiveObjects;

    public void Init()
    {
        GameManager.Instance.OnLoadingPosition += (Map.MapPosition t) =>
        {
            CheckReasons();
        };
        GameManager.Instance.EventCenter.EventChanged += (string eventName, EventCenter.EventArgs eventArgs) =>
        {
            CheckReasons();
        };
        _activeObjects = new List<MySceneObject>();
        _unactiveObjects = new List<MySceneObject>();
        _unloadObjects = new List<Save.ObjectInfo>();
    }

    /// <summary>
    /// �����Ʒ��������״̬
    /// </summary>
    public void CheckReasons()
    {
        CheckToload();
        CheckToActive();
        CheckToUnactive();
        CheckToDestory();
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="info"></param>
    private MySceneObject CreateObject(Save.ObjectInfo info)
    {
        MySceneObject res = new MySceneObject();

        res.Object = (GameManager.Instance.FactoryManager.
        Create(info.Type, info.ResourceName) as ISceneObject).
        GetObject();
        res.Object.transform.position = res.Info.Position;
        res.Info = info;
        return res;
    }

    /// <summary>
    /// ����Ƿ��п����ɶ���
    /// </summary>
    private void CheckToload()
    {
        for(int i = _unloadObjects.Count - 1; i >= 0; --i)
        {
            if (_unloadObjects[i].CreatReason.Value)
            {
                var t = CreateObject(_unloadObjects[i]);
                _unloadObjects.RemoveAt(i);
                _activeObjects.Add(t);
            }
        }
    }

    /// <summary>
    /// ����Ƿ��пɼ������
    /// </summary>
    private void CheckToActive()
    {
        for(int i = _unactiveObjects.Count - 1; i >= 0; --i)
        {
            if (_unactiveObjects[i].Info.EnableReason.Value)
            {
                _unactiveObjects[i].Object.SetActive(true);
                _activeObjects.Add(_unactiveObjects[i]);
                _unactiveObjects.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// ����Ƿ�����Ҫ���ö���
    /// </summary>
    private void CheckToUnactive()
    {
        for (int i = _activeObjects.Count - 1; i >= 0; --i)
        {
            if (_activeObjects[i].Info.DisableReason.Value)
            {
                _activeObjects[i].Object.SetActive(false);
                _unactiveObjects.Add(_activeObjects[i]);
                _activeObjects.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// ����Ƿ�����Ҫ���ٶ���
    /// </summary>
    private void CheckToDestory()
    {
        for (int i = _unactiveObjects.Count - 1; i >= 0; --i)
        {
            if (_unactiveObjects[i].Info.DestoryReason.Value)
            {
                Destroy(_unactiveObjects[i].Object);
                _unloadObjects.Add(_unactiveObjects[i].Info);
                _unactiveObjects.RemoveAt(i);
            }
        }
        for (int i = _activeObjects.Count - 1; i >= 0; --i)
        {
            if (_activeObjects[i].Info.DestoryReason.Value)
            {
                Destroy(_activeObjects[i].Object);
                _unloadObjects.Add(_activeObjects[i].Info);
                _activeObjects.RemoveAt(i);
            }
        }
    }

    [SerializeField]
    private class MySceneObject
    {
        public Save.ObjectInfo Info;
        public GameObject Object;
    }
}

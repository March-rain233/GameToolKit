using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��������Panel
/// </summary>
public class PanelFactory
{
    public PanelFactory()
    {
        GameObject.DontDestroyOnLoad(Root);
    }

    private Transform _root;
    /// <summary>
    /// ����ظ���ַ
    /// </summary>
    public Transform Root
    {
        get
        {
            if (_root == null)
            {
                _root = GameObject.Find("UIroot").transform;
            }
            return _root;
        }
    }

    /// <summary>
    /// ���ڳ�
    /// </summary>
    private Dictionary<PanelType, BasePanel> _pool = new Dictionary<PanelType, BasePanel>();

    /// <summary>
    /// ���ɴ���
    /// </summary>
    /// <param name="windowType">��������</param>
    /// <param name="window">����ʵ��</param>
    /// <returns>�Ƿ���ڸô��ڵ�Ԥ����</returns>
    /// <remarks>
    /// �÷����������ڵĳ�ʼ��
    /// </remarks>
    public bool TryGetWindow(PanelType windowType, out BasePanel window)
    {
        //�����������д��ڣ���ֱ����ȡ
        if (_pool.TryGetValue(windowType, out window))
        {
            window.Init();
            return true;
        }
        else
        {
            //��ȡԤ���岢����
            var temp = GameObject.Instantiate(
                Resources.Load<GameObject>(GameManager.Instance.
                GameConfig.PathConfig.Paths[ObjectType.UI].
                PathDic[windowType.ToString()]));
            temp.name = temp.name.Replace("(Clone)", "");
            window = temp.GetComponent<BasePanel>();
        }

        window.Init();
        _pool.Add(windowType, window);
        return true;
    }

    /// <summary>
    /// �����ٵĴ��ڷ�������
    /// </summary>
    /// <param name="view"></param>
    public void PushToThePool(BasePanel view)
    {
        _pool[view.Type] = view;
        view.transform.parent = Root;
    }
}

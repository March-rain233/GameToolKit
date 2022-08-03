using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ͼ�Ļ��࣬��ΪView��ܿش��ڵ���ʾ
/// </summary>
public abstract class BaseView : MonoBehaviour
{
    /// <summary>
    /// ��ͼ������
    /// </summary>
    [SerializeField]
    protected Animator _animator;

    /// <summary>
    /// UI�Ƿ�����
    /// </summary>
    public bool IsEnable
    {
        get
        {
            return _isEnable;
        }
        protected set
        {
            _isEnable = value;
        }
    }
    private bool _isEnable;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        Init();
    }

    private void Start()
    {
        OnEnter();
    }

    /// <summary>
    /// ����UI����������
    /// </summary>
    /// <param name="arg"></param>
    public void SetIsEnable(bool arg)
    {
        IsEnable = arg;
        if (arg)
        {
            OnResume();
        }
        else
        {
            OnPause();
        }
    }

    /// <summary>
    /// UI������Ϣ
    /// </summary>
    protected internal abstract void OnEnter();

    /// <summary>
    /// UI��ͣ��Ϣ
    /// </summary>
    protected abstract void OnPause();

    /// <summary>
    /// UI�ָ���Ϣ
    /// </summary>
    protected abstract void OnResume();

    /// <summary>
    /// UI�ر���Ϣ
    /// </summary>
    protected abstract void OnExit();

    /// <summary>
    /// UI���·���
    /// </summary>
    /// <remarks>
    /// ���UI���ã�����ÿһ֡����
    /// </remarks>
    protected internal virtual void OnUpdate()
    {

    }

    /// <summary>
    /// ���������������������ڣ��˳�ui��
    /// </summary>
    /// <param name="resourceRecoveryHandler">
    /// ��Դ���մ���ί��
    /// </param>
    /// <remarks>
    /// ��дʱҪע����Ҫ��������ί��֪ͨ����������
    /// </remarks>
    protected internal virtual void SendExit(System.Action<BaseView> resourceRecoveryHandler)
    {
        if (_animator != null)
        {
            StartCoroutine(ObserveAnimProgress("Exit", 1, () =>
            {
                resourceRecoveryHandler.Invoke(this);
            }));
        }
        OnExit();
        if (_animator == null)
        {
            resourceRecoveryHandler.Invoke(this);
        }
    }

    /// <summary>
    /// ���Ӷ���״̬�����н���
    /// </summary>
    /// <param name="tag">״̬�ı�ǩ</param>
    /// <param name="time">Ŀ�����</param>
    /// <param name="callback">�ص�����</param>
    /// <returns></returns>
    public IEnumerator ObserveAnimProgress(string tag, float time, System.Action callback)
    {
        //��ȡĿ��״̬��ע�����
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        while (!stateInfo.IsTag(tag))
        {
            Debug.Log($"δ����{tag}����");
            yield return 0;
            stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        }

        //���Ӷ�������״̬
        while (stateInfo.normalizedTime < time)
        {
            //Debug.Log($"����{stateInfo.fullPathHash} ���н��̣�{stateInfo.normalizedTime}");
            yield return 0;
            stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        }

        //���ûص�����֪ͨ
        callback?.Invoke();
    }

    private void Update()
    {
        if (!IsEnable)
        {
            return;
        }

        OnUpdate();
    }

    /// <summary>
    /// �ַ�model����������
    /// </summary>
    /// <param name="name">������</param>
    /// <param name="value">����</param>
    public abstract void NotifyHandler(string name, object value);

    /// <summary>
    /// ��ʼ��UI��ͼ
    /// </summary>
    protected internal abstract void Init();
}

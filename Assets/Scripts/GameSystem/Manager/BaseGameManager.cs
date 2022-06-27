using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;
using Save;
using Map;
using NPC;
using Config;
using UnityEngine.SceneManagement;

/// <summary>
/// ��������࣬��Ϊ�н��߰Ѳ����ַ�������������
/// </summary>
/// <remarks>
/// �������Ĺ��ܿ������������벻Ҫ��������������
/// </remarks>
public abstract class BaseGameManager<T> : SerializedMonoBehaviour
    where T : BaseGameManager<T>
{
    /// <summary>
    /// ����
    /// </summary>
    public static T Instance
    {
        get 
        { 
            if(_instance == null)
            {
                //Debug.LogError("Υ�����");
            }
            return _instance;
        }
    }
    private static T _instance;
    //��������
    public FactoryManager FactoryManager
    {
        get;
        private set;
    }
    public AudioManager AudioManager
    {
        get;
        private set;
    }

    public EventCenter EventCenter
    {
        get;
        private set;
    }
    public MapManager MapManager
    {
        get;
        private set;
    }
    public ControlManager ControlManager
    {
        get;
        private set;
    }
    public PanelManager PanelManager
    {
        get;
        private set;
    }
    public SceneObjectManager SceneObjectManager
    {
        get;
        private set;
    }

    /// <summary>
    /// ��Դ·�������ļ�
    /// </summary>
    public GameConfig GameConfig
    {
        get;
        private set;
    }

    /// <summary>
    /// ��ǰ����
    /// </summary>
    public Config.SceneInfo CurrentScene
    {
        get;
        protected set;
    }

    /// <summary>
    /// ��Ϸ״̬
    /// </summary>
    public GameStatus Status
    {
        get;
        protected internal set;
    }

    /// <summary>
    /// ��ʼ�л�����
    /// </summary>
    public event System.Action<MapPosition> BeginToLoadPosition;

    /// <summary>
    /// ������ʼ����
    /// </summary>
    public event System.Action<MapPosition> OnLoadingPosition;

    /// <summary>
    /// �����л����
    /// </summary>
    public event System.Action<MapPosition> AfterLoadingPosition;

    protected virtual void Awake()
    {
        if(_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this as T;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// ��ʼ��������
    /// </summary>
    /// <typeparam name="TInstaller"></typeparam>
    protected virtual void InitManager<TInstaller>() where TInstaller : IFactoryInstaller, new()
    {
        GameConfig = Resources.Load<GameConfig>("Config/ȫ������");
        FactoryManager = new FactoryManager();
        AudioManager = FindObjectOfType<AudioManager>();
        ControlManager = GetComponent<ControlManager>();
        EventCenter = GetComponent<EventCenter>();
        MapManager = new MapManager();
        //PanelManager = new PanelManager();
        SceneObjectManager = GetComponent<SceneObjectManager>();

        //��ʼ����������
        TInstaller installer = new TInstaller();
        installer.Install(FactoryManager);
        if (FactoryManager.Load(out SettingSave setting, "Setting.sav"))
        {
            AudioManager.Init(setting.SoundSave);
            ControlManager.Init(setting.ControlSave);
        }
        else
        {
            AudioManager.Init(new SoundSave
            {
                Effect = 1,
                Music = 1
            });
            ControlManager.Init(null);
        }

        SceneObjectManager.Init();
    }

    /// <summary>
    /// ��ʼ����Ϸ״̬
    /// </summary>
    protected virtual void InitGameStatus()
    {
        Status = GameStatus.Pause;
    }

    /// <summary>
    /// <inheritdoc cref="AudioManager.Play(AudioClip, MusicPath, bool)"/>
    /// </summary>
    /// <param name="name">��Դ��</param>
    /// <param name="path">��Ƶͨ��</param>
    /// <param name="isLoop">�Ƿ�ѭ��</param>
    public void PlaySound(string name, MusicPath path, bool isLoop = false)
    {
        AudioManager.Play((FactoryManager.Create(ObjectType.AudioClip, name) as PAudioClip).AudioClip, path, isLoop);
    }

    /// <summary>
    /// ת�Ƶ�ָ���ص�
    /// </summary>
    /// <param name="position"></param>
    public void EnterPosition(MapPosition position)
    {
        //var oldPosition = GameManager.CurrentScene;
        //GameManager.Status = GameStatus.Loading;
        ////�ȴ�����������������ٽ���ת��
        ////PanelManager.Push(WindowType.loading);
        //PanelManager.Peek().StartCoroutine(PanelManager.Peek().ObserveAnimProgress("Enter", 1, () => 
        //{
        //    Debug.Log($"���ڴ�{oldPosition.name}��ת��{position.Scene}:{position.Point}");
        //    //����󳡾�û�����仯��ֱ����ת
        //    if (GameConfig.SceneInfoConfig.ScenesObject[position.Scene]
        //        != GameManager.CurrentScene)
        //    {
        //        MapManager.LoadScene(GameConfig.SceneInfoConfig.
        //            ScenesObject[position.Scene].Scene, () =>
        //            {
        //                LoadPosition(position);
        //            });
        //    }
        //    else
        //    {
        //        LoadPosition(position);
        //    }
        //}));
        //BeginToLoadPosition?.Invoke(position);
    }

    /// <summary>
    /// ���볡��
    /// </summary>
    /// <remarks>
    /// ��ʼ����ͼ�������ɳ�����Ʒ
    /// </remarks>
    protected void LoadPosition(MapPosition position)
    {
        OnLoadingPosition?.Invoke(position);
        Status = GameStatus.Playing;
        AfterLoadingPosition?.Invoke(position);
    }
}

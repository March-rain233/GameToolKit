//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Sirenix.OdinInspector;
//using System.Linq;
//using System;

//public class TalkSystem : SerializedMonoBehaviour
//{
//    /// <summary>
//    /// ����
//    /// </summary>
//    [System.Serializable]
//    public struct TextBody
//    {
//        [System.Serializable]
//        public enum DialogType
//        {
//            Bubble,
//            Queue,
//        }
//        [System.Serializable]
//        public enum ControlType
//        {
//            KeyBoard,
//            Mouse,
//            Both
//        }
//        [LabelText("��ɫ��")]
//        public string Name;
//        [LabelText("����"), Multiline]
//        public string Body;
//        [LabelText("��Ϸ������")]
//        public string ObjectName;
//        [LabelText("�Ƿ񲥷���������ת����һ�Ի�")]
//        public bool Skip;
//        [LabelText("�Ի�������"), EnumToggleButtons]
//        public DialogType Dialog;
//        [LabelText("��������"), EnumToggleButtons]
//        public ControlType Control;

//    }

//    public static TalkSystem Instance
//    {
//        get;
//        private set;
//    }

//    public GameObject BubbleDialogPrefabs;

//    [SerializeField]
//    private Queue<TextBody> _textBodies = new Queue<TextBody>();

//    [SerializeField]
//    private bool _skip = false;

//    private BubbleDialog BubbleDialog
//    {
//        get
//        {
//            if (_dialog == null)
//            {
//                _dialog = Instantiate(BubbleDialogPrefabs, GameObject.Find("UIroot").transform).GetComponent<BubbleDialog>();
//                _dialog.ShowEnd += TextEnd;
//            }
//            return _dialog;
//        }
//    }
//    [SerializeField]
//    private BubbleDialog _dialog;

//    private CanvasGroup Mask
//    {
//        get
//        {
//            if(_mask == null)
//            {
//                _mask = GameObject.Find("MASK")?.GetComponent<CanvasGroup>();
//            }
//            return _mask;
//        }
//    }
//    private CanvasGroup _mask;

//    [SerializeField]
//    public Dialogue.DialogueTree DialogueTree
//    {
//        get => _dialogueTree;
//        set
//        {
//            if (value)
//            {
//                if(value != _dialogueTree)
//                {
//                    _dialogueTree = value.Clone() as Dialogue.DialogueTree; 
//                }
//            }
//            else
//            {
//                _dialogueTree = null;
//            }
//        }
//    }
//    private Dialogue.DialogueTree _dialogueTree;

//    private bool _dialogEnd;

//    private void Start()
//    {
//        if (Instance != null)
//        {
//            Destroy(gameObject);
//            return;
//        }
//        Instance = this;
//        DontDestroyOnLoad(gameObject);

//        if (Mask) Mask.blocksRaycasts = false;

//        GameManager.Instance.EventCenter.AddListener("DIALOG_EXIT", e =>
//        {
//            _dialog?.Hide();
//            if (Mask) Mask.blocksRaycasts = false;
//        });
//        GameManager.Instance.EventCenter.AddListener("DIALOG_PUSH", e =>
//        {
//            PushBodies(e.Object as TextBody[]);
//            if (Mask) Mask.blocksRaycasts = true;
//        });
//        GameManager.Instance.EventCenter.AddListener("DIALOG_HIDE", e => BubbleDialog.Hide());
//        GameManager.Instance.EventCenter.AddListener("DIALOG_SHOW", e => BubbleDialog.Show());
//    }

//    private void Update()
//    {
//        if (_textBodies.Count > 0)
//        {
//            bool interact = Input.GetKeyDown(GameManager.Instance.ControlManager.KeyDic[KeyType.Interact]);
//            bool skip = Input.GetKey(GameManager.Instance.ControlManager.KeyDic[KeyType.Skip]);
//            bool mouse = Input.GetMouseButtonDown(0);
//            bool next = false;
//            switch (_textBodies.Peek().Control)
//            {
//                case TextBody.ControlType.KeyBoard:
//                    next = skip | interact;
//                    break;
//                case TextBody.ControlType.Mouse:
//                    next = mouse;
//                    break;
//                case TextBody.ControlType.Both:
//                    next = mouse | interact | skip;
//                    break;
//            }
//            if (next) { NextStep(); }
//        }
//        if (DialogueTree)
//        {
//            if(DialogueTree.Tick(null) == NodeStatus.Success)
//            {
//                DialogueTree = null;
//            }
//        }
//    }

//    public void PushBodies(TextBody[] textBodies)
//    {
//        Array.ForEach(textBodies, body => _textBodies.Enqueue(body));
//        _dialogEnd = false;
//        NextStep();
//    }

//    public void NextStep()
//    {
//        if (_dialogEnd)
//        {
//            _dialogEnd = false;
//            _textBodies.Dequeue();
//        }
//        if (_textBodies.Count <= 0) 
//        { 
//            GameManager.Instance.EventCenter.SendEvent("DIALOG_END", new EventCenter.EventArgs());
//            return;
//        }
//        var body = _textBodies.Peek();
//        switch (body.Dialog)
//        {
//            case TextBody.DialogType.Bubble:
//                BubbleDialogHandler(body);
//                break;
//            case TextBody.DialogType.Queue:
//                break;
//        }
//    }

//    private void TextEnd()
//    {
//        //_textBodies.Dequeue();
//        _dialogEnd = true;
//        if (_skip) { NextStep(); }
//        if (_textBodies.Count <= 0)
//        {
//            GameManager.Instance.EventCenter.SendEvent("DIALOG_END", new EventCenter.EventArgs());
//            return;
//        }
//    }

//    private void BubbleDialogHandler(TextBody body)
//    {
//        if (!BubbleDialog.isActiveAndEnabled) { BubbleDialog.gameObject.SetActive(true); }
//        if (BubbleDialog.Typing)
//        {
//            BubbleDialog.OutputImmediately();
//        }
//        else
//        {
//            if (!string.IsNullOrEmpty(body.Name))
//            {
//                BubbleDialog.SetName(body.Name);
//                if (string.IsNullOrEmpty(body.ObjectName))
//                {
//                    Transform t = null;
//                    t = GetTalker(body.Name);
//                    SetDialogFollow(t);
//                }
//            }
//            if (!string.IsNullOrEmpty(body.ObjectName))
//            {
//                Transform t = null;
//                t = GetTalker(body.ObjectName);
//                SetDialogFollow(t);
//            }
//            _skip = body.Skip;
//            BubbleDialog.BeginRead(body.Body);
//        }
//    }

//    private Transform GetTalker(string name)
//    {
//        Transform t = null;
//        if (DialogueTree.Variables.ContainsKey(name))
//        {
//            var temp = DialogueTree.Variables[name].Object;
//            if (temp is GameObject)
//            {
//                t = (temp as GameObject).transform.Find("TalkPoint");
//                if (!t) { t = (temp as GameObject).transform; }
//            }
//            if (temp is Component)
//            {
//                t = (temp as Component).transform.Find("TalkPoint");
//                if (!t) { t = (temp as Component).transform; }
//            }
//        }
//        else
//        {
//            t = GameObject.Find(name).transform.Find("TalkPoint");
//            if (!t) { t = GameObject.Find(name).transform; }
//        }
//        return t;
//    }

//    private void SetDialogFollow(Transform follow)
//    {
//        BubbleDialog.Follow = follow;
//    }
//}

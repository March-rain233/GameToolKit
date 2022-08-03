using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using System.Text.RegularExpressions;

public abstract class Dialog : SerializedMonoBehaviour
{
    /// <summary>
    /// ���Ŀؼ�
    /// </summary>
    [SerializeField]
    protected TextMeshProUGUI _textLable;

    /// <summary>
    /// ��ǰ������ı�
    /// </summary>
    [SerializeField, TextArea]
    protected string _outPutText;

    /// <summary>
    /// ��ǰ�ı�����
    /// </summary>
    [SerializeField]
    protected float _textSpeed;

    /// <summary>
    /// Ĭ���������
    /// </summary>
    [SerializeField]
    protected float _defaultSpeed;

    /// <summary>
    /// ��ǰ�Ƿ������
    /// </summary>
    [OdinSerialize]
    public bool Typing
    {
        get;
        private set;
    }

    //�ж��Ƿ�Ϊ��ǩ
    protected static Regex _open = new Regex(@"^<[^/]+?>");
    protected static Regex _close = new Regex(@"^</[^/]+?>");

    /// <summary>
    /// ��ǰ֧�ֵ��Զ����ǩ
    /// </summary>
    [OdinSerialize]
    protected static List<string> UserTag = new List<string>();

    public System.Action ShowEnd;
    public System.Action BeginShow;

    /// <summary>
    /// �������
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReadWord()
    {
        string tempSave = _outPutText;

        int offset = 0;
        //����������ı���
        for (int i = 0; i < tempSave.Length; ++i)
        {
            while (Regex.IsMatch(tempSave.Substring(i), @"^<.+?>"))
            {
                //�ж��Ƿ�Ϊ����ǩ������������ר�Ŵ���
                if (_open.IsMatch(tempSave.Substring(i)))
                {
                    var tag = _open.Match(tempSave.Substring(i)).Value;
                    //�Բ�����html���Զ����ǩ�������⴦��
                    if (IsUserTag(tag))
                    {
                        //ִ�б�ǩ����
                        UserOpenTagHandler(tag);

                        //����Ҫ������ַ�����ɾȥ
                        tempSave = _open.Replace(tempSave, "", 1);
                    }
                    else
                    {
                        //������ǩ���벢�ѹ���ú�
                        _textLable.text = _textLable.text.Insert(offset + i, tag);
                        i += tag.Length;

                        //���ɶ�Ӧ�ձ�ǩ������
                        string close = Regex.Match(tag
                            , @"<[^/]+?>").Value
                            .Insert(1, @"/");
                        _textLable.text = _textLable.text.Insert(offset + i, close);
                    }
                }
                else if (_close.IsMatch(tempSave.Substring(i)))
                {
                    var tag = _close.Match(tempSave.Substring(i)).Value;
                    if (IsUserTag(tag))
                    {
                        //ִ�б�ǩ����
                        UserCloseTagHandler(tag);

                        //����Ҫ������ַ�����ɾȥ
                        tempSave = _close.Replace(tempSave, "", 1);
                    }
                    else
                    {
                        i += _close.Match(tempSave.Substring(i)).Value.Length;
                    }
                }
            }
            if (i < tempSave.Length)
            {
                _textLable.text = _textLable.text.Insert(offset + i, _outPutText[i].ToString());
            }
            yield return new WaitForSeconds(_textSpeed);
        }

        //�����ϣ�֪ͨ�����������һ��
        ReadFin();
    }

    /// <summary>
    /// �ж��Ƿ�Ϊ�û��Զ���ı�ǩ
    /// </summary>
    protected static bool IsUserTag(string source)
    {
        //��ȡ��ǩ�ؼ���
        source = Regex.Match(source
            , @"(?<=\<)(/?[^/= ]+?)(?=(( |=)[^>]*?)?\>)").Value;

        //�����洢���û���ǩ���鿴�Ƿ�ƥ��
        return UserTag.Exists(tag => tag == source);
    }

    /// <summary>
    /// ���ݴ�����Զ����ǩͷִ�ж�Ӧ�߼�
    /// </summary>
    protected virtual void UserOpenTagHandler(string value)
    {
        //goto:��ǩͷ
    }

    /// <summary>
    /// ���ݴ�����Զ����ǩβִ�ж�Ӧ�߼�
    /// </summary>
    protected virtual void UserCloseTagHandler(string value)
    {
        //TODO:��ձ�ǩ
    }

    /// <summary>
    /// ��ȡ������ʾ���ı�
    /// </summary>
    /// <returns></returns>
    protected string GetOutPutText()
    {
        string tempSave = _outPutText;
        for (int i = 0; i < tempSave.Length; ++i)
        {
            //�����е��Զ����ǩ��ת��Ϊ��
            while (i < tempSave.Length
                && Regex.IsMatch(tempSave.Substring(i), @"^<.+?>"))
            {
                string tag = Regex.Match(tempSave.Substring(i)
                    , @"^<.+?>").Value;
                if (IsUserTag(tag))
                {
                    tempSave.Replace(tag, "");
                }
                else { break; }
            }
        }
        return tempSave;
    }

    /// <summary>
    /// ��ʼ���
    /// </summary>
    /// <param name="output"></param>
    [Button]
    public void BeginRead(string output)
    {
        StopAllCoroutines();
        _textLable.text = "";
        Typing = true;
        _outPutText = output;
        BeginShow?.Invoke();
        StartCoroutine("ReadWord");
    }

    public void OutputImmediately()
    {
        StopAllCoroutines();
        _textLable.text = "";
        BeginShow?.Invoke();
        ReadFin();
    }

    /// <summary>
    /// �����������Ļ
    /// </summary>
    /// <param name="output"></param>
    public void OutputImmediately(string output)
    {
        StopAllCoroutines();
        _textLable.text = "";
        _outPutText = output;
        BeginShow?.Invoke();
        ReadFin();
    }

    /// <summary>
    /// ������
    /// </summary>
    private void ReadFin()
    {
        _textLable.text = GetOutPutText();
        Typing = false;
        ShowEnd.Invoke();
    }
}

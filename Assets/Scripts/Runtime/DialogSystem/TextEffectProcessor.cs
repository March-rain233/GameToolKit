using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using GameFrame.Utility;

namespace GameFrame.Dialog
{
    /// <summary>
    /// �ı�Ч��������
    /// </summary>
    public class TextEffectProcessor
    {
        public delegate void CharacterRenderHandler();
        public delegate void TagInvokeHandler(string attr);
        public delegate void TagCancelHandler();

        /// <summary>
        /// ��ǩ������������
        /// </summary>
        public struct TagProcessor
        {
            public TagInvokeHandler OnTagInvoke;
            public TagCancelHandler OnTagCancel;
            public CharacterRenderHandler OnRenderCharacter;
        }

        TextMeshProUGUI _textController;
        RichTextTree _tree;
        string _rawText;

        /// <summary>
        /// �ı�����ٶ� �ַ�/��
        /// </summary>
        public float TextSpeed
        {
            get
            {
                if(_speedStack.Count > 0)
                {
                    return _speedStack.Peek();
                }
                return _defaultSpeed;
            }
        }
        float _defaultSpeed = 1;
        Stack<float> _speedStack = new Stack<float>();

        /// <summary>
        /// ��ȡ��һ���ַ�����Ҫ��ʱ��
        /// </summary>
        public float NextCharacterTime;

        /// <summary>
        /// ��ȫ�����ı�����
        /// </summary>
        public event Action OnAllCharactersVisiable;

        /// <summary>
        /// �Ƿ��ڹ���״̬
        /// </summary>
        bool _isWorking = false;
        /// <summary>
        /// ����ʾȫ��
        /// </summary>
        Action onDisplayAll;

        public Dictionary<string, TagProcessor> TagProcessors = new Dictionary<string, TagProcessor>();

        public TextEffectProcessor(string text, TextMeshProUGUI controller)
        {
            _rawText = text;
            _tree = new RichTextTree(text);
            _textController = controller;

            Init();
        }

        void Init()
        {
            var speed = new TagProcessor()
            {
                OnTagInvoke = attr =>_speedStack.Push(Convert.ToSingle(RichTextUtility.GetPropertys(attr)["speed"])),
                OnTagCancel = ()=>_speedStack.Pop(),
            };
            TagProcessors["speed"] = speed;

            var comma = new TagProcessor()
            {
                OnTagInvoke = attr=>NextCharacterTime= Convert.ToSingle(RichTextUtility.GetPropertys(attr)["comma"]),
            };
            TagProcessors["comma"] = comma;
        }

        /// <summary>
        /// �����ı�
        /// </summary>
        /// <returns></returns>
        public IEnumerator Process()
        {
            if (_isWorking)
            {
                Debug.LogError("�ô������Ѵ��ڹ���״̬");
                yield break;
            }
            _isWorking = true;

            //��ʼ��
            _textController.SetText(RichTextHelper.GetOutPutText(_tree));
            _textController.maxVisibleCharacters = 0;
            NextCharacterTime = 0;

            bool readEnd = false;//��ȡ����

            var enumerator = RichTextHelper.GetTagInTraver(_tree);
            var begin = new Queue<PairTagNode>();
            var end = new Queue<PairTagNode>();
            var empty = new Queue<EmptyTagNode>();

            onDisplayAll = () =>
            {
                readEnd = true;
                _textController.maxVisibleCharacters = _textController.textInfo.characterCount;
                Action solveTag = () =>
                {
                    while (begin.Count > 0)
                    {
                        var tag = begin.Dequeue();
                        if (TagProcessors.TryGetValue(tag.Tag, out var processor))
                        {
                            processor.OnTagInvoke?.Invoke(tag.Attr);
                        }
                    }
                    while (end.Count > 0)
                    {
                        var tag = end.Dequeue();
                        if (TagProcessors.TryGetValue(tag.Tag, out var processor))
                        {
                            processor.OnTagCancel?.Invoke();
                        }
                    }
                    while (empty.Count > 0)
                    {
                        var tag = empty.Dequeue();
                        if (TagProcessors.TryGetValue(tag.Tag, out var processor))
                        {
                            processor.OnTagInvoke?.Invoke(tag.Attr);
                        }
                    }
                };
                solveTag();
                while (enumerator.MoveNext())
                {
                    (begin, end, empty) = enumerator.Current;
                    solveTag();
                }
            };

            //ÿһ֡������Ч�����и���
            while (true)
            {
                _textController.ForceMeshUpdate();

                //�ı���Ϣ
                var textInfo = _textController.textInfo;

                //��ÿ���ֽ��и���
                for (int i = 0; i < _textController.maxVisibleCharacters; i++)
                {
                    var charInfo = textInfo.characterInfo[i];
                    var pairList = RichTextHelper.GetNonNativeTagsOfIndex(_tree, i);
                    foreach(var tag in pairList)
                    {
                        if(TagProcessors.TryGetValue(tag.Tag, out TagProcessor processor))
                        {
                            processor.OnRenderCharacter?.Invoke();
                        }
                    }
                }

                //Ӧ�ø���
                _textController.UpdateVertexData();

                //�ı�����ȡ
                if(!readEnd && NextCharacterTime <= 0)
                {
                    if (empty.Count > 0) //������ڻ�δ����Ŀձ�ǩ���������ձ�ǩ
                    {
                        while(NextCharacterTime <= 0 && empty.Count > 0)
                        {
                            var tag = empty.Dequeue();
                            if(TagProcessors.TryGetValue(tag.Tag, out var processor))
                            {
                                processor.OnTagInvoke?.Invoke(tag.Attr);
                            }
                        }
                    }
                    else //���������һ�����ı�
                    {
                        if (enumerator.MoveNext())
                        {
                            (begin, end, empty) = enumerator.Current;
                            while (begin.Count > 0)
                            {
                                var tag = begin.Dequeue();
                                if (TagProcessors.TryGetValue(tag.Tag, out var processor))
                                {
                                    processor.OnTagInvoke?.Invoke(tag.Attr);
                                }
                            }
                            while (end.Count > 0)
                            {
                                var tag = end.Dequeue();
                                if (TagProcessors.TryGetValue(tag.Tag, out var processor))
                                {
                                    processor.OnTagCancel?.Invoke();
                                }
                            }
                            if (empty.Count == 0) //���ձ�ǩ��ĿΪ0ʱ˵�����������ʾ�ַ�
                            {
                                ++_textController.maxVisibleCharacters;
                                NextCharacterTime = SpeedToTime(TextSpeed);
                            }
                        }
                        else
                        {
                            readEnd = true;
                            OnAllCharactersVisiable?.Invoke();
                        }
                    }
                }
                else
                {
                    NextCharacterTime -= Time.deltaTime;
                }
                yield return null;
            }
        }

        /// <summary>
        /// ǿ���ı���ʾȫ��
        /// </summary>
        public void DisplayAll()
        {
            onDisplayAll?.Invoke();
        }

        /// <summary>
        /// ���ݴ�����ı���ʾ�ٶ�������Ҫ��ʾ��ʱ��
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        float SpeedToTime(float speed)
        {
            return 1 / speed;
        }
    }
}

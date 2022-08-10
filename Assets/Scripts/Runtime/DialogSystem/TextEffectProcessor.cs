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
    /// 文本效果处理器
    /// </summary>
    public class TextEffectProcessor
    {
        public delegate void CharacterRenderHandler();
        public delegate void TagInvokeHandler(string attr);
        public delegate void TagCancelHandler();

        /// <summary>
        /// 标签处理器函数集
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
        /// 文本输出速度 字符/秒
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
        /// 读取下一个字符所需要的时间
        /// </summary>
        public float NextCharacterTime;

        /// <summary>
        /// 当全部的文本可视
        /// </summary>
        public event Action OnAllCharactersVisiable;

        /// <summary>
        /// 是否处于工作状态
        /// </summary>
        bool _isWorking = false;
        /// <summary>
        /// 当显示全部
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
        /// 处理文本
        /// </summary>
        /// <returns></returns>
        public IEnumerator Process()
        {
            if (_isWorking)
            {
                Debug.LogError("该处理器已处于工作状态");
                yield break;
            }
            _isWorking = true;

            //初始化
            _textController.SetText(RichTextHelper.GetOutPutText(_tree));
            _textController.maxVisibleCharacters = 0;
            NextCharacterTime = 0;

            bool readEnd = false;//读取结束

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

            //每一帧对字体效果进行更新
            while (true)
            {
                _textController.ForceMeshUpdate();

                //文本信息
                var textInfo = _textController.textInfo;

                //对每个字进行更新
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

                //应用更改
                _textController.UpdateVertexData();

                //文本流读取
                if(!readEnd && NextCharacterTime <= 0)
                {
                    if (empty.Count > 0) //如果存在还未处理的空标签则继续处理空标签
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
                    else //负责读入下一步的文本
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
                            if (empty.Count == 0) //当空标签数目为0时说明读入的是显示字符
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
        /// 强制文本显示全部
        /// </summary>
        public void DisplayAll()
        {
            onDisplayAll?.Invoke();
        }

        /// <summary>
        /// 根据传入的文本显示速度推算需要显示的时间
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        float SpeedToTime(float speed)
        {
            return 1 / speed;
        }
    }
}

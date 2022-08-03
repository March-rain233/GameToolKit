using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using GameFrame.Utility;
using System.Collections.Generic;
using System;
using System.Linq;

namespace GameFrame.Dialog.Editor {
    public class TextEditor : EditorWindow
    {
        /// <summary>
        /// Դ�ı�
        /// </summary>
        public string RawText
        {
            get => _tree.ToString();
            set
            {
                _tree = new RichTextTree(value);
                var rawText = RawText;
                var plainText = _tree.GetPlainText();

                rawTextField.SetValueWithoutNotify(rawText);
                plainTextField.SetValueWithoutNotify(plainText);
                previewTextField.text = plainText;

                rawTextLength.SetValueWithoutNotify(rawText.Length);
                plainTextLength.SetValueWithoutNotify(plainText.Length);

                OnTextIndexChanged();
            }
        }
        
        RichTextTree _tree;

        /// <summary>
        /// ���ָ��
        /// </summary>
        /// <remarks>
        /// ����ÿһ���������Ӧ���ı��ڵ��������е�λ�ã����λ��-1��Ӧ������Ԫ
        /// </remarks>
        List<InnerTextNode> _indexPointer;

        public int BeginIndex => Mathf.Min(plainTextField.selectIndex, plainTextField.cursorIndex);
        public int EndIndex => Mathf.Max(plainTextField.selectIndex, plainTextField.cursorIndex);

        bool _showRawText = true;

        TextField rawTextField;
        TextField plainTextField;
        Label previewTextField;

        IntegerField rawTextLength;
        IntegerField plainTextLength;
        IntegerField beginIndexField;
        IntegerField endIndexField;

        Button playBtn;

        VisualElement floatBtn;

        [MenuItem("ǳ����ι���/Text Editor")]
        public static void ShowExample()
        {
            TextEditor wnd = GetWindow<TextEditor>();
            wnd.titleContent = new GUIContent("Text Editor");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/DialogSystem/TextEditor.uxml");
            visualTree.CloneTree(root);

            rawTextField = root.Q<TextField>("raw-text");
            plainTextField = root.Q<TextField>("plain-text");
            previewTextField = root.Q("preview-text").Q<Label>("body");
            rawTextLength = root.Q<IntegerField>("raw-text-length");
            plainTextLength = root.Q<IntegerField>("plain-text-length");
            beginIndexField = root.Q<IntegerField>("begin-index");
            endIndexField = root.Q<IntegerField>("end-index");
            playBtn = root.Q<Button>("play");
            floatBtn = root.Q("float-button");
            var floatBtnView = floatBtn.Q<Label>("button-view");
            var left = root.Q("left");

            rawTextField.RegisterValueChangedCallback(e => RawText = e.newValue);
            plainTextField.RegisterCallback<FocusOutEvent>(e => 
            {
                OnTextIndexChanged();
            });
            plainTextField.RegisterCallback<KeyDownEvent>(e =>
            {
                if(e.character != char.MinValue) //�������ַ�ʱ�����������λ�ò����ַ�
                {
                    if(BeginIndex == EndIndex) //�����һ��ʱΪ�����ַ�
                    {
                        _tree.InsertPlainText(BeginIndex, e.character.ToString());
                    }
                    else //����Ϊ����ַ�
                    {
                        _tree.ReplacePlainText(BeginIndex, EndIndex, e.character.ToString());
                    }
                }
                else if(e.keyCode == KeyCode.Backspace) //��ɾ���ַ�ʱ��ɾ��������ڵ��ַ�
                {
                    if(BeginIndex == EndIndex)//�����λ����ͬʱɾ��ǰһ���ַ�
                    {
                        if(BeginIndex != 0)
                        {
                            _tree.RemovePlainText(BeginIndex - 1, EndIndex);
                        }
                    }
                    else //����ɾ����Χ���ַ�
                    {
                        _tree.RemovePlainText(BeginIndex, EndIndex);
                    }
                }
                else if(e.ctrlKey && e.keyCode == KeyCode.V) //����ı�
                {
                    if (BeginIndex == EndIndex) //�����һ��ʱΪ�����ַ�
                    {
                        _tree.InsertPlainText(BeginIndex, GUIUtility.systemCopyBuffer);
                    }
                    else //����Ϊ����ַ�
                    {
                        _tree.ReplacePlainText(BeginIndex, EndIndex, GUIUtility.systemCopyBuffer);
                    }
                }
                rawTextField.SetValueWithoutNotify(_tree.ToString());
                previewTextField.text = _tree.ToString();
            });
            endIndexField.RegisterValueChangedCallback(e =>
            {
                var value = e.newValue;
                if(value > plainTextField.text.Length)
                {
                    value = plainTextField.text.Length;
                }
                else if(value < beginIndexField.value)
                {
                    value = beginIndexField.value;
                }
                plainTextField.SelectRange(beginIndexField.value, value);
                endIndexField.SetValueWithoutNotify(value);
            });
            beginIndexField.RegisterValueChangedCallback(e =>
            {
                var value = e.newValue;
                if (value > endIndexField.value)
                {
                    value = endIndexField.value;
                }
                else if (value < 0)
                {
                    value = 0;
                }
                plainTextField.SelectRange(value, endIndexField.value);
                beginIndexField.SetValueWithoutNotify(value);
            });
            playBtn.clicked += () =>
            {
                Debug.Log($"cursor:{plainTextField.cursorIndex} select:{plainTextField.selectIndex}");
            };
            floatBtn.RegisterCallback<ClickEvent>(e =>
            {
                if (e.button == 0)
                {
                    _showRawText = !_showRawText;
                    if (_showRawText)
                    {
                        left.style.width = 250;
                        floatBtnView.text = "��";
                    }
                    else
                    {
                        left.style.width = 0;
                        floatBtnView.text = "��";
                    }
                }
            });

            rawTextLength.SetEnabled(false);
            plainTextLength.SetEnabled(false);

            RawText = "";
        }

        void OnTextIndexChanged()
        {
            endIndexField.SetValueWithoutNotify(Mathf.Max(plainTextField.selectIndex, plainTextField.cursorIndex));
            beginIndexField.SetValueWithoutNotify(Mathf.Min(plainTextField.selectIndex, plainTextField.cursorIndex));
        }
    }
}
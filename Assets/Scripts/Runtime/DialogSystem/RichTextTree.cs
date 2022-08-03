using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;
using System.Collections.ObjectModel;

namespace GameFrame.Dialog
{
    /// <summary>
    /// ���ı�������
    /// </summary>
    public class RichTextTree : IEnumerable<RichTextNode>
    {
        /// <summary>
        /// ���ڵ�
        /// </summary>
        public RootNode RootNode { get; private set; }

        /// <summary>
        /// ���ݴ����ı�����������
        /// </summary>
        /// <param name="rawText"></param>
        public RichTextTree(string rawText)
        {
            RootNode = new RootNode();
            Stack<PairTagNode> stack = new Stack<PairTagNode>();
            Dictionary<string, bool> isEmptyTag = new Dictionary<string, bool>();//�ж��Ƿ��ǿձ�ǩ
            StringBuilder stringBuilder = new StringBuilder(rawText);
            stack.Push(RootNode);
            Action<string> createTextNode = (string body) =>
            {
                RichTextNode node = new InnerTextNode(body);
                stack.Peek().AddChild(node);
                stringBuilder.Remove(0, body.Length);
            };

            //todo������<noparse></noparse>��
            while (stringBuilder.Length > 0)
            {
                var temp = stringBuilder.ToString();
                var match = Regex.Match(temp, "<([^\"<>]|\".*\")*?>");//������һ��ǩ
                if (match.Success)
                {
                    if(match.Index != 0)
                    {
                        createTextNode(temp.Substring(0, match.Index));
                    }

                    string tag = match.Value.Substring(1, match.Length - 2);
                    if(tag[0] == '/')//����Ǳձ�ǩ����ջ��Ԫ��
                    {
                        var node = stack.Pop();
                        if (node.Tag != tag.Remove(0, 1))
                        {
                            throw new RichTextTreeException(node, $"{tag.Remove(0, 1)} Labels do not match");
                        }
                        stringBuilder.Remove(0, match.Length);
                    }
                    else
                    {
                        //��ȡ��ǩ��Ӧ����
                        var tagName = Regex.Match(tag, "^\\w*").Value;
                        //�ж��Ƿ��ǿձ�ǩ
                        if (!isEmptyTag.ContainsKey(tagName))
                        {
                            isEmptyTag[tagName] = !rawText.Contains($"</{tagName}>");
                        }
                        if (isEmptyTag[tagName])
                        {
                            var node = new EmptyTagNode(tagName, tag);
                            stack.Peek().AddChild(node);
                        }
                        else
                        {
                            //����ǿ���ǩ��ѹ��Ԫ��
                            var node = new PairTagNode(tagName, tag);
                            stack.Peek().AddChild(node);
                            stack.Push(node);
                        }
                        stringBuilder.Remove(0, match.Length);
                    }
                }
                else
                {
                    createTextNode(temp);
                    break;
                }
            }

            if(stack.Peek() != RootNode)
            {
                throw new RichTextTreeException(RootNode, "Parse failure");
            }
        }

        /// <summary>
        /// ���봿�ı�
        /// </summary>
        /// <param name="index">���ı���������</param>
        /// <param name="text"></param>
        public void InsertPlainText(int index, string text)
        {
            if(RootNode.Children.Count == 0)
            {
                RootNode.InsertChild(0, new InnerTextNode(text));
            }
            else
            {
                InnerTextNode node = null;
                foreach(InnerTextNode child in this.Where(t=>t is InnerTextNode))
                {
                    if(index - child.Length <= 0)
                    {
                        node = child;
                        break;
                    }
                    index -= child.Length;
                }
                node.Insert(index, text);
            }
        }

        /// <summary>
        /// �Ƴ����ı�
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        public void RemovePlainText(int startIndex, int endIndex)
        {
            //��ȡ��Ҫɾ����Ԫ��
            var index = 0;
            List<(InnerTextNode node, int index, int length)> textList = new List<(InnerTextNode, int, int)>();
            List<EmptyTagNode> emptyTagList = new List<EmptyTagNode>();
            foreach (var child in this)
            {
                switch (child)
                {
                    case InnerTextNode text:
                        index += text.Length;
                        if(index >= startIndex)
                        {
                            int length = Math.Min(Math.Min(index, endIndex) - startIndex, text.Length);
                            textList.Add((text, text.Length - length, length));
                        }
                        break;
                    case EmptyTagNode emptyTag:
                        emptyTagList.Add(emptyTag);
                        break;
                }
                if(index >= endIndex)
                {
                    break;
                }
            }

            //ɾ��Ԫ��
            foreach((var n, var i, var l) in textList)
            {
                n.Remove(i, l);
            }
            foreach(var n in emptyTagList)
            {
                n.Parent.RemoveChild(n);
            }
        }

        /// <summary>
        /// �滻���ı�
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="replaceText"></param>
        public void ReplacePlainText(int startIndex, int endIndex, string replaceText)
        {
            //��ȡ��Ҫɾ����Ԫ��
            var index = 0;
            List<(InnerTextNode node, int index, int length)> textList = new List<(InnerTextNode, int, int)>();
            List<EmptyTagNode> emptyTagList = new List<EmptyTagNode>();
            foreach (var child in this)
            {
                switch (child)
                {
                    case InnerTextNode text:
                        index += text.Length;
                        if (index >= startIndex)
                        {
                            int length = Math.Min(Math.Min(index, endIndex) - startIndex, text.Length);
                            textList.Add((text, text.Length - length, length));
                        }
                        break;
                    case EmptyTagNode emptyTag:
                        emptyTagList.Add(emptyTag);
                        break;
                }
                if (index >= endIndex)
                {
                    break;
                }
            }

            //ɾ�����˵�һ���ı�Ԫ���������Ԫ��
            var first = textList.First();
            textList.RemoveAt(0);
            foreach ((var n, var i, var l) in textList)
            {
                n.Remove(i, l);
            }
            foreach (var n in emptyTagList)
            {
                n.Parent.RemoveChild(n);
            }

            //���滻�ı������һ���ı�Ԫ�غ�ɾ��ԭ�ȵ��ı�
            (var textNode, var start, var len) = first;
            textNode.Insert(textNode.Length, replaceText);
            textNode.Remove(start, len);
        }

        /// <summary>
        /// ��ȡ���ı�
        /// </summary>
        /// <returns></returns>
        public string GetPlainText()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach(InnerTextNode elem in this.Where(n=>n is InnerTextNode))
            {
                stringBuilder.Append(elem.Text);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// ��ȡԴ�ı�
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return RootNode.ToString();
        }

        public IEnumerator<RichTextNode> GetEnumerator()
        {
            return RootNode.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    /// <summary>
    /// ���ı������ڵ�
    /// </summary>
    public abstract class RichTextNode
    {
        /// <summary>
        /// ���ڵ�
        /// </summary>
        public PairTagNode Parent { get; internal protected set; }

        /// <summary>
        /// ��һ���ֵܽڵ�
        /// </summary>
        public RichTextNode NextSibling
        {
            get
            {
                if(Parent != null)
                {
                    int index = Parent.Children.IndexOf(this) + 1;
                    if(index != Parent.Children.Count)
                    {
                        return Parent.Children[index];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// ��һ���ֵܽڵ�
        /// </summary>
        public RichTextNode PrevSibling
        {
            get
            {
                if (Parent != null)
                {
                    int index = Parent.Children.IndexOf(this) - 1;
                    if (index != -1)
                    {
                        return Parent.Children[index];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// ��̽ڵ�
        /// </summary>
        public RichTextNode Successor => NextSibling ?? Parent.Successor;

        /// <summary>
        /// ǰ���ڵ�
        /// </summary>
        public RichTextNode Precursor => PrevSibling ?? Parent;

        /// <summary>
        /// �ڵ���ԭ�ı��е���ʼλ��
        /// </summary>
        /// <remarks>
        /// �ýڵ��һ���ַ����ڵ�����
        /// </remarks>
        public virtual int StartIndex => Parent.EndIndex;

        /// <summary>
        /// �ڵ���ԭ�ı��е���ֹλ��
        /// </summary>
        /// <remarks>
        /// ��̽ڵ��һ���ַ����ڵ�����
        /// </remarks>
        public abstract int EndIndex { get; }

        /// <summary>
        /// �ı�����
        /// </summary>
        public virtual int Length => EndIndex - StartIndex;
    }

    /// <summary>
    /// ��ǩ�ڵ�
    /// </summary>
    public abstract class TagNode : RichTextNode
    {
        /// <summary>
        /// ��ǩ����ֵ
        /// </summary>
        public string Attr;

        /// <summary>
        /// �ڵ�����
        /// </summary>
        public readonly string Tag;

        public TagNode(string tag, string attr)
        {
            Tag = tag;
            Attr = attr;
        }
    }

    /// <summary>
    /// �ɶԱ�ǩ�ڵ�
    /// </summary>
    public class PairTagNode : TagNode, IEnumerable<RichTextNode>
    {
        /// <summary>
        /// �ڲ�Ԫ����ʼ����
        /// </summary>
        /// <remarks>
        /// �ڲ�Ԫ�ص�һ���ַ����ڵ�λ��
        /// </remarks>
        public virtual int InnerStartIndex => StartIndex + 2 + Attr.Length; //2Ϊ<>�ĳ���

        /// <summary>
        /// �ڲ�Ԫ����ֹ����
        /// </summary>
        /// <remarks>
        /// �ձ�ǩ��һ��Ԫ������λ��
        /// </remarks>
        public virtual int InnerEndIndex => _children.Last()?.EndIndex ?? InnerStartIndex;//����������ӽڵ㣬��Ϊ�ڲ�Ԫ����ʼ����

        public override int EndIndex => InnerEndIndex + 3 + Tag.Length;//3Ϊ</>�ĳ���

        /// <summary>
        /// �ӽڵ�
        /// </summary>
        protected List<RichTextNode> _children = new List<RichTextNode>();

        /// <summary>
        /// <inheritdoc cref="_children"/>
        /// </summary>
        public ReadOnlyCollection<RichTextNode> Children => _children.AsReadOnly();

        public PairTagNode(string tag, string attr) : base(tag, attr) { }

        /// <summary>
        /// �����ӽڵ�
        /// </summary>
        /// <param name="node"></param>
        public void AddChild(RichTextNode node)
        {
            _children.Add(node);
            node.Parent = this;
        }

        /// <summary>
        /// �����ӽڵ�
        /// </summary>
        /// <param name="index"></param>
        /// <param name="node"></param>
        public void InsertChild(int index, RichTextNode node)
        {
            _children.Insert(index, node);
            node.Parent = this;
        }

        /// <summary>
        /// �Ƴ��ӽڵ�
        /// </summary>
        /// <param name="node"></param>
        public void RemoveChild(RichTextNode node)
        {
            _children.Remove(node);
            node.Parent = null;
            if(_children.Count == 0)
            {
                Parent?.RemoveChild(this);
            }
        }

        /// <summary>
        /// ��ȡ�ڲ�Ԫ���ı�
        /// </summary>
        /// <returns></returns>
        public string GetInnerRawText()
        {
            var stringBuilder = new StringBuilder();
            foreach (var child in _children)
            {
                stringBuilder.Append(child.ToString());
            }
            return stringBuilder.ToString();
        }

        public override string ToString()
        {
            return $"<{Attr}>{GetInnerRawText()}</{Tag}>";
        }

        public IEnumerator<RichTextNode> GetEnumerator()
        {
            for(int i = 0; i < _children.Count; i++)
            {
                yield return _children[i];
                var pair = _children[i] as PairTagNode;
                if(pair != null)
                {
                    foreach(var child in pair)
                    {
                        yield return child;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// �ձ�ǩ�ڵ�
    /// </summary>
    public class EmptyTagNode : TagNode
    {
        public override int EndIndex => StartIndex + 2 + Attr.Length;

        public override int Length => 2 + Attr.Length;

        public EmptyTagNode(string tag, string value) : base(tag, value) { }

        public override string ToString()
        {
            return $"<{Attr}>";
        }
    }

    /// <summary>
    /// ����ǩ�ڵ�
    /// </summary>
    public class RootNode : PairTagNode
    {
        public override int StartIndex => 0;
        public override int EndIndex => InnerEndIndex;

        public override int InnerStartIndex => StartIndex;

        public RootNode() : base("body", null) { }
        public override string ToString()
        {
            return GetInnerRawText();
        }
    }

    /// <summary>
    /// �ڲ����ֽڵ�
    /// </summary>
    public class InnerTextNode : RichTextNode
    {
        public string Text { get;protected set; }
        public override int EndIndex => StartIndex + Text.Length;

        public override int Length => Text.Length;

        public InnerTextNode(string text)
        {
            Text = text;
        }

        public void Insert(int startIndex, string value)
        {
            Text = Text.Insert(startIndex, value);
        }

        public void Remove(int startIndex, int length)
        {
            Text = Text.Remove(startIndex, length);
            if (string.IsNullOrEmpty(Text))
            {
                Parent?.RemoveChild(this);
            }
        }

        public override string ToString()
        {
            return Text;
        }
    }

    public class RichTextTreeException : Exception
    {
        public RichTextNode Node;
        public RichTextTreeException(RichTextNode node, string message) : base(message) { }
    }
}
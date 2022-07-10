using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace GameFrame.Behavior.Tree
{
    [Serializable]
    [Node("#E4007F")]
    public abstract class BaseNode
    {

        #region �༭����س�Ա
#if UNITY_EDITOR
        /// <summary>
        /// ����
        /// </summary>
        public string Name;
        /// <summary>
        /// ��ͼλ��
        /// </summary>
        public Vector2 ViewPosition;
        /// <summary>
        /// ���ڵ�߿���ɫ�ı�ʱ
        /// </summary>
        public event Action<Color> OnColorChanged;
        /// <summary>
        /// ������Դ��
        /// </summary>
        /// <param name="sourceInfo"></param>
        public void AddSource(SourceInfo sourceInfo)
        {
            _sources.Add(sourceInfo);
        }
        /// <summary>
        /// �Ƴ���Դ��
        /// </summary>
        /// <param name="sourceInfo"></param>
        public void RemoveSource(SourceInfo sourceInfo)
        {
            _sources.Remove(sourceInfo);
        }
        /// <summary>
        /// ����Դ���Ƴ����й�����Դ��
        /// </summary>
        /// <param name="source"></param>
        public void RemoveSource(BaseNode source)
        {
            _sources.RemoveAll(info=>info.SourceNode == source);
        }
        /// <summary>
        /// ���Ľڵ�߿���ɫ
        /// </summary>
        public void ChangeColor(Color color)
        {
            OnColorChanged?.Invoke(color);
        }
#endif
        #endregion

        /// <summary>
        /// Ψһ��ʶ��
        /// </summary>
        public string Guid;

        /// <summary>
        /// �ڵ�󶨵���Ϊ��
        /// </summary>
        public BehaviorTree BehaviorTree { get; private set; }

        /// <summary>
        /// ��Դ����Ϣ
        /// </summary>
        [SerializeField]
        List<SourceInfo> _sources = new List<SourceInfo>();

        /// <summary>
        /// ��һ�θ���ʱ��
        /// </summary>
        protected float _lastUpdataTime = 0;

        /// <summary>
        /// ��ȡָ�������ֶε�ֵ
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        protected abstract object GetValue(string fieldInfo);
        /// <summary>
        /// ����֡��һ�θ������ݶ˿�
        /// </summary>
        protected void Init()
        {
            _sources.ForEach(info =>
            {
                var obj = info.SourceNode.GetValue(info.SourceField);
                var type = GetType();
                do
                {
                    var field = GetType().GetField(info.TargetField);
                    if (field != null)
                    {
                        field.SetValue(this, obj);
                        break;
                    }
                } while (type != typeof(BaseNode));
            });
        }

        /// <summary>
        /// ��¡��ǰ�ڵ㼰�����ڵ�
        /// </summary>
        public virtual BaseNode Clone(BehaviorTree tree)
        {
            var node = Activator.CreateInstance(GetType()) as BaseNode;
            node.Guid = Guid;
            node._lastUpdataTime = 0;

            tree.Nodes.Add(node);
            node.BehaviorTree = tree;
            node._sources.Clear();
            _sources.ForEach(info =>
            {
                var source = tree.Nodes.Find(node => node.Guid == info.SourceNode.Guid);
                if(source == null) 
                { 
                    source = info.SourceNode.Clone(tree);
                }
                node._sources.Add(new SourceInfo(source, info.SourceField, info.TargetField));
            });
            return node;
        }
    }
}

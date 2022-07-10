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

        #region 编辑器相关成员
#if UNITY_EDITOR
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 视图位置
        /// </summary>
        public Vector2 ViewPosition;
        /// <summary>
        /// 当节点边框颜色改变时
        /// </summary>
        public event Action<Color> OnColorChanged;
        /// <summary>
        /// 增加资源边
        /// </summary>
        /// <param name="sourceInfo"></param>
        public void AddSource(SourceInfo sourceInfo)
        {
            _sources.Add(sourceInfo);
        }
        /// <summary>
        /// 移除资源边
        /// </summary>
        /// <param name="sourceInfo"></param>
        public void RemoveSource(SourceInfo sourceInfo)
        {
            _sources.Remove(sourceInfo);
        }
        /// <summary>
        /// 根据源点移除所有关联资源边
        /// </summary>
        /// <param name="source"></param>
        public void RemoveSource(BaseNode source)
        {
            _sources.RemoveAll(info=>info.SourceNode == source);
        }
        /// <summary>
        /// 更改节点边框颜色
        /// </summary>
        public void ChangeColor(Color color)
        {
            OnColorChanged?.Invoke(color);
        }
#endif
        #endregion

        /// <summary>
        /// 唯一标识符
        /// </summary>
        public string Guid;

        /// <summary>
        /// 节点绑定的行为树
        /// </summary>
        public BehaviorTree BehaviorTree { get; private set; }

        /// <summary>
        /// 资源边信息
        /// </summary>
        [SerializeField]
        List<SourceInfo> _sources = new List<SourceInfo>();

        /// <summary>
        /// 上一次更新时间
        /// </summary>
        protected float _lastUpdataTime = 0;

        /// <summary>
        /// 获取指定名称字段的值
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        protected abstract object GetValue(string fieldInfo);
        /// <summary>
        /// 当该帧第一次更新数据端口
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
        /// 克隆当前节点及关联节点
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

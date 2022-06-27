using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// ���ѡ��ڵ�
    /// </summary>
    public class RandomSelector : CompositeNode
    {
        [System.Serializable]
        private class WeightInfo
        {
            /// <summary>
            /// ��ǰȨ��
            /// </summary>
            public int Weight;
            /// <summary>
            /// ����Ȩ��
            /// </summary>
            public int BaseWeight;
            /// <summary>
            /// Ȩ��ÿһ�������Ķ�Ӧֵ
            /// </summary>
            public int DeltaWeight;
            /// <summary>
            /// Ȩ���Ƿ������
            /// </summary>
            public bool IsReset;
        }

        private int _current;

        /// <summary>
        /// �ڵ��������Ȩ��
        /// </summary>
        /// <remarks>
        /// Ȩ��Խ�߳鵽�ĸ���Խ��
        /// </remarks>
        [SerializeField]
        private List<WeightInfo> _weights;

        private int _totalWight
        {
            get
            {
                int total = 0;
                _weights.ForEach(w => total += w.Weight);
                return total;
            }
        }

#if UNITY_EDITOR
        public override void AddChild(Node node)
        {
            base.AddChild(node);
            _weights.Add(new WeightInfo());
        }
        public override void RemoveChild(Node node)
        {
            int i = Childrens.IndexOf(node);
            base.RemoveChild(node);
            _weights.RemoveAt(i);
        }
#endif

        protected override void OnEnter()
        {
            _current = -1;
            int total = _totalWight;
            int value = Random.Range(0, total + 1);
            for(int i = 0; i < _weights.Count; ++i)
            {
                if (_current < 0 && value <= _weights[i].Weight)
                {
                    _current = i;
                    if (!_weights[i].IsReset) { _weights[i].Weight = _weights[i].BaseWeight; }
                    continue;
                }
                value -= _weights[i].Weight;
                _weights[i].Weight += _weights[i].DeltaWeight;
            }
        }

        protected override NodeStatus OnUpdate()
        {
            return Childrens[_current].Tick();
        }
    }
}

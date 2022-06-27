using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// �ڰ����
    /// </summary>
    public abstract class BlackBoardBase
    {
        public abstract object this[string index] { get; set; }

#if UNITY_EDITOR
        /// <summary>
        /// ��ȡ��Ӧ���͵��Ѷ���ı����б�
        /// </summary>
        /// <typeparam name="T">ָ������</typeparam>
        /// <returns></returns>
        public abstract List<KeyValuePair<string, object>> GetDefineList(System.Type type);

        /// <summary>
        /// ��ȡȫ���Ѷ���ı����б�
        /// </summary>
        /// <returns></returns>
        public abstract List<KeyValuePair<string, object>> GetAllDefineList();
#endif
    }

    /// <summary>
    /// �ڰ�
    /// </summary>
    public class BlackBoard: BlackBoardBase
    {
        /// <summary>
        /// �󶨵���
        /// </summary>
        public BehaviorTree Tree => _tree;
        private BehaviorTree _tree;
        /// <summary>
        /// �󶨵��������
        /// </summary>
        public BehaviorTreeRunner Runner => _runner;
        private BehaviorTreeRunner _runner;

#if UNITY_EDITOR
        /// <summary>
        /// ���ر���ģ���
        /// </summary>
        public Dictionary<string, object> LocalVariablesModel;
        /// <summary>
        /// �����ر���ģ��ı�ʱ
        /// </summary>
        public event System.Action<string, object, object> LocalValueChanged;
        /// <summary>
        /// �����ر���ģ������ʱ
        /// </summary>
        public event System.Action<string, object> LocalValueAdd;
        /// <summary>
        /// �����ر���ģ�����ʱ
        /// </summary>
        public event System.Action<string> LocalValueRemove;

        /// <summary>
        /// �����������仯ʱ
        /// </summary>
        public event System.Action<string, string> IndexChanged;
#endif

        /// <summary>
        /// ��ȡ���ر������ȫ�ֱ������еı���
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override object this[string key]
        {
            get
            {
                if (key.StartsWith("Local")) return Runner.LocalVariables[key.Substring(6)];
                return GlobalVariables.Instance.Variables[key.Substring(7)];
            }
            set
            {
                if (key.StartsWith("Local")) Runner.LocalVariables[key.Substring(6)] = value;
                GlobalVariables.Instance.Variables[key.Substring(7)] = value;
            }
        }

        internal BlackBoard(BehaviorTree tree)
        {
            _tree = tree;
            GlobalVariables.Instance.IndexChanged += (newIndex, oldIndex) => IndexChanged(newIndex, oldIndex);
        }
        public BlackBoard CreateRuntimeBlackBoard(BehaviorTreeRunner runner)
        {
            var runtime = new BlackBoard(_tree);
            runtime._runner = runner;
            return runtime;
        }
#if UNITY_EDITOR
        /// <summary>
        /// ��ȡ��Ӧ���͵��Ѷ���ı����б�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override List<KeyValuePair<string, object>> GetDefineList(System.Type type)
        {
            var list = new List<KeyValuePair<string, object>>();
            foreach (var item in LocalVariablesModel)
            {
                if (type.IsEquivalentTo(item.Value.GetType()))
                    list.Add(new KeyValuePair<string, object>(item.Key, item.Value));
            }
            return list;
        }

        /// <summary>
        /// ��ȡȫ���Ѷ���ı����б�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override List<KeyValuePair<string, object>> GetAllDefineList()
        {
            var list = new List<KeyValuePair<string, object>>();
            foreach (var item in LocalVariablesModel)
            {
                list.Add(new KeyValuePair<string, object>(item.Key, item.Value));
            }
            return list;
        }
#endif
    }
}

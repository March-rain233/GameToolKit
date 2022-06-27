using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// 黑板基类
    /// </summary>
    public abstract class BlackBoardBase
    {
        public abstract object this[string index] { get; set; }

#if UNITY_EDITOR
        /// <summary>
        /// 获取对应类型的已定义的变量列表
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <returns></returns>
        public abstract List<KeyValuePair<string, object>> GetDefineList(System.Type type);

        /// <summary>
        /// 获取全部已定义的变量列表
        /// </summary>
        /// <returns></returns>
        public abstract List<KeyValuePair<string, object>> GetAllDefineList();
#endif
    }

    /// <summary>
    /// 黑板
    /// </summary>
    public class BlackBoard: BlackBoardBase
    {
        /// <summary>
        /// 绑定的树
        /// </summary>
        public BehaviorTree Tree => _tree;
        private BehaviorTree _tree;
        /// <summary>
        /// 绑定的运行组件
        /// </summary>
        public BehaviorTreeRunner Runner => _runner;
        private BehaviorTreeRunner _runner;

#if UNITY_EDITOR
        /// <summary>
        /// 本地变量模板表
        /// </summary>
        public Dictionary<string, object> LocalVariablesModel;
        /// <summary>
        /// 当本地变量模板改变时
        /// </summary>
        public event System.Action<string, object, object> LocalValueChanged;
        /// <summary>
        /// 当本地变量模板增加时
        /// </summary>
        public event System.Action<string, object> LocalValueAdd;
        /// <summary>
        /// 当本地变量模板减少时
        /// </summary>
        public event System.Action<string> LocalValueRemove;

        /// <summary>
        /// 变量表索引变化时
        /// </summary>
        public event System.Action<string, string> IndexChanged;
#endif

        /// <summary>
        /// 获取本地变量表和全局变量表中的变量
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
        /// 获取对应类型的已定义的变量列表
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
        /// 获取全部已定义的变量列表
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

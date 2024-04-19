using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using GameToolKit.Editor;
namespace GameToolKit.Behavior.Tree.Editor
{
    /// <summary>
    /// ÐÐÎªÊ÷±à¼­Æ÷
    /// </summary>
    public class BehaviorTreeEditor : DataFlowGraphEditor<BehaviorTree, Node>
    {
        private BehaviorTreeView TreeView => _view as BehaviorTreeView;

        [MenuItem("GameToolKit/BTree Editor")]
        public static void ShowMenu()
        {
            BehaviorTreeEditor wnd = GetWindow<BehaviorTreeEditor>();
            wnd.titleContent = new GUIContent("BTree Editor");
        }
        protected override void CreateGUI()
        {
            base.CreateGUI();
            VisualElement root = rootVisualElement;

            root.Q("blackboard-toggle").RegisterCallback((ChangeEvent<bool> e) =>
            {
                TreeView.ShowBlackboard(e.newValue);
            });
        }

        protected override BehaviorTree GetSelectionGraph(Object obj) => obj switch
        {
            GameObject gameObject when gameObject.TryGetComponent<BehaviorTreeRunner>(out var runner) 
                && EditorApplication.isPlaying => runner.RunTree,
            GameObject gameObject when gameObject.TryGetComponent<BehaviorTreeRunner>(out var runner)
                && !EditorApplication.isPlaying => runner.ModelTree,
            _ => base.GetSelectionGraph(obj),
        };
    }
}
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using GameFrame.Behavior.Tree;
using GameFrame.Interface;

namespace GameFrame.Editor
{
    public class BehaviorTreeEditor : EditorWindow
    {
        public VisualTreeAsset VisualTreeAsset;
        public StyleSheet StyleSheet;

        private TreeView _treeView;
        private InspectorView _inspectorView;
        private Label _label;

        [MenuItem("ǳ����ι���/��Ϊ���༭��")]
        public static void ShowMenu()
        {
            BehaviorTreeEditor wnd = GetWindow<BehaviorTreeEditor>();
            wnd.titleContent = new GUIContent("��Ϊ���༭��");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            VisualTreeAsset.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            root.styleSheets.Add(StyleSheet);

            _treeView = root.Q<TreeView>();
            _inspectorView = root.Q<InspectorView>();
            _label = root.Q<Label>("name");
            root.Q<ToolbarButton>("load").clicked += LoadAsset;
            root.Q<ToolbarButton>("loadNode").clicked += LoadNode;
            root.Q<ToolbarButton>("sort1").clicked += _treeView.SortDown;
            root.Q<ToolbarButton>("sort2").clicked += _treeView.SortMiddle;
            root.Q<ScrollView>().Add(_inspectorView);
            _treeView.OnElementSelected += element => _inspectorView.UpdateSelection(element);
            OnSelectionChange();
        }

        private void LoadNode()
        {
            string path = EditorUtility.OpenFilePanel("ѡ��ڵ�", Application.dataPath, "asset");
            path = path.Replace(Application.dataPath, "Assets");
            Behavior.Tree.Node node = AssetDatabase.LoadAssetAtPath<Behavior.Tree.Node>(path);
            if (node == null)
            {
                Debug.LogError("�ļ�Υ��");
                return;
            }
            _treeView.AddSubtree(node.Clone());
        }

        private void LoadAsset()
        {
            string path = EditorUtility.OpenFilePanel("ѡ����Ϊ��", Application.dataPath, "asset");
            path = path.Replace(Application.dataPath, "Assets");
            ITree tree = AssetDatabase.LoadAssetAtPath<BehaviorTree>(path);
            if (tree == null)
            {
                Debug.Log("�ļ�Υ��");
                return;
            }
            LoadTree(tree);
        }

        private void LoadTree(ITree tree)
        {
            //if (!AssetDatabase.IsMainAsset(tree as UnityEngine.Object)) { return; }
            _label.text = (tree as Object).name + " View";
            _treeView.PopulateView(tree);
            _inspectorView.Clear();
        }

        private void OnSelectionChange()
        {
            ITree tree = Selection.activeObject as ITree;
            if (tree == null && Selection.activeObject is GameObject)
            {
                tree = Selection.activeGameObject.GetComponent<BehaviorTreeRunner>()?.RunTree;
                if (tree == null)
                {
                    tree = (Selection.activeGameObject.GetComponent<BehaviorTreeRunner>()?.ModelTreeSlot);
                }
            }
            //if (tree == null && Selection.activeObject is GameObject)
            //{
            //    tree = Selection.activeGameObject.GetComponent<TalkSystem>()?.DialogueTree;
            //}

            if (tree != null)
            {
                tree.CorrectnessChecking();
                LoadTree(tree);
            }
        }
    }
}
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
namespace GameToolKit.Editor
{
    /// <summary>
    /// 图编辑器基类
    /// </summary>
    public class DataFlowGraphEditor<TGraph, TNode> : EditorWindow 
        where TGraph : DataFlowGraph<TGraph, TNode>
        where TNode : BaseNode
    {
        public VisualTreeAsset VisualTreeAsset;
        public StyleSheet StyleSheet;
        protected TGraph _graph;
        protected DataFlowGraphView<TGraph, TNode> _view;
        protected Label _filename;
        protected GraphInspector _graphInspector;
        protected virtual Length _inspectorWidth => new Length(30, LengthUnit.Percent);

        protected virtual void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            VisualTreeAsset.CloneTree(root);
            root.styleSheets.Add(StyleSheet);

            _view = root.Q<DataFlowGraphView<TGraph, TNode>>();
            _filename = root.Q<Label>("filename-label");
            _graphInspector = root.Q<GraphInspector>("graph-inspector");
            _graphInspector.style.width = _inspectorWidth;
            var inspectorToggle = root.Q<ToolbarToggle>("inspector-toggle");
            inspectorToggle.RegisterCallback((ChangeEvent<bool> e) =>
            {
                var len = new Length(-_inspectorWidth.value, _inspectorWidth.unit);
                _graphInspector.style.right = e.newValue ? 0 : len;
            });
            var blackboardToggle = root.Q<ToolbarToggle>("blackboard-toggle");
            blackboardToggle.RegisterCallback((ChangeEvent<bool> e) => _view.ShowBlackboard(e.newValue));
            root.Q<ToolbarSearchField>("search-box").RegisterCallback((InputEvent e) =>_view.Search(e.newData));

            _view.Window = this;
            _view.Inspector = _graphInspector;
        }

        private void OnProjectChange()
        {
            if (_view != null && _graph == null)
            {
                _view.ClearView();
                _graphInspector.Tittle = "Unknow";
            }
        }

        protected virtual TGraph GetSelectionGraph(Object obj) => obj switch
        {
            TGraph assetGraph when AssetDatabase.Contains(assetGraph) => assetGraph,
            _ => null,
        };

        private void OnSelectionChange()
        {
            TGraph graph = GetSelectionGraph(Selection.activeObject);
            if(graph == null) return;
            _graphInspector.Tittle = graph.name;
            _filename.text = AssetDatabase.GetAssetPath(graph);
            _view.PopulateView(graph);
        }
    }
}

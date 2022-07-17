using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;

namespace GameFrame.Editor
{
    public class GraphInspector : GraphElement
    {
        private VisualElement _mainContainer;
        private VisualElement _contentContainer;
        private Label _title;
        public override string title { get => _title.text; set => _title.text = value; }
        public override VisualElement contentContainer => _contentContainer;
        public GraphInspector()
        {
            VisualTreeAsset visualTreeAsset = EditorGUIUtility.Load("Assets/Scripts/Editor/UXML/GraphInspector.uxml") as VisualTreeAsset;
            _mainContainer = visualTreeAsset.Instantiate();
            var titleContainer = this.Q("TitleContainer");
            base.hierarchy.Add(_mainContainer);
            base.capabilities |= (Capabilities.Resizable | Capabilities.Movable);
            base.style.overflow = Overflow.Hidden;
            base.style.position = Position.Absolute;
            base.style.width = 300;
            base.style.height = 400;
            base.style.right = 0;
            hierarchy.Add(new Resizer() { });
            base.contentContainer.Q<TemplateContainer>().style.flexGrow = 1;
            _contentContainer = this.Q("contentContainer");
            _title = this.Q<Label>("title");
            this.AddManipulator(new Dragger() { clampToParentEdges = true });
            RegisterCallback<WheelEvent>(e=>e.StopImmediatePropagation());
        }
    }
}

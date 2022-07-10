using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;

namespace GameFrame.Behavior.Tree.Editor
{
    public class TreeInspector : GraphElement
    {
        private VisualElement _mainContainer;
        private VisualElement _contentContainer;
        private Label _title;
        public override string title { get => _title.text; set => _title.text = value; }
        public override VisualElement contentContainer => _contentContainer;
        public TreeInspector()
        {
            VisualTreeAsset visualTreeAsset = EditorGUIUtility.Load("Assets/Scripts/NPC/BehaviorTree/Editor/UXML/TreeInspector.uxml") as VisualTreeAsset;
            _mainContainer = visualTreeAsset.Instantiate();
            base.hierarchy.Add(_mainContainer);
            base.capabilities |= (Capabilities.Resizable | Capabilities.Movable);
            base.style.overflow = Overflow.Hidden;
            ClearClassList();
            AddToClassList("treeInspector");
            hierarchy.Add(new Resizer() { });
            base.contentContainer.Q<TemplateContainer>().style.flexGrow = 1;
            _contentContainer = this.Q("contentContainer");
            _title = this.Q<Label>("title");

            bool dragging = false;
            Vector2 ori = Vector2.zero;
            Vector2 clickPos = Vector2.zero;
            var window = BehaviorTreeEditor.GetWindow<BehaviorTreeEditor>();
            RegisterCallback<MouseMoveEvent>(e =>
            {
                if (dragging)
                {
                    Vector2 pos = ori + e.mousePosition - clickPos;
                    pos = window.rootVisualElement.ChangeCoordinatesTo(hierarchy.parent, pos);
                    //因不知名bug会产生偏移，所以-20补足偏移
                    pos.y -= 20;
                    //框定范围
                    float h = hierarchy.parent.layout.height - style.height.value.value;
                    float w = hierarchy.parent.layout.width - style.width.value.value;
                    if (pos.y < 0)
                        pos.y = 0;
                    if(pos.x < 0)
                        pos.x = 0;
                    if(pos.y > h)
                        pos.y = h;
                    if(pos.x > w)
                        pos.x = w;
                    style.left = pos.x;
                    style.top = pos.y;
                }
            });
            RegisterCallback<MouseLeaveEvent>(e =>
            {
                dragging = false;
            });
            RegisterCallback<MouseUpEvent>(e =>
            {
                dragging = false;
            });
            RegisterCallback<MouseDownEvent>(e =>
            {
                dragging = true;
                ori = worldTransform.GetPosition();
                clickPos = e.mousePosition;
            });
            style.width = 200;
            style.height = 200;
        }
    }
}

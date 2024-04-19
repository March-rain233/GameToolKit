using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using GameToolKit;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor.UIElements;

namespace GameToolKit.Editor
{
    public class SourceEdgeView : Edge
    {
        public SyncType SyncType
        {
            get => _syncType;
            set
            {
                _syncType = value;
                _label.text = value == SyncType.Pull ? "⇢" : "⊱";
            }
        }
        SyncType _syncType;

        protected Label _label;

        public SourceEdgeView() : base()
        {
            _label = new Label()
            {
                style =
                {
                    color                   = Color.white,
                    fontSize                = 50,
                    unityTextAlign          = TextAnchor.UpperCenter,
                    paddingBottom           = 0,
                    paddingTop              = 0,
                    paddingRight            = 0,
                    paddingLeft             = 0,
                    borderBottomColor       = Color.black,
                    borderTopColor          = Color.black,
                    borderLeftColor         = Color.black,
                    borderRightColor        = Color.black,
                    borderBottomWidth       = 0,
                    borderTopWidth          = 0,
                    borderLeftWidth         = 0,
                    borderRightWidth        = 0,
                    borderBottomLeftRadius  = new Length(20, LengthUnit.Percent),
                    borderBottomRightRadius = new Length(20, LengthUnit.Percent),
                    borderTopLeftRadius     = new Length(20, LengthUnit.Percent),
                    borderTopRightRadius    = new Length(20, LengthUnit.Percent),
                }
            };

            Add(_label);
            _label.BringToFront();

            edgeControl.RegisterCallback<GeometryChangedEvent>(e =>
            {
                _label.transform.position = (edgeControl.controlPoints[1] + edgeControl.controlPoints[2] - _label.layout.size) / 2;
                var dir = (edgeControl.controlPoints[2] - edgeControl.controlPoints[1]).normalized;
                var angle = Vector2.SignedAngle(Vector2.right, dir);
                _label.transform.rotation = Quaternion.Euler(0, 0, angle);
                _label.style.color = Color.Lerp(edgeControl.outputColor, edgeControl.inputColor, 0.5f);
            });
        }

        public override void OnSelected()
        {
            base.OnSelected();
            _label.style.color = Color.Lerp(edgeControl.outputColor, edgeControl.inputColor, 0.5f);
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            _label.style.color = Color.Lerp(edgeControl.outputColor, edgeControl.inputColor, 0.5f);
        }
    }
}

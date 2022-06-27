using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System;

namespace GameFrame.Editor
{
    public class EdgeView : Edge
    {
        public Action<EdgeView> OnEdgeSelected;
    }
}
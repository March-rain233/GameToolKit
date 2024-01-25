using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GameToolKit.Editor
{
    internal class SourceEdgeConnectorListener : IEdgeConnectorListener
    {
        public void OnDrop(GraphView graphView, Edge edge)
        {
            var sourcePort = edge.output;
            var targetPort = edge.input;
            if (sourcePort != null && targetPort != null)
            {
                
            }
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            throw new NotImplementedException();
        }
    }
}

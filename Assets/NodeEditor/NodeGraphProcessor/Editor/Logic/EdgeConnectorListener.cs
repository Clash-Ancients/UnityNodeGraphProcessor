using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


namespace GraphProcessor
{
    
    public class BaseEdgeConnectorListener : IEdgeConnectorListener
    {
        
        public readonly BaseGraphView graphView;
        
        public BaseEdgeConnectorListener(BaseGraphView graphView)
        {
            this.graphView = graphView;
        }
        
        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
        }
    }
}


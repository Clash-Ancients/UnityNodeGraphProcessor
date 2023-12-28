using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphProcessor
{
    public class BaseEdgeConnector : EdgeConnector
    {
        protected BaseEdgeDragHelper dragHelper;
        Edge edgeCandidate;
        protected bool active;
        Vector2 mouseDownPosition;
        protected BaseGraphView graphView;

        internal const float k_ConnectionDistanceTreshold = 10f;
        
        public BaseEdgeConnector(IEdgeConnectorListener listener) : base()
        {
            graphView = (listener as BaseEdgeConnectorListener)?.graphView;
            active = false;
            InitEdgeConnector(listener);
        }

        protected virtual void InitEdgeConnector(IEdgeConnectorListener listener)
        {
            dragHelper = new BaseEdgeDragHelper(listener);
            //activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
        }
        
        protected override void RegisterCallbacksOnTarget()
        {
           
        }

        protected override void UnregisterCallbacksFromTarget()
        {
          
        }

        public override EdgeDragHelper edgeDragHelper { get; }
    }
}
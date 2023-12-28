using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphProcessor
{
    public class BaseEdgeDragHelper : EdgeDragHelper
    {
        protected readonly IEdgeConnectorListener listener;
        
        public BaseEdgeDragHelper(IEdgeConnectorListener listener)
        {
            this.listener = listener;
           
            Reset();
        }
        
        public override bool HandleMouseDown(MouseDownEvent evt)
        {
            return true;
        }

        public override void HandleMouseMove(MouseMoveEvent evt)
        {
           
        }

        public override void HandleMouseUp(MouseUpEvent evt)
        {
           
        }

        public override void Reset(bool didConnect = false)
        {
           
        }

        public override Edge edgeCandidate { get; set; }
        public override Port draggedPort { get; set; }
    }
}
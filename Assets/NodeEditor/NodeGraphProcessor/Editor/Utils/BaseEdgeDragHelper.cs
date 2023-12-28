using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphProcessor
{
    public class BaseEdgeDragHelper : EdgeDragHelper
    {
        protected readonly IEdgeConnectorListener listener;
        
        protected GraphView graphView;
        
        internal const int k_PanAreaWidth = 30;
        internal const int k_PanSpeed = 4;
        internal const int k_PanInterval = 10;
        internal const float k_MinSpeedFactor = 0.5f;
        internal const float k_MaxSpeedFactor = 7f;
        internal const float k_MaxPanSpeed = k_MaxSpeedFactor * k_PanSpeed;
        internal const float kPortDetectionWidth = 30;
        
        protected Dictionary<BaseNodeView, List<PortView>> compatiblePorts = new Dictionary<BaseNodeView, List<PortView>>();
        
        protected static NodeAdapter nodeAdapter = new NodeAdapter();
        
        private Vector3 panDiff = Vector3.zero;
        
        Vector2 lastMousePos;
        private IVisualElementScheduledItem panSchedule;
        private bool wasPanned;
        public BaseEdgeDragHelper(IEdgeConnectorListener listener)
        {
            this.listener = listener;
           
            Reset();
        }
        
        public override bool HandleMouseDown(MouseDownEvent evt)
        {
            #region init edgeCandidate
            
            //get mouse position
            var mousePosition = evt.mousePosition;

            if (null ==draggedPort || null == edgeCandidate)
                return false;

            graphView = draggedPort.GetFirstAncestorOfType<GraphView>();

            Debug.Log(null == graphView?"not mouse down port":"mouse down port");
            
            if (null == graphView)
            {
                return false;
            }

            if (null == edgeCandidate)
            {
                graphView.AddElement(edgeCandidate);
            }

            var isInput = draggedPort.direction == Direction.Input;

            edgeCandidate.candidatePosition = mousePosition;
            edgeCandidate.SetEnabled(false);
            
            if (isInput)
            {
                edgeCandidate.input = draggedPort;
                edgeCandidate.output = null;
            }
            else
            {
                edgeCandidate.input = null;
                edgeCandidate.output = draggedPort;
            }

            draggedPort.portCapLit = true;
            
            
            
            #endregion
            
            #region highlight compatible ports
            compatiblePorts.Clear();
            
            foreach (PortView port in graphView.GetCompatiblePorts(draggedPort, nodeAdapter))
            {
                compatiblePorts.TryGetValue(port.owner, out var portList);
                if (portList == null)
                    portList = compatiblePorts[port.owner] = new List<PortView>();
                portList.Add(port);
            }

            // Only light compatible anchors when dragging an edge.
            graphView.ports.ForEach((p) => {
                p.OnStartEdgeDragging();
            });

            foreach (var kp in compatiblePorts)
            foreach (var port in kp.Value)
                port.highlight = true;
            
            edgeCandidate.UpdateEdgeControl();
            #endregion
            
            #region register update edge callback : Pan
            if (panSchedule == null)
            {
                panSchedule = graphView.schedule.Execute(Pan).Every(k_PanInterval).StartingIn(k_PanInterval);
                panSchedule.Pause();
            }
            wasPanned = false;
            #endregion
            
            edgeCandidate.layer = Int32.MaxValue;
            return true;
        }

        private void Pan(TimerState ts)
        {
            graphView.viewTransform.position -= panDiff;

            // Workaround to force edge to update when we pan the graph
            edgeCandidate.output = edgeCandidate.output;
            edgeCandidate.input = edgeCandidate.input;

            edgeCandidate.UpdateEdgeControl();
            wasPanned = true;
        }
        internal Vector2 GetEffectivePanSpeed(Vector2 mousePos)
        {
            Vector2 effectiveSpeed = Vector2.zero;

            if (mousePos.x <= k_PanAreaWidth)
                effectiveSpeed.x = -(((k_PanAreaWidth - mousePos.x) / k_PanAreaWidth) + 0.5f) * k_PanSpeed;
            else if (mousePos.x >= graphView.contentContainer.layout.width - k_PanAreaWidth)
                effectiveSpeed.x = (((mousePos.x - (graphView.contentContainer.layout.width - k_PanAreaWidth)) / k_PanAreaWidth) + 0.5f) * k_PanSpeed;

            if (mousePos.y <= k_PanAreaWidth)
                effectiveSpeed.y = -(((k_PanAreaWidth - mousePos.y) / k_PanAreaWidth) + 0.5f) * k_PanSpeed;
            else if (mousePos.y >= graphView.contentContainer.layout.height - k_PanAreaWidth)
                effectiveSpeed.y = (((mousePos.y - (graphView.contentContainer.layout.height - k_PanAreaWidth)) / k_PanAreaWidth) + 0.5f) * k_PanSpeed;

            effectiveSpeed = Vector2.ClampMagnitude(effectiveSpeed, k_MaxPanSpeed);

            return effectiveSpeed;
        }
        
        
        public override void HandleMouseMove(MouseMoveEvent evt)
        {
            var ve = (VisualElement)evt.target;
            Vector2 gvMousePos = ve.ChangeCoordinatesTo(graphView.contentContainer, evt.localMousePosition);
            panDiff = GetEffectivePanSpeed(gvMousePos);

            if (panDiff != Vector3.zero)
                panSchedule.Resume();
            else
                panSchedule.Pause();

            Vector2 mousePosition = evt.mousePosition;
            lastMousePos =  evt.mousePosition;

            edgeCandidate.candidatePosition = mousePosition;
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
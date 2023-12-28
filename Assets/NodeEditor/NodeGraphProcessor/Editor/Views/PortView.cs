using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphProcessor
{
    
    public class PortView : Port
    {
        
        public BaseNodeView     	owner { get; private set; }
        
        public static PortView CreatePortView(Direction direction, FieldInfo fieldInfo, PortData portData,
            BaseEdgeConnectorListener edgeConnectorListener)
        {
            var pv = new PortView(direction, fieldInfo, portData, edgeConnectorListener);
            pv.m_EdgeConnector = new BaseEdgeConnector(edgeConnectorListener);
            pv.AddManipulator(pv.m_EdgeConnector);
            return pv;
        }
        protected PortView(Direction direction, FieldInfo fieldInfo, PortData portData, BaseEdgeConnectorListener edgeConnectorListener) : base(portData.vertical ? Orientation.Vertical : Orientation.Horizontal, direction, Capacity.Multi, portData.displayType ?? fieldInfo.FieldType)
        {
            
        }

        public virtual void Initialize(BaseNodeView nodeView, string name)
        {
            
            this.owner = nodeView;
            
            if (name != null)
                portName = name;
        }
    }
}


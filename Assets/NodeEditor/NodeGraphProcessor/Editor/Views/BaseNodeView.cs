using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphProcessor
{
    [BoxGroup]
    [HideLabel]
    [HideReferenceObjectPicker]
    public class BaseNodeView : Node
    {
        //[OnValueChanged(nameof(UpdateFieldValues), true)]
        public BaseNode							nodeTarget;
        
        public BaseGraphView					owner { private set; get; }
        
        [HideInInspector]
        public VisualElement 					controlsContainer;
        protected VisualElement					rightTitleContainer;
        protected VisualElement					topPortContainer;
        protected VisualElement					bottomPortContainer;
        private List<Node> selectedNodes = new List<Node>();

        readonly string							baseNodeStyle = "GraphProcessorStyles/BaseNodeView";
        
        private bool initializing = false;
        
        public void Initialize(BaseGraphView _owner, BaseNode node)
        {
            nodeTarget = node;

            owner = _owner;

            
            styleSheets.Add(Resources.Load<StyleSheet>(baseNodeStyle));
            
            initializing = true;
            
            InitializeView();
            InitializePorts();
            // If the standard Enable method is still overwritten, we call it
            if (GetType().GetMethod(nameof(Enable), new Type[]{}).DeclaringType != typeof(BaseNodeView))
	            ExceptionToLog.Call(() => Enable());
            else
	            ExceptionToLog.Call(() => Enable(false));
            
            
            RefreshExpandedState();

            this.RefreshPorts();
        }

        
       
        private void InitializePorts()
        {
	        var listener = owner.connectorListener;

	        foreach (var inputPort in nodeTarget.inputPorts)
	        {
		        AddPort(inputPort.fieldInfo, Direction.Input, listener, inputPort.portData);
	        }

	        foreach (var outputPort in nodeTarget.outputPorts)
	        {
		        AddPort(outputPort.fieldInfo, Direction.Output, listener, outputPort.portData);
	        }
        }

        private PortView AddPort(FieldInfo inputPortFieldInfo, Direction input, BaseEdgeConnectorListener listener, PortData portData)
        {
	        PortView p = CreatePortView(input, inputPortFieldInfo, portData, listener);

	        if (p.direction == Direction.Input)
	        {
		        //inputPortViews.Add(p);

		        if (portData.vertical)
			        topPortContainer.Add(p);
		        else
			        inputContainer.Add(p);
	        }
	        else
	        {
		        //outputPortViews.Add(p);

		        if (portData.vertical)
			        bottomPortContainer.Add(p);
		        else
			        outputContainer.Add(p);
	        }
	        
	        p.Initialize(this, portData?.displayName);
	        
	        return p;
        }

        protected virtual PortView CreatePortView(Direction direction, FieldInfo fieldInfo, PortData portData, BaseEdgeConnectorListener listener)
	        => PortView.CreatePortView(direction, fieldInfo, portData, listener);
        
        void InitializeView()
        {
	        
	        controlsContainer = new VisualElement{ name = "controls" };
	        controlsContainer.AddToClassList("NodeControls");
	        mainContainer.Add(controlsContainer);
	        
	        rightTitleContainer = new VisualElement{ name = "RightTitleContainer" };
	        titleContainer.Add(rightTitleContainer);
	        titleContainer.Insert(0, new VisualElement(){ name = "NodeIcon_Action" });
	        
	        
	        topPortContainer = new VisualElement { name = "TopPortContainer" };
	        this.Insert(0, topPortContainer);

	        bottomPortContainer = new VisualElement { name = "BottomPortContainer" };
	        this.Add(bottomPortContainer);
	        
	        UpdateTitle();
	        
	        SetPosition(nodeTarget.position);
        }

        private void UpdateTitle()
        {
	        title = (nodeTarget.GetCustomName() == null) ? nodeTarget.GetType().Name : nodeTarget.GetCustomName();
        }

        public override void SetPosition(Rect newPos)
        {
	        //if (initializing || !nodeTarget.isLocked)
	        if (initializing)
	        {
		        base.SetPosition(newPos);

		        // if (!initializing)
			       //  owner.RegisterCompleteObjectUndo("Moved graph node");

		        nodeTarget.position = newPos;
		        initializing = false;
	        }
        }

        #region [obsolute]base call
        public virtual void Enable(bool fromInspector = false) => DrawDefaultInspector(fromInspector);
        public virtual void Enable() => DrawDefaultInspector(false);

        public virtual void Disable() {}
        
        //DrawDefaultInspector
        protected virtual void DrawDefaultInspector(bool fromInspector = false)
		{
			var fields = nodeTarget.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				// Filter fields from the BaseNode type since we are only interested in user-defined fields
				// (better than BindingFlags.DeclaredOnly because we keep any inherited user-defined fields) 
				.Where(f => f.DeclaringType != typeof(BaseNode));

			fields = nodeTarget.OverrideFieldOrder(fields).Reverse();

			// foreach (var field in fields)
			// {
			// 	//skip if the field is a node setting
			// 	if(field.GetCustomAttribute(typeof(SettingAttribute)) != null)
			// 	{
			// 		hasSettings = true;
			// 		continue;
			// 	}
			//
			// 	//skip if the field is not serializable
			// 	if((!field.IsPublic && field.GetCustomAttribute(typeof(SerializeField)) == null) || field.IsNotSerialized)
			// 	{
			// 		AddEmptyField(field, fromInspector);
			// 		continue;
			// 	}
			//
			//
			// 	//skip if the field is an input/output and not marked as SerializedField
			// 	bool hasInputAttribute         = field.GetCustomAttribute(typeof(InputAttribute)) != null;
			// 	bool hasInputOrOutputAttribute = hasInputAttribute || field.GetCustomAttribute(typeof(OutputAttribute)) != null;
			// 	bool showAsDrawer			   = !fromInspector && field.GetCustomAttribute(typeof(ShowAsDrawer)) != null;
			// 	if (field.GetCustomAttribute(typeof(SerializeField)) == null && hasInputOrOutputAttribute && !showAsDrawer)
			// 	{
			// 		AddEmptyField(field, fromInspector);
			// 		continue;
			// 	}
			//
			// 	//skip if marked with NonSerialized or HideInInspector
			// 	if (field.GetCustomAttribute(typeof(System.NonSerializedAttribute)) != null || field.GetCustomAttribute(typeof(HideInInspector)) != null)
			// 	{
			// 		AddEmptyField(field, fromInspector);
			// 		continue;
			// 	}
			//
			// 	// Hide the field if we want to display in in the inspector
			// 	var showInInspector = field.GetCustomAttribute<ShowInInspector>();
			// 	if (showInInspector != null && !showInInspector.showInNode && !fromInspector)
			// 	{
			// 		AddEmptyField(field, fromInspector);
			// 		continue;
			// 	}
			//
			// 	var showInputDrawer = field.GetCustomAttribute(typeof(InputAttribute)) != null && field.GetCustomAttribute(typeof(SerializeField)) != null;
			// 	showInputDrawer |= field.GetCustomAttribute(typeof(InputAttribute)) != null && field.GetCustomAttribute(typeof(ShowAsDrawer)) != null;
			// 	showInputDrawer &= !fromInspector; // We can't show a drawer in the inspector
			// 	showInputDrawer &= !typeof(IList).IsAssignableFrom(field.FieldType);
			//
			// 	var elem = AddControlField(field, ObjectNames.NicifyVariableName(field.Name), showInputDrawer);
			// 	if (hasInputAttribute)
			// 	{
			// 		hideElementIfConnected[field.Name] = elem;
			//
			// 		// Hide the field right away if there is already a connection:
			// 		if (portsPerFieldName.TryGetValue(field.Name, out var pvs))
			// 			if (pvs.Any(pv => pv.GetEdges().Count > 0))
			// 				elem.style.display = DisplayStyle.None;
			// 	}
			// }
		}
        #endregion
    }
}


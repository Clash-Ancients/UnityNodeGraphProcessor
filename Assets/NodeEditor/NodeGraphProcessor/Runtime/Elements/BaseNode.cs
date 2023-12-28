using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GraphProcessor
{
	
	public delegate IEnumerable< PortData > CustomPortBehaviorDelegate(List< SerializableEdge > edges);
	public delegate IEnumerable< PortData > CustomPortTypeBehaviorDelegate(string fieldName, string displayName, object value);
	
    [BoxGroup]
    [HideLabel]
    [HideReferenceObjectPicker]
    public  abstract class BaseNode
    {
        private BaseGraph graph;
        
        
        internal class NodeFieldInformation
        {
	        public string						name;
	        public string						fieldName;
	        public FieldInfo					info;
	        public bool							input;
	        public bool							isMultiple;
	        public string						tooltip;
	        public CustomPortBehaviorDelegate	behavior;
	        public bool							vertical;
	        public bool                         showPortIcon;
	        public string                       portIconName;

	        public NodeFieldInformation(FieldInfo info, string name, bool input, bool isMultiple, string tooltip,
		        bool vertical, CustomPortBehaviorDelegate behavior, bool showPortIcon, string portIconName)
	        {
		        this.input = input;
		        this.isMultiple = isMultiple;
		        this.info = info;
		        this.name = name;
		        this.fieldName = info.Name;
		        this.behavior = behavior;
		        this.tooltip = tooltip;
		        this.vertical = vertical;
		        this.showPortIcon = showPortIcon;
		        this.portIconName = portIconName;
	        }
        }
        
        [HideInInspector]
        public string nodeCustomName = null; // The name of the node in case it was renamed by a user
        
        //id
        [HideInInspector]
        public string				GUID;
        
        
        public virtual bool         isLocked => nodeLock; 
        
        [HideInInspector]
        public bool                 nodeLock = false;
        
        /// <summary>
        /// Container of input ports
        /// </summary>
        [NonSerialized]
        public NodeInputPortContainer	inputPorts;
        /// <summary>
        /// Container of output ports
        /// </summary>
        [NonSerialized]
        public NodeOutputPortContainer	outputPorts;
        
        [HideInInspector]
        public Rect					position;
        
        public virtual string       name => GetType().Name;
        
        public void Initialize(BaseGraph _graph)
        {

            graph = _graph;
            
            inputPorts = new NodeInputPortContainer(this);
            outputPorts = new NodeOutputPortContainer(this);

            if (null == nodeFields)
            {
	            nodeFields = new Dictionary< string, NodeFieldInformation >();
            }
            
            InitializeInOutDatas();
            InitializePorts();
        }

        private void InitializePorts()
        {
	        foreach (var key in OverrideFieldOrder(nodeFields.Values.Select(k => k.info)))
	        {
		        var nodeField = nodeFields[key.Name];

		        // If we don't have a custom behavior on the node, we just have to create a simple port
		        AddPort(nodeField.input, nodeField.fieldName, new PortData { acceptMultipleEdges = nodeField.isMultiple, displayName = nodeField.name, tooltip = nodeField.tooltip, vertical = nodeField.vertical, showPortIcon = nodeField.showPortIcon, portIconName = nodeField.portIconName});
				
	        }
        }
        
        public void AddPort(bool input, string fieldName, PortData portData)
        {
	        // Fixup port data info if needed:
	        portData.displayType ??= nodeFields[fieldName].info.FieldType;

	        if (input)
		        inputPorts.Add(new NodePort(this, fieldName, portData));
	        else
		        outputPorts.Add(new NodePort(this, fieldName, portData));
        }
		
        #region ports

        [NonSerialized]
        internal Dictionary< string, NodeFieldInformation >	nodeFields = new Dictionary< string, NodeFieldInformation >();
        
        private bool _needsInspector = true;
        public virtual FieldInfo[] GetNodeFields()
	        => GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        private void InitializeInOutDatas()
        {
            var fields = GetNodeFields();
			var methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (var field in fields)
			{
				var inputAttribute = field.GetCustomAttribute< InputAttribute >();
				var outputAttribute = field.GetCustomAttribute< OutputAttribute >();
				var tooltipAttribute = field.GetCustomAttribute< TooltipAttribute >();
				// var showInInspector = field.GetCustomAttribute< ShowInInspector >();
				var vertical = field.GetCustomAttribute< VerticalAttribute >();
				// var showPortIconAttribute = field.GetCustomAttribute< ShowPortIconAttribute >();
				bool isMultiple = false;
				bool input = false;
				string name = field.Name;
				string tooltip = null;

				//if (showInInspector != null)
					_needsInspector = true;

				if (inputAttribute == null && outputAttribute == null)
					continue ;

				//check if field is a collection type
				isMultiple = (inputAttribute != null) ? inputAttribute.allowMultiple : (outputAttribute.allowMultiple);
				input = inputAttribute != null;
				tooltip = tooltipAttribute?.tooltip;

				if (!String.IsNullOrEmpty(inputAttribute?.name))
					name = inputAttribute.name;
				if (!String.IsNullOrEmpty(outputAttribute?.name))
					name = outputAttribute.name;

				bool _showPortIcon = true;
				string _portIconName = null;
				// if (showPortIconAttribute != null)
				// {
				// 	_showPortIcon = showPortIconAttribute.ShowIcon;
				// 	_portIconName = showPortIconAttribute.IconNameMatchedInUSSFile;
				// }
				// By default we set the behavior to null, if the field have a custom behavior, it will be set in the loop just below
				nodeFields[field.Name] = new NodeFieldInformation(field, name, input, isMultiple, tooltip, vertical != null, null, _showPortIcon, _portIconName);
			}

			foreach (var method in methods)
			{
				var customPortBehaviorAttribute = method.GetCustomAttribute< CustomPortBehaviorAttribute >();
				CustomPortBehaviorDelegate behavior = null;

				if (customPortBehaviorAttribute == null)
					continue ;

				// Check if custom port behavior function is valid
				try {
					var referenceType = typeof(CustomPortBehaviorDelegate);
					behavior = (CustomPortBehaviorDelegate)Delegate.CreateDelegate(referenceType, this, method, true);
				} catch {
					Debug.LogError("The function " + method + " cannot be converted to the required delegate format: " + typeof(CustomPortBehaviorDelegate));
				}

				if (nodeFields.ContainsKey(customPortBehaviorAttribute.fieldName))
					nodeFields[customPortBehaviorAttribute.fieldName].behavior = behavior;
				else
					Debug.LogError("Invalid field name for custom port behavior: " + method + ", " + customPortBehaviorAttribute.fieldName);
			}
        }
		#endregion
        /// <summary>
        /// Creates a node of type T at a certain position
        /// </summary>
        /// <param name="position">position in the graph in pixels</param>
        /// <typeparam name="T">type of the node</typeparam>
        /// <returns>the node instance</returns>
        public static T CreateFromType< T >(Vector2 position) where T : BaseNode
        {
            return CreateFromType(typeof(T), position) as T;
        }

        /// <summary>
        /// Creates a node of type nodeType at a certain position
        /// </summary>
        /// <param name="position">position in the graph in pixels</param>
        /// <typeparam name="nodeType">type of the node</typeparam>
        /// <returns>the node instance</returns>
        public static BaseNode CreateFromType(Type nodeType, Vector2 position)
        {
            if (!nodeType.IsSubclassOf(typeof(BaseNode)))
                return null;

            var node = Activator.CreateInstance(nodeType) as BaseNode;

            node.position = new Rect(position, new Vector2(100, 100));

            ExceptionToLog.Call(() => node.OnNodeCreated());

            return node;
        }
        
        public virtual void	OnNodeCreated() => GUID = Guid.NewGuid().ToString();
        
        public void SetCustomName(string customName) => nodeCustomName = customName;
        
        public string GetCustomName() => String.IsNullOrEmpty(nodeCustomName) ? name : nodeCustomName; 
        
        #region obsolute
        public virtual IEnumerable<FieldInfo> OverrideFieldOrder(IEnumerable<FieldInfo> fields)
        {
            long GetFieldInheritanceLevel(FieldInfo f)
            {
                int level = 0;
                var t = f.DeclaringType;
                while (t != null)
                {
                    t = t.BaseType;
                    level++;
                }

                return level;
            }

            // Order by MetadataToken and inheritance level to sync the order with the port order (make sure FieldDrawers are next to the correct port)
            return fields.OrderByDescending(f => (long)(((GetFieldInheritanceLevel(f) << 32)) | (long)f.MetadataToken));
        }
        #endregion
    }

}

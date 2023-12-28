using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GraphProcessor
{
    public class NodeProvider
    {
        private static NodeProvider _mInst;
        public static NodeProvider Inst => _mInst;

        public static void OnCreateNodeProvider()
        {
            if (null == _mInst)
            {
                _mInst = new NodeProvider();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "NodeProvider should only inst once", "OK");
            }


           
        }
        
        public static void OnResetNodeProvider()
        {
            _mInst = null;
        }
        
        NodeProvider()
        {
            BuildNodeViewCache();
            BuildGenericNodeCache();
        }

        #region link node and node view
        Dictionary< Type, Type >			nodeViewPerType = new Dictionary< Type, Type >();
        /// <summary>
        /// function: link node and nodeview
        /// example: CircleRadians link to CircleRadiansView
        /// </summary>
        void BuildNodeViewCache()
        {
            foreach (var nodeViewType in TypeCache.GetTypesDerivedFrom<BaseNodeView>())
            {
                if(nodeViewType.IsAbstract)
                    continue;
                UtilityAttribute.TryGetTypeAttribute<NodeCustomEditor>(nodeViewType, out var nodeCustomEditor);
                if (nodeCustomEditor != null)
                {
                    nodeViewPerType[nodeCustomEditor.nodeType] = nodeViewType;
                }
            }
        }
        
        public Type GetNodeViewTypeFromType(Type nodeType)
        {
            Type view;
            
            if (nodeViewPerType.TryGetValue(nodeType, out view))
                return view;

            Type baseType = null;

            // Allow for inheritance in node views: multiple C# node using the same view
            foreach (var type in nodeViewPerType)
            {
                // Find a view (not first fitted view) of nodeType
                if (nodeType.IsSubclassOf(type.Key) && (baseType == null || type.Value.IsSubclassOf(baseType)))
                    baseType = type.Value;
            }

            if (baseType != null)
                return baseType;

            return view;
        }
        #endregion
        
        #region cache generic node
        
        public struct PortDescription
        {
            public Type nodeType;
            public Type portType;
            public bool isInput;
            public string portFieldName;
            public string portIdentifier;
            public string portDisplayName;
        }
        
        public class NodeDescriptions
        {
            public Dictionary< string, Type >		nodePerMenuTitle = new Dictionary< string, Type >();
            public List< PortDescription >			nodeCreatePortDescription = new List<PortDescription>();
        }
        
        NodeDescriptions							genericNodes = new NodeDescriptions();
        private  void BuildGenericNodeCache()
        {
            foreach (var nodeType in TypeCache.GetTypesDerivedFrom<BaseNode>())
            {
                if (!IsNodeAccessibleFromMenu(nodeType))
                    continue;
                
                BuildCacheForNode(nodeType, genericNodes);
            }
        }

        private void BuildCacheForNode(Type nodeType, NodeDescriptions targetDescription, BaseGraph graph = null)
        {
            var attrs = nodeType.GetCustomAttributes(typeof(NodeMenuItemAttribute), false) as NodeMenuItemAttribute[];

            if (attrs != null && attrs.Length > 0)
            {
                foreach (var attr in attrs)
                    targetDescription.nodePerMenuTitle[attr.menuTitle] = nodeType;
            }
            
            ProvideNodePortCreationDescription(nodeType, targetDescription, graph);
        }

        public  IEnumerable<(string path, Type type)>	GetNodeMenuEntries(BaseGraph graph = null)
        {
            foreach (var node in genericNodes.nodePerMenuTitle)
                yield return (node.Key, node.Value);
            //
            // if (graph != null && specificNodeDescriptions.TryGetValue(graph, out var specificNodes))
            // {
            //     foreach (var node in specificNodes.nodePerMenuTitle)
            //         yield return (node.Key, node.Value);
            // }
        }
        
        readonly FieldInfo SetGraph = typeof(BaseNode).GetField("graph", BindingFlags.NonPublic | BindingFlags.Instance);
        
        void ProvideNodePortCreationDescription(Type nodeType, NodeDescriptions targetDescription, BaseGraph graph = null)
        {
            var node = Activator.CreateInstance(nodeType) as BaseNode;
            try {
                
                SetGraph.SetValue(node, graph);
                // node.InitializePorts();
                // node.UpdateAllPorts();
            }
            catch (Exception)
            {
                // ignored
            }

            // foreach (var p in node.inputPorts)
            //     AddPort(p, true);
            // foreach (var p in node.outputPorts)
            //     AddPort(p, false);
            //
            // void AddPort(NodePort p, bool input)
            // {
            //     targetDescription.nodeCreatePortDescription.Add(new PortDescription{
            //         nodeType = nodeType,
            //         portType = p.portData.displayType ?? p.fieldInfo.FieldType,
            //         isInput = input,
            //         portFieldName = p.fieldName,
            //         portDisplayName = p.portData.displayName ?? p.fieldName,
            //         portIdentifier = p.portData.identifier,
            //     });
            // }
        }
        
        bool IsNodeAccessibleFromMenu(Type nodeType)
        {
            if (nodeType.IsAbstract)
                return false;
            UtilityAttribute.TryGetTypeAttributes(nodeType,out var attributes);
            return attributes.Any();
        }

        #endregion
    }
}

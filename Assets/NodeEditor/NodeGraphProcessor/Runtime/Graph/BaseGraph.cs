using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GraphProcessor
{
    
    public class GraphChanges
    {
        // public SerializableEdge	removedEdge;
        // public SerializableEdge	addedEdge;
        public BaseNode			removedNode;
        public BaseNode			addedNode;
        public BaseNode			nodeChanged;
        // public Group			addedGroups;
        // public Group			removedGroups;
        // public BaseStackNode	addedStackNode;
        // public BaseStackNode	removedStackNode;
        // public StickyNote		addedStickyNotes;
        // public StickyNote		removedStickyNotes;
    }
    
    public class BaseGraph : SerializedScriptableObject
    {
        [HideInInspector]
        public Vector3					position = Vector3.zero;
        [HideInInspector]
        public Vector3					scale = Vector3.one;
        
        [System.NonSerialized]
        public Dictionary< string, BaseNode >			nodesPerGUID = new Dictionary< string, BaseNode >();
        
        public List< BaseNode >							nodes = new List< BaseNode >();
        
        public event Action< GraphChanges > onGraphChanges;
        
        public void OnGraphDisable()
        {
            
        }

        public void OnGraphEnable()
        {
            InitializeGraphElements();
        }
        
        void InitializeGraphElements()
        {
            // Sanitize the element lists (it's possible that nodes are null if their full class name have changed)
            // If you rename / change the assembly of a node or parameter, please use the MovedFrom() attribute to avoid breaking the graph.
            nodes.RemoveAll(n => n == null);
            //exposedParameters.RemoveAll(e => e == null);

            foreach (var node in nodes.ToList())
            {
                nodesPerGUID[node.GUID] = node;
                node.Initialize(this);
            }

            // foreach (var edge in edges.ToList())
            // {
            //     edge.Deserialize();
            //     edgesPerGUID[edge.GUID] = edge;
            //
            //     // Sanity check for the edge:
            //     if (edge.inputPort == null || edge.outputPort == null)
            //     {
            //         Disconnect(edge.GUID);
            //         continue;
            //     }
            //
            //     // Add the edge to the non-serialized port data
            //     edge.inputPort.owner.OnEdgeConnected(edge);
            //     edge.outputPort.owner.OnEdgeConnected(edge);
            // }
        }
        
        public void RemoveNode(BaseNode node)
        {
            
            nodesPerGUID.Remove(node.GUID);

            nodes.Remove(node);

            //onGraphChanges?.Invoke(new GraphChanges{ removedNode = node });
        }

        
        public BaseNode AddNode(BaseNode node)
        {
            nodesPerGUID[node.GUID] = node;

            nodes.Add(node);
            node.Initialize(this);

            onGraphChanges?.Invoke(new GraphChanges{ addedNode = node });

            return node;
        }
    }

}

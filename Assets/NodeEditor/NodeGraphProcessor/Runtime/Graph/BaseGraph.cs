using System;
using System.Collections.Generic;
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

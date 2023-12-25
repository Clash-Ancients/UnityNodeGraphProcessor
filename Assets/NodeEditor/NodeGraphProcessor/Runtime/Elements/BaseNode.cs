using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GraphProcessor
{
    [BoxGroup]
    [HideLabel]
    [HideReferenceObjectPicker]
    public  abstract class BaseNode
    {
        private BaseGraph graph;
        
        [HideInInspector]
        public string nodeCustomName = null; // The name of the node in case it was renamed by a user
        
        //id
        [HideInInspector]
        public string				GUID;
        
        [HideInInspector]
        public Rect					position;
        
        public virtual string       name => GetType().Name;
        
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

        public void Initialize(BaseGraph graph)
        {
            
        }
        
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
    }

}

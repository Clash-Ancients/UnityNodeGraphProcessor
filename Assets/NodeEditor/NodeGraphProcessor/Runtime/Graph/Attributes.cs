using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GraphProcessor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class NodeMenuItemAttribute : Attribute
    {
        public string	menuTitle;
        public Type		onlyCompatibleWithGraph;

        /// <summary>
        /// Register the node in the NodeProvider class. The node will also be available in the node creation window.
        /// </summary>
        /// <param name="menuTitle">Path in the menu, use / as folder separators</param>
        public NodeMenuItemAttribute(string menuTitle = null, Type onlyCompatibleWithGraph = null)
        {
            this.menuTitle = menuTitle;
            this.onlyCompatibleWithGraph = onlyCompatibleWithGraph;
        }
    }
    
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class InputAttribute : Attribute
    {
        public string		name;
        public bool			allowMultiple = false;

        /// <summary>
        /// Mark the field as an input port
        /// </summary>
        /// <param name="name">display name</param>
        /// <param name="allowMultiple">is connecting multiple edges allowed</param>
        public InputAttribute(string name = null, bool allowMultiple = false)
        {
            this.name = name;
            this.allowMultiple = allowMultiple;
        }
    }
    
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class OutputAttribute : Attribute
    {
        public string		name;
        public bool			allowMultiple = true;

        /// <summary>
        /// Mark the field as an output port
        /// </summary>
        /// <param name="name">display name</param>
        /// <param name="allowMultiple">is connecting multiple edges allowed</param>
        public OutputAttribute(string name = null, bool allowMultiple = true)
        {
            this.name = name;
            this.allowMultiple = allowMultiple;
        }
    }
}

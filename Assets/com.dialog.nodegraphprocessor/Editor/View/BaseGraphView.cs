using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphProcessor
{
    
    public class BaseGraphView : GraphView, IDisposable
    {
        public BaseGraphView(EditorWindow window)
        {
            
        }
    
        public void Dispose()
        {
       
        }
    }

}

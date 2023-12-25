using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphProcessor
{
    
    public class BaseGraphView : GraphView, IDisposable
    {
        
        public BaseGraph							graph;
        CreateNodeMenuWindow						createNodeMenu;
        
        public void Dispose()
        {
            
        }

        public BaseGraphView(EditorWindow window)
        {
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            SetupZoom(0.05f, 2f);
            
            this.StretchToParentSize();
            
            createNodeMenu = ScriptableObject.CreateInstance< CreateNodeMenuWindow >();
            createNodeMenu.Initialize(this, window);
        }

        public void Initialize(BaseGraph _graph)
        {
            graph = _graph;
            InitializeGraphView();
        }

        private void InitializeGraphView()
        {
            viewTransform.position = graph.position;
            viewTransform.scale = graph.scale;
            nodeCreationRequest = (c) => SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), createNodeMenu);
        }
    }
}


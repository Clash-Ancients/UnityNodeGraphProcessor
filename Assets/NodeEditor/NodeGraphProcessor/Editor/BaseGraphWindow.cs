using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphProcessor
{
    
    public abstract class BaseGraphWindow : EditorWindow
    {
        protected BaseGraphView		graphView;
        protected VisualElement		rootView;
        
        [SerializeField]
        protected BaseGraph			graph;

        readonly string				graphWindowStyle = "GraphProcessorStyles/BaseGraphView";
        
        public event Action< BaseGraph >	graphLoaded;
        public event Action< BaseGraph >	graphUnloaded;
        
        protected virtual void OnEnable()
        {
            InitializeRootView();
            
            graphLoaded = baseGraph => { baseGraph?.OnGraphEnable(); }; 
            graphUnloaded = baseGraph => { baseGraph?.OnGraphDisable(); };
        }

        protected void OnDisable()
        {
            if (graph != null && graphView != null)
            { 
                graphUnloaded?.Invoke(this.graph);  
            }
        }

        public void InitializeGraph(BaseGraph _graph)
        {
            if (null != graph && graph != _graph)
            {
                GraphCreateAndSaveHelper.SaveGraphToDisk(_graph);
                graphUnloaded?.Invoke(graph);
            }
            
            graphLoaded?.Invoke(graph);

            graph = _graph;
            
            if (graphView != null)
            {
                rootView.Remove(graphView);
            }

            InitializeWindow(graph);

            rootView.Add(graphView);
            
            graphView = rootView.Children().FirstOrDefault(e => e is BaseGraphView) as BaseGraphView;
            
            if (graphView == null) 
                return;
            
            graphView.Initialize(graph);
        }
        
        void InitializeRootView()
        {
            rootView = base.rootVisualElement;

            rootView.name = "graphRootView";

            rootView.styleSheets.Add(Resources.Load<StyleSheet>(graphWindowStyle));
        }
        
        protected abstract void	InitializeWindow(BaseGraph graph);
    }

}

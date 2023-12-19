using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseGraphWindow : EditorWindow
{
    
    protected VisualElement		rootView;
    
    readonly string				graphWindowStyle = "GraphProcessorStyles/BaseGraphView";
    
    public void InitializeGraph(BaseGraph graph)
    {
        //Initialize will provide the BaseGraphView
        InitializeWindow(graph);
    }
    
    protected abstract void	InitializeWindow(BaseGraph graph);
    
    void InitializeRootView()
    {
        rootView = base.rootVisualElement;

        rootView.name = "graphRootView";

        var styleSheet = Resources.Load<StyleSheet>(graphWindowStyle);
        
        rootView.styleSheets.Add(styleSheet);
    }

    protected virtual void OnEnable()
    {
        InitializeRootView();
    }

}

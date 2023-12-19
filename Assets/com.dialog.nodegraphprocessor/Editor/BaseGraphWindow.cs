using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class BaseGraphWindow : EditorWindow
{
    public void InitializeGraph(BaseGraph graph)
    {
        //Initialize will provide the BaseGraphView
        InitializeWindow(graph);
    }
    
    protected abstract void	InitializeWindow(BaseGraph graph);
}

using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

public class AllGraphWindow : BaseGraphWindow
{
    protected BaseGraphView		graphView;
    
    protected override void InitializeWindow(BaseGraph graph)
    {
        titleContent = new GUIContent("All Graph");

        if (graphView == null)
        {
            graphView = new AllGraphView(this);
            //toolbarView = new CustomToolbarView(graphView);
            //graphView.Add(toolbarView);
        }

        //rootView.Add(graphView);
    }
}

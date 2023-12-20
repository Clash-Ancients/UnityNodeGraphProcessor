using UnityEngine;
using UnityEngine.UIElements;

public class AllGraphWindow : BaseGraphWindow
{

    protected override void InitializeWindow(BaseGraph graph)
    {
        titleContent = new GUIContent("All Graph");

        if (graphView == null)
        {
            graphView = new AllGraphView(this);
            //toolbarView = new CustomToolbarView(graphView);
            //graphView.Add(toolbarView);
        }

        rootView.Add(graphView);
    }
}

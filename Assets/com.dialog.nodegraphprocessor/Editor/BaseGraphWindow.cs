using GraphProcessor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseGraphWindow : EditorWindow
{
    
    protected VisualElement		rootView;
    protected BaseGraphView		graphView;

    [SerializeField] 
    protected BaseGraph graph;
    
    readonly string				graphWindowStyle = "GraphProcessorStyles/BaseGraphView";
    
    #region open asset - load graph
    protected virtual void OnEnable()
    {
        InitializeRootView();
       
    }
    void InitializeRootView()
    {
        rootView = base.rootVisualElement;

        rootView.name = "graphRootView";

        var styleSheet = Resources.Load<StyleSheet>(graphWindowStyle);
        
        rootView.styleSheets.Add(styleSheet);
    }
    #endregion
    
    #region open asset - 初始化
    public void InitializeGraph(BaseGraph graph)
    {
        
        if (this.graph != null && graph != this.graph)
        {
            // Save the graph to the disk
            EditorUtility.SetDirty(this.graph);
            AssetDatabase.SaveAssets();
           
        }
        
        this.graph = graph;
			
        if (graphView != null)
            rootView.Remove(graphView);
        
        //Initialize will provide the BaseGraphView
        InitializeWindow(graph);
        
        if (graphView == null)
        {
            Debug.LogError("GraphView has not been added to the BaseGraph root view !");
            return ;
        }
			
        graphView.Initialize(graph);
			
        //InitializeGraphView(graphView);
        
    }
    
    protected abstract void	InitializeWindow(BaseGraph graph);
    #endregion
   

}

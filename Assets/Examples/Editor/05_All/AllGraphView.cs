using GraphProcessor;
using UnityEditor;
using UnityEngine.UIElements;

public class AllGraphView : BaseGraphView
{
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        base.BuildContextualMenu(evt);
    }
    
    public AllGraphView(EditorWindow window) : base(window) {}
}

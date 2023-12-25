using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

namespace Plugins.NodeEditor
{
    
    public class UniversalGraphWindow : BaseGraphWindow
    {
        protected override void InitializeWindow(BaseGraph graph)
        {
            graphView = new BaseGraphView(this);
        }
    }

}



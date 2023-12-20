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
        /// <summary>
        /// Graph that owns of the node
        /// </summary>
        BaseGraph							graph;
        
        #region 构造 
        protected BaseGraphView(EditorWindow window)
        {
            RegisterCallback< MouseDownEvent >(MouseDownCallback);

            
            
            InitializeManipulators();
            
            SetupZoom(0.05f, 2f);
            
            this.StretchToParentSize();
        }
    
        protected virtual void InitializeManipulators()
        {
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }
        
        public void Dispose()
        {
       
        }
        #endregion

        #region 初始化
        public void Initialize(BaseGraph _graph)
        {
            if (graph != null)
            {
                // SaveGraphToDisk();
                // // Close pinned windows from old graph:
                // ClearGraphElements();
                // NodeProvider.UnloadGraph(graph);
            }

            graph = _graph;
            
            //InitializeGraphView();
        }
        #endregion
        
        #region 事件
        void MouseDownCallback(MouseDownEvent e)
        {
            // When left clicking on the graph (not a node or something else)
            if (e.button == 0)
            {
                // Close all settings windows:
                //nodeViews.ForEach(v => v.CloseSettings());
            }

            // if (DoesSelectionContainsInspectorNodes())
            //     UpdateNodeInspectorSelection();
        }
        
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            evt.menu.AppendAction("Test", TestBuildContextMenu);
        }

        void TestBuildContextMenu(DropdownMenuAction obj)
        {
            Debug.Log("right mouse click");
        }
        #endregion
    }

}

using System;
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
        
        public List< BaseNodeView >					nodeViews = new List< BaseNodeView >();
        
        public Dictionary< BaseNode, BaseNodeView >	nodeViewsPerNode = new Dictionary< BaseNode, BaseNodeView >();
        
        public BaseEdgeConnectorListener			connectorListener;
        
        #region 構造 / 析構
        public BaseGraphView(EditorWindow window)
        {
            
            graphViewChanged = GraphViewChangedCallback;
            
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            SetupZoom(0.05f, 2f);
            
            this.StretchToParentSize();
            
            createNodeMenu = ScriptableObject.CreateInstance< CreateNodeMenuWindow >();
            createNodeMenu.Initialize(this, window);
        }
        
        public void Dispose()
        {
            
        }
        #endregion
        
        #region 初始化
        public void Initialize(BaseGraph _graph)
        {
            graph = _graph;
            
            connectorListener = CreateEdgeConnectorListener();
            
            InitializeGraphView();
            InitializeNodeViews();
        }

        protected virtual BaseEdgeConnectorListener CreateEdgeConnectorListener()
            => new BaseEdgeConnectorListener(this);
        
        private void InitializeGraphView()
        {
            
            viewTransform.position = graph.position;
            viewTransform.scale = graph.scale;
            nodeCreationRequest = (c) => SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), createNodeMenu);
        }
        
        void InitializeNodeViews()
        {
            graph.nodes.RemoveAll(n => n == null);

            foreach (var node in graph.nodes)
            {
                var v = AddNodeView(node);
            }
        }
        #endregion
        
        //过滤basenode menu
        public virtual IEnumerable<(string path, Type type)> FilterCreateNodeMenuEntries()
        {
            // By default we don't filter anything
            foreach (var nodeMenuItem in NodeProvider.Inst.GetNodeMenuEntries(graph))
                yield return nodeMenuItem;

            // TODO: add exposed properties to this list
        }
        
        //保存到本地
        public void SaveGraphToDisk()
        {
            if (graph == null)
                return ;

            GraphCreateAndSaveHelper.SaveGraphToDisk(graph);
        }
        
        //删除节点
        private GraphViewChange GraphViewChangedCallback(GraphViewChange changes)
        {
            if (null != changes.elementsToRemove)
            {
                changes.elementsToRemove.Sort((e1, e2) => {
                    int GetPriority(GraphElement e)
                    {
                        if (e is BaseNodeView)
                            return 0;
                        else
                            return 1;
                    }
                    return GetPriority(e1).CompareTo(GetPriority(e2));
                });
                
                //Handle ourselves the edge and node remove
                changes.elementsToRemove.RemoveAll(e => {

                    switch (e)
                    {
                        case BaseNodeView nodeView:
                            graph.RemoveNode(nodeView.nodeTarget);
                            RemoveElement(nodeView);
                            return true;
                    }

                    return false;
                });
                
            }
            
            return changes;
        }
        
        //添加节点
        public BaseNodeView AddNode(BaseNode node)
        {
            // This will initialize the node using the graph instance
            graph.AddNode(node);
            
            var view = AddNodeView(node);

            // Call create after the node have been initialized
            // ExceptionToLog.Call(() => view.OnCreated());
            //
            // UpdateComputeOrder();

            return view;
        }

        private BaseNodeView AddNodeView(BaseNode node)
        {
            var viewType = NodeProvider.Inst.GetNodeViewTypeFromType(node.GetType());

            if (viewType == null)
                viewType = typeof(BaseNodeView);

            var baseNodeView = Activator.CreateInstance(viewType) as BaseNodeView;
            
            baseNodeView?.Initialize(this, node);
            
            AddElement(baseNodeView);

            nodeViews.Add(baseNodeView);
            nodeViewsPerNode[node] = baseNodeView;

            return baseNodeView;
        }

        public EdgeView CreateEdgeView()
        {
            return new EdgeView();
        }
        
    }
}


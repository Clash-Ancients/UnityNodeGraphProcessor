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
        
        public BaseGraph							graph;
        CreateNodeMenuWindow						createNodeMenu;
        public SerializedObject		serializedGraph { get; private set; }
        
        public List< BaseNodeView >					nodeViews = new List< BaseNodeView >();
        
        public Dictionary< BaseNode, BaseNodeView >	nodeViewsPerNode = new Dictionary< BaseNode, BaseNodeView >();
        
        public void Dispose()
        {
            
        }

        public BaseGraphView(EditorWindow window)
        {
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            SetupZoom(0.05f, 2f);
            
            this.StretchToParentSize();
            
            createNodeMenu = ScriptableObject.CreateInstance< CreateNodeMenuWindow >();
            createNodeMenu.Initialize(this, window);
        }

        public void Initialize(BaseGraph _graph)
        {
            graph = _graph;
            InitializeGraphView();
        }

        private void InitializeGraphView()
        {
            graph.onGraphChanges += GraphChangesCallback;
            viewTransform.position = graph.position;
            viewTransform.scale = graph.scale;
            nodeCreationRequest = (c) => SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), createNodeMenu);
        }
        
        public virtual IEnumerable<(string path, Type type)> FilterCreateNodeMenuEntries()
        {
            // By default we don't filter anything
            foreach (var nodeMenuItem in NodeProvider.Inst.GetNodeMenuEntries(graph))
                yield return nodeMenuItem;

            // TODO: add exposed properties to this list
        }
        
        public void SaveGraphToDisk()
        {
            if (graph == null)
                return ;

            GraphCreateAndSaveHelper.SaveGraphToDisk(graph);
        }

        void GraphChangesCallback(GraphChanges changes)
        {
            
        }
        
        public BaseNodeView AddNode(BaseNode node)
        {
            // This will initialize the node using the graph instance
            graph.AddNode(node);

            UpdateSerializedProperties();

            var view = AddNodeView(node);

            // Call create after the node have been initialized
            // ExceptionToLog.Call(() => view.OnCreated());
            //
            // UpdateComputeOrder();

            return view;
        }
        
        public BaseNodeView AddNodeView(BaseNode node)
        {
            var viewType = NodeProvider.Inst.GetNodeViewTypeFromType(node.GetType());

            if (viewType == null)
                viewType = typeof(BaseNodeView);

            var baseNodeView = Activator.CreateInstance(viewType) as BaseNodeView;
            baseNodeView.Initialize(this, node);
            AddElement(baseNodeView);

            nodeViews.Add(baseNodeView);
            nodeViewsPerNode[node] = baseNodeView;

            return baseNodeView;
        }

        void UpdateSerializedProperties()
        {
            serializedGraph = new SerializedObject(graph);
        }
        
    }
}


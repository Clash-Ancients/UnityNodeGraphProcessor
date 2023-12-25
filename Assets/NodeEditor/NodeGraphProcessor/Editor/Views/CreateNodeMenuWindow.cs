using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphProcessor
{
    
    public class CreateNodeMenuWindow : ScriptableObject, ISearchWindowProvider
    {
        private BaseGraphView graphView;
        
        Texture2D       icon;
        
        EditorWindow    window;
        
        public void Initialize(BaseGraphView _graphView, EditorWindow _window)
        {
            graphView = _graphView;

            window = _window;
            
            if (icon == null)
                icon = new Texture2D(1, 1);
            icon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            icon.Apply();
        }
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
            };
            
            var nodeEntries = graphView.FilterCreateNodeMenuEntries().OrderBy(k => k.path);
            var titlePaths = new HashSet< string >();
            
            foreach (var nodeMenuItem in nodeEntries)
            {
                var nodePath = nodeMenuItem.path;
                var nodeName = nodePath;
                var level    = 0;
                var parts    = nodePath.Split('/');

                if(parts.Length > 1)
                {
                    level++;
                    nodeName = parts[parts.Length - 1];
                    var fullTitleAsPath = "";
                    
                    for(var i = 0; i < parts.Length - 1; i++)
                    {
                        var title = parts[i];
                        fullTitleAsPath += title;
                        level = i + 1;
                        
                        // Add section title if the node is in subcategory
                        if (!titlePaths.Contains(fullTitleAsPath))
                        {
                            tree.Add(new SearchTreeGroupEntry(new GUIContent(title)){
                                level = level
                            });
                            titlePaths.Add(fullTitleAsPath);
                        }
                    }
                }
                
                tree.Add(new SearchTreeEntry(new GUIContent(nodeName, icon))
                {
                    level    = level + 1,
                    userData = nodeMenuItem.type
                });
            }
            
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            // window to graph position
            var windowRoot = window.rootVisualElement;
            var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent, context.screenMousePosition - window.position.position);
            var graphMousePosition = graphView.contentViewContainer.WorldToLocal(windowMousePosition);

            var nodeType = searchTreeEntry.userData is Type ? (Type)searchTreeEntry.userData : ((NodeProvider.PortDescription)searchTreeEntry.userData).nodeType;
            
            //graphView.RegisterCompleteObjectUndo("Added " + nodeType);
            var view = graphView.AddNode(BaseNode.CreateFromType(nodeType, graphMousePosition));

            return true;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Plugins.NodeEditor
{
    
    public class GraphAssetCallbacks : MonoBehaviour
    {
        [MenuItem("Assets/Create/GraphProcessor_Skill", false, 10)]
        public static void CreateGraphPorcessor_Skill()
        {
            var graph = ScriptableObject.CreateInstance<SkillGraph>();
            ProjectWindowUtil.CreateAsset(graph, "SkillGraph.asset");
        }
        
        [OnOpenAsset(0)]
        public static bool OnBaseGraphOpened(int instanceID, int line)
        {
            var baseGraph = EditorUtility.InstanceIDToObject(instanceID) as BaseGraph;
            return InitializeGraph(baseGraph);
        }

        public static bool InitializeGraph(BaseGraph baseGraph)
        {
            if (baseGraph == null) return false;

            switch (baseGraph)
            {
                case SkillGraph skillGraph:
                    NodeGraphWindowHelper.GetAndShowNodeGraphWindow<SkillGraphWindow>(skillGraph)
                        .InitializeGraph(skillGraph);
                    break;
               
            }

            return true;
        }
        
    }

}

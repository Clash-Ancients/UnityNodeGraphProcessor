using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    }

}

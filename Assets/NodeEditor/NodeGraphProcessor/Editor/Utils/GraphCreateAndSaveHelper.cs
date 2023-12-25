using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GraphProcessor
{
    
    public  static class GraphCreateAndSaveHelper
    {
        public static void SaveGraphToDisk(BaseGraph baseGraphToSave)
        {
            EditorUtility.SetDirty(baseGraphToSave);
            AssetDatabase.SaveAssets();
        }
    }
}

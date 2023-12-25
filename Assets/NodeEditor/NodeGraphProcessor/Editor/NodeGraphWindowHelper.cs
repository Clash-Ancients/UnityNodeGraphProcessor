
using UnityEditor;

namespace GraphProcessor
{
    
    public static class NodeGraphWindowHelper
    {
        private static T GetAndShowNodeGraphWindow<T>(string path) where T : BaseGraphWindow
        {
            
            T resultWindow = EditorWindow.CreateWindow<T>(typeof(T));
            
            return resultWindow;
        }

        public static T GetAndShowNodeGraphWindow<T>(BaseGraph owner) where T : BaseGraphWindow
        {
            return GetAndShowNodeGraphWindow<T>(AssetDatabase.GetAssetPath(owner));
        }
    }

}

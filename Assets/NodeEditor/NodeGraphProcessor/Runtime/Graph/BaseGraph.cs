using Sirenix.OdinInspector;
using UnityEngine;

namespace GraphProcessor
{
    
    public class BaseGraph : SerializedScriptableObject
    {
        [HideInInspector]
        public Vector3					position = Vector3.zero;
        [HideInInspector]
        public Vector3					scale = Vector3.one;
        public void OnGraphDisable()
        {
            
        }

        public void OnGraphEnable()
        {
            
        }
    }

}

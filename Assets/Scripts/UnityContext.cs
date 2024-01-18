using UnityEngine;
using System.Collections.Generic;

namespace NPBehave
{
    public class UnityContext : MonoBehaviour
    {
        static UnityContext instance = null;

        static UnityContext GetInstance()
        {
            if (instance == null)
            {
                GameObject gameObject = new GameObject();
                gameObject.name = "~Context";
                instance = (UnityContext)gameObject.AddComponent(typeof(UnityContext));
                gameObject.isStatic = true;
#if !UNITY_EDITOR
            gameObject.hideFlags = HideFlags.HideAndDontSave;
#endif
            }
            return instance;
        }

        public static Clock GetClock()
        {
            return GetInstance().clock;
        }

        public static Blackboard GetSharedBlackboard(string key)
        {
            UnityContext context = GetInstance();
            if (!context.blackboards.ContainsKey(key))
            {
                context.blackboards.Add(key, new Blackboard(context.clock));
            }
            return context.blackboards[key];
        }

        Dictionary<string, Blackboard> blackboards = new Dictionary<string, Blackboard>();

        Clock clock = new Clock();

        void Update()
        {
            clock.Update(Time.deltaTime);
        }
    }
}
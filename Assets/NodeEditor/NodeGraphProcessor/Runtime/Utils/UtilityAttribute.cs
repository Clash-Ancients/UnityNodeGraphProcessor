using System;
using System.Collections.Generic;
using System.Reflection;


namespace GraphProcessor
{
    
    public class UtilityAttribute
    {
        private static readonly Dictionary<Type, Attribute[]> TypeAttributes = new Dictionary<Type, Attribute[]>();
        
        public static bool TryGetTypeAttribute<TAttributeType>(Type type, out TAttributeType attribute)
            where TAttributeType : Attribute
        {
            if (TryGetTypeAttributes(type, out Attribute[] attributes))
            {
                foreach (var tempAttribute in attributes)
                {
                    attribute = tempAttribute as TAttributeType;
                    if (attribute != null)
                        return true;
                }
            }

            attribute = null;
            return false;
        }
        
        public static bool TryGetTypeAttributes(Type type, out Attribute[] attributes)
        {
            if (TypeAttributes.TryGetValue(type, out attributes))
                return attributes == null || attributes.Length > 0;

            attributes = type.GetCustomAttributes() as Attribute[];
            TypeAttributes[type] = attributes;
            return attributes == null || attributes.Length > 0;
        }
    }
}


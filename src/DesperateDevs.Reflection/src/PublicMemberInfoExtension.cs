using System;
using System.Collections.Generic;
using System.Reflection;

namespace DesperateDevs.Reflection
{
    public static class PublicMemberInfoExtension
    {
        public static List<PublicMemberInfo> GetPublicMemberInfos(this Type type)
        {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;

            var fieldInfos = type.GetFields(bindingFlags);
            var propertyInfos = type.GetProperties(bindingFlags);
            var memberInfos = new List<PublicMemberInfo>(
                fieldInfos.Length + propertyInfos.Length
            );

            foreach (var t in fieldInfos)
                memberInfos.Add(new PublicMemberInfo(t));

            foreach (var propertyInfo in propertyInfos)
            {
                if (propertyInfo.CanRead && propertyInfo.CanWrite && propertyInfo.GetIndexParameters().Length == 0)
                    memberInfos.Add(new PublicMemberInfo(propertyInfo));
            }

            return memberInfos;
        }

        public static object PublicMemberClone(this object obj)
        {
            var clone = Activator.CreateInstance(obj.GetType());
            CopyPublicMemberValues(obj, clone);
            return clone;
        }

        public static T PublicMemberClone<T>(this object obj) where T : new()
        {
            var clone = new T();
            CopyPublicMemberValues(obj, clone);
            return clone;
        }

        public static void CopyPublicMemberValues(this object source, object target)
        {
            var memberInfos = source.GetType().GetPublicMemberInfos();
            foreach (var info in memberInfos)
                info.SetValue(target, info.GetValue(source));
        }
    }
}

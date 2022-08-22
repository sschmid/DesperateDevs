using System;
using System.Linq;
using System.Reflection;

namespace DesperateDevs.Reflection
{
    public static class PublicMemberInfoExtension
    {
        public static PublicMemberInfo[] GetPublicMemberInfos(this Type type)
        {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;

            var fieldInfos = type.GetFields(bindingFlags)
                .Select(info => new PublicMemberInfo(info));

            var propertyInfos = type.GetProperties(bindingFlags)
                .Where(info => info.CanRead && info.CanWrite && info.GetIndexParameters().Length == 0)
                .Select(info => new PublicMemberInfo(info));

            return fieldInfos.Concat(propertyInfos).ToArray();
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
            foreach (var info in source.GetType().GetPublicMemberInfos())
                info.SetValue(target, info.GetValue(source));
        }
    }
}

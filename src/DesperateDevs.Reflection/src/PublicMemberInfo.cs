using System;
using System.Reflection;

namespace DesperateDevs.Reflection
{
    public class PublicMemberInfo
    {
        public readonly Type Type;
        public readonly string Name;
        public readonly AttributeInfo[] Attributes;

        readonly FieldInfo _fieldInfo;
        readonly PropertyInfo _propertyInfo;

        public PublicMemberInfo(FieldInfo info)
        {
            _fieldInfo = info;
            Type = _fieldInfo.FieldType;
            Name = _fieldInfo.Name;
            Attributes = GetAttributes(_fieldInfo.GetCustomAttributes(false));
        }

        public PublicMemberInfo(PropertyInfo info)
        {
            _propertyInfo = info;
            Type = _propertyInfo.PropertyType;
            Name = _propertyInfo.Name;
            Attributes = GetAttributes(_propertyInfo.GetCustomAttributes(false));
        }

        public PublicMemberInfo(Type type, string name, AttributeInfo[] attributes = null)
        {
            Type = type;
            Name = name;
            Attributes = attributes;
        }

        public object GetValue(object obj) => _fieldInfo != null
            ? _fieldInfo.GetValue(obj)
            : _propertyInfo.GetValue(obj, null);

        public void SetValue(object obj, object value)
        {
            if (_fieldInfo != null)
                _fieldInfo.SetValue(obj, value);
            else
                _propertyInfo.SetValue(obj, value, null);
        }

        static AttributeInfo[] GetAttributes(object[] attributes)
        {
            var infos = new AttributeInfo[attributes.Length];
            for (var i = 0; i < attributes.Length; i++)
            {
                var attr = attributes[i];
                infos[i] = new AttributeInfo(attr, attr.GetType().GetPublicMemberInfos());
            }

            return infos;
        }
    }
}

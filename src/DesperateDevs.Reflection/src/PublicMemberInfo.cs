using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DesperateDevs.Reflection
{
    public class PublicMemberInfo
    {
        public readonly Type Type;
        public readonly string Name;
        public readonly IEnumerable<AttributeInfo> Attributes;

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

        public object GetValue(object obj) => _fieldInfo != null
            ? _fieldInfo.GetValue(obj)
            : _propertyInfo.GetValue(obj, null);

        public void SetValue(object obj, object value)
        {
            if (_fieldInfo != null)
                _fieldInfo.SetValue(obj, value);
            else
                _propertyInfo.SetValue(obj, value);
        }

        static IEnumerable<AttributeInfo> GetAttributes(IEnumerable<object> attributes) =>
            attributes.Select(attr => new AttributeInfo(attr, attr.GetType().GetPublicMemberInfos()));
    }
}

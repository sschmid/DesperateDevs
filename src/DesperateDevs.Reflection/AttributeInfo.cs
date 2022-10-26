using System.Collections.Generic;

namespace DesperateDevs.Reflection
{
    public class AttributeInfo
    {
        public readonly object Attribute;
        public readonly IEnumerable<PublicMemberInfo> MemberInfos;

        public AttributeInfo(object attribute, IEnumerable<PublicMemberInfo> memberInfos)
        {
            Attribute = attribute;
            MemberInfos = memberInfos;
        }
    }
}

using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.Reflection.Tests
{
    public class PublicMemberInfoTests
    {
        [Fact]
        public void CreatesEmptyInfoWhenClassHasNoFieldsOrProperties()
        {
            var infos = typeof(EmptyTestClass).GetPublicMemberInfos();
            infos.Should().BeEmpty();
        }

        [Fact]
        public void CreatesMemberInfosForPublicFields()
        {
            var infos = typeof(TestClassWithFields).GetPublicMemberInfos();
            infos.Should().HaveCount(1);
            var mi = infos[0];
            mi.Type.Should().Be(typeof(string));
            mi.Name.Should().Be("PublicField");
            mi.Attributes.Should().HaveCount(1);
            var attribute = mi.Attributes.First();
            attribute.MemberInfos.Should().HaveCount(1);
            var attrValue = attribute.MemberInfos.First().GetValue(attribute.Attribute);
            attrValue.Should().Be("MyField");
        }

        [Fact]
        public void CreatesMemberInfosForPublicProperties()
        {
            var infos = typeof(TestClassWithProperties).GetPublicMemberInfos();
            infos.Should().HaveCount(1);
            var mi = infos[0];
            mi.Type.Should().Be(typeof(string));
            mi.Name.Should().Be("PublicProperty");
            mi.Attributes.Should().HaveCount(1);
            var attribute = mi.Attributes.First();
            attribute.MemberInfos.Should().HaveCount(1);
            var attrValue = attribute.MemberInfos.First().GetValue(attribute.Attribute);
            attrValue.Should().Be("MyProperty");
        }

        [Fact]
        public void CreatesMemberInfosForFieldsAndProperties()
        {
            var infos = typeof(TestClassWithFieldsAndProperties).GetPublicMemberInfos();
            infos.Should().HaveCount(2);

            infos[0].Type.Should().Be(typeof(string));
            infos[0].Name.Should().Be("PublicField");

            infos[1].Type.Should().Be(typeof(string));
            infos[1].Name.Should().Be("PublicProperty");
        }

        [Fact]
        public void GetsValuesForFieldsAndProperties()
        {
            var obj = new TestClassWithFieldsAndProperties
            {
                PublicField = "publicFieldValue",
                PublicProperty = "publicPropertyValue"
            };

            var infos = obj.GetType().GetPublicMemberInfos();
            infos[0].GetValue(obj).Should().Be("publicFieldValue");
            infos[1].GetValue(obj).Should().Be("publicPropertyValue");
        }

        [Fact]
        public void SetsValuesForFieldsAndProperties()
        {
            var obj = new TestClassWithFieldsAndProperties();
            var infos = obj.GetType().GetPublicMemberInfos();
            infos[0].SetValue(obj, "publicFieldValue");
            infos[1].SetValue(obj, "publicPropertyValue");
            obj.PublicField.Should().Be("publicFieldValue");
            obj.PublicProperty.Should().Be("publicPropertyValue");
        }

        [Fact]
        public void ClonesObjectAndSetsPublicMembers()
        {
            var obj = new TestClassWithFieldsAndProperties
            {
                PublicField = "field",
                PublicProperty = "property"
            };

            var clone = (TestClassWithFieldsAndProperties)obj.PublicMemberClone();
            clone.Should().NotBeSameAs(obj);
            clone.PublicField.Should().Be(obj.PublicField);
            clone.PublicProperty.Should().Be(obj.PublicProperty);
        }

        [Fact]
        public void ClonesGenericObjectAndSetsPublicMembers()
        {
            var obj = new TestClassWithFieldsAndProperties
            {
                PublicField = "field",
                PublicProperty = "property"
            };

            var clone = obj.PublicMemberClone<TestClassWithFieldsAndProperties>();
            clone.Should().NotBeSameAs(obj);
            clone.PublicField.Should().Be(obj.PublicField);
            clone.PublicProperty.Should().Be(obj.PublicProperty);
        }

        [Fact]
        public void CopiesPublicMembersToOtherObject()
        {
            var obj = new TestClassWithFieldsAndProperties
            {
                PublicField = "field",
                PublicProperty = "property"
            };
            var newObj = new TestClassWithFieldsAndProperties();
            obj.CopyPublicMemberValues(newObj);
            newObj.PublicField.Should().Be(obj.PublicField);
            newObj.PublicProperty.Should().Be(obj.PublicProperty);
        }
    }

    public class EmptyTestClass { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class TestMemberAttribute : Attribute
    {
        public readonly string Value;
        public TestMemberAttribute(string value) => Value = value;
    }

    public class TestClassWithFields
    {
        [TestMember("MyField")]
        public string PublicField;

        // Should be ignored
#pragma warning disable
        public static bool PublicStaticField;
        bool _privateField;
        static bool _privateStaticField;
    }

    public class TestClassWithProperties
    {
        [TestMember("MyProperty")]
        public string PublicProperty { get; set; }

        // Should be ignored
#pragma warning disable
        public static bool PublicStaticProperty { get; set; }
        bool PrivateProperty { get; set; }
        static bool PrivateStaticProperty { get; set; }
        public string PublicPropertyGet => null;

        public string PublicPropertySet
        {
            set { }
        }
    }

    public class TestClassWithFieldsAndProperties
    {
        public string PublicField;
        public string PublicProperty { get; set; }
    }
}

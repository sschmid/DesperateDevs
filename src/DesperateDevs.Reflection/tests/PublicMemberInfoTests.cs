using System;
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
            infos.Count.Should().Be(1);
            var mi = infos[0];
            mi.Type.Should().Be(typeof(string));
            mi.Name.Should().Be("PublicField");
            mi.Attributes.Length.Should().Be(1);
            mi.Attributes[0].memberInfos.Count.Should().Be(1);
            var attrValue = mi.Attributes[0].memberInfos[0].GetValue(mi.Attributes[0].attribute);
            attrValue.Should().Be("MyField");
        }

        [Fact]
        public void CreatesMemberInfosForPublicProperties()
        {
            var infos = typeof(TestClassWithProperties).GetPublicMemberInfos();
            infos.Count.Should().Be(1);
            var mi = infos[0];
            mi.Type.Should().Be(typeof(string));
            mi.Name.Should().Be("PublicProperty");
            mi.Attributes.Length.Should().Be(1);
            mi.Attributes[0].memberInfos.Count.Should().Be(1);
            var attrValue = mi.Attributes[0].memberInfos[0].GetValue(mi.Attributes[0].attribute);
            attrValue.Should().Be("MyProperty");
        }

        [Fact]
        public void CreatesMemberInfosForFieldsAndProperties()
        {
            var infos = typeof(TestClassWithFieldsAndProperties).GetPublicMemberInfos();
            infos.Count.Should().Be(2);
            var mi1 = infos[0];
            var mi2 = infos[1];

            mi1.Type.Should().Be(typeof(string));
            mi1.Name.Should().Be("PublicField");

            mi2.Type.Should().Be(typeof(string));
            mi2.Name.Should().Be("PublicProperty");
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
            var mi1 = infos[0];
            var mi2 = infos[1];

            mi1.GetValue(obj).Should().Be("publicFieldValue");
            mi2.GetValue(obj).Should().Be("publicPropertyValue");
        }

        [Fact]
        public void SetsValuesForFieldsAndProperties()
        {
            var obj = new TestClassWithFieldsAndProperties();

            var infos = obj.GetType().GetPublicMemberInfos();
            var mi1 = infos[0];
            var mi2 = infos[1];

            mi1.SetValue(obj, "publicFieldValue");
            mi2.SetValue(obj, "publicPropertyValue");

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

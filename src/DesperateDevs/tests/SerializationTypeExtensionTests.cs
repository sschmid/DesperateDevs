using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.Tests
{
    public class SerializationTypeExtensionTests
    {
        [Theory]
        // built-in types, https://msdn.microsoft.com/en-us/library/ya5y69ds.aspx
        [InlineData(typeof(bool), "bool")]
        [InlineData(typeof(byte), "byte")]
        [InlineData(typeof(sbyte), "sbyte")]
        [InlineData(typeof(char), "char")]
        [InlineData(typeof(decimal), "decimal")]
        [InlineData(typeof(double), "double")]
        [InlineData(typeof(float), "float")]
        [InlineData(typeof(int), "int")]
        [InlineData(typeof(uint), "uint")]
        [InlineData(typeof(long), "long")]
        [InlineData(typeof(ulong), "ulong")]
        [InlineData(typeof(object), "object")]
        [InlineData(typeof(short), "short")]
        [InlineData(typeof(ushort), "ushort")]
        [InlineData(typeof(string), "string")]
        [InlineData(typeof(void), "void")]
        // arrays
        [InlineData(typeof(int[]), "int[]")]
        [InlineData(typeof(int[,]), "int[,]")]
        [InlineData(typeof(int[,,]), "int[,,]")]
        [InlineData(typeof(int[][]), "int[][]")]
        // generics
        [InlineData(typeof(List<int>), "System.Collections.Generic.List<int>")]
        [InlineData(typeof(HashSet<TestClass>), "System.Collections.Generic.HashSet<DesperateDevs.Tests.TestClass>")]
        [InlineData(typeof(Dictionary<string, TestClass>), "System.Collections.Generic.Dictionary<string, DesperateDevs.Tests.TestClass>")]
        // enum
        [InlineData(typeof(TestEnum), "DesperateDevs.Tests.TestEnum")]
        [InlineData(typeof(TestNestedEnumClass.NestedEnum), "DesperateDevs.Tests.TestNestedEnumClass.NestedEnum")]
        // custom types
        [InlineData(typeof(TestClass), "DesperateDevs.Tests.TestClass")]
        [InlineData(typeof(TestNestedClass.TestInnerClass), "DesperateDevs.Tests.TestNestedClass.TestInnerClass")]
        // mixed
        [InlineData(typeof(List<int>[,]), "System.Collections.Generic.List<int>[,]")]
        [InlineData(typeof(Dictionary<List<TestNestedEnumClass.NestedEnum>[,], TestClass>[]), "System.Collections.Generic.Dictionary<System.Collections.Generic.List<DesperateDevs.Tests.TestNestedEnumClass.NestedEnum>[,], DesperateDevs.Tests.TestClass>[]")]
        public void ToCompilableString(Type type, string typeName)
        {
            type.ToCompilableString().Should().Be(typeName);
        }

        [Theory]
        // built-in types
        [InlineData("bool", typeof(bool))]
        [InlineData("byte", typeof(byte))]
        [InlineData("sbyte", typeof(sbyte))]
        [InlineData("char", typeof(char))]
        [InlineData("decimal", typeof(decimal))]
        [InlineData("double", typeof(double))]
        [InlineData("float", typeof(float))]
        [InlineData("int", typeof(int))]
        [InlineData("uint", typeof(uint))]
        [InlineData("long", typeof(long))]
        [InlineData("ulong", typeof(ulong))]
        [InlineData("object", typeof(object))]
        [InlineData("short", typeof(short))]
        [InlineData("ushort", typeof(ushort))]
        [InlineData("string", typeof(string))]
        [InlineData("void", typeof(void))]
        // arrays
        [InlineData("int[]", typeof(int[]))]
        [InlineData("int[,]", typeof(int[,]))]
        [InlineData("int[,,]", typeof(int[,,]))]
        [InlineData("int[][]", typeof(int[][]))]
        // generics
        [InlineData("System.Collections.Generic.List<int>", typeof(List<int>))]
        // [InlineData("System.Collections.Generic.HashSet<DesperateDevs.Tests.TestClass>", typeof(HashSet<TestClass>))]
        // [InlineData("System.Collections.Generic.Dictionary<string, DesperateDevs.Tests.TestClass>", typeof(Dictionary<string, TestClass>))]
        // enum
        [InlineData("DesperateDevs.Tests.TestEnum", typeof(TestEnum))]
        [InlineData("DesperateDevs.Tests.TestNestedEnumClass+NestedEnum", typeof(TestNestedEnumClass.NestedEnum))]
        // custom types
        [InlineData("DesperateDevs.Tests.TestClass", typeof(TestClass))]
        [InlineData("DesperateDevs.Tests.TestNestedClass+TestInnerClass", typeof(TestNestedClass.TestInnerClass))]
        // mixed
        [InlineData("System.Collections.Generic.List<int>[,]", typeof(List<int>[,]))]
        // [InlineData("System.Collections.Generic.Dictionary<System.Collections.Generic.List<DesperateDevs.Tests.TestNestedEnumClass+NestedEnum>[,], DesperateDevs.Tests.TestClass>[]", typeof(Dictionary<List<TestNestedEnumClass.NestedEnum>[,], TestClass>[]))]
//
        public void ToType(string typeName, Type type)
        {
            typeName.ToType().Should().Be(type);
        }

        [Fact]
        public void ReturnsShortTypeNameForShortTypeName()
        {
            "MyClass".ShortTypeName().Should().Be("MyClass");
        }

        [Fact]
        public void ReturnsShortTypeNameForFullTypeName()
        {
            "Namespace.Module.MyClass".ShortTypeName().Should().Be("MyClass");
        }

        [Fact]
        public void ReturnsTypeNameForShortTypeName()
        {
            "MyClass".RemoveDots().Should().Be("MyClass");
        }

        [Fact]
        public void ReturnsTypeNameWithoutDotsForFullTypeName()
        {
            "Namespace.Module.MyClass".RemoveDots().Should().Be("NamespaceModuleMyClass");
        }
    }

    public class TestClass { }

    public class TestNestedClass
    {
        public class TestInnerClass { }
    }

    public enum TestEnum { }

    public class TestNestedEnumClass
    {
        public enum NestedEnum { }
    }
}

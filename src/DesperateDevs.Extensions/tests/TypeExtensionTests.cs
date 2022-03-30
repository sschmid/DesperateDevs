using System;
using System.Collections.Generic;
using DesperateDevs.Extensions.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.Extensions.Tests
{
    public class TypeExtensionTests
    {
        [Fact]
        public void ReturnsFalseIfTypeDoesNotOmplementInterface()
        {
            typeof(object).ImplementsInterface<ITestInterface>().Should().BeFalse();
        }

        [Fact]
        public void ReturnsFalseIfTypeIsSame()
        {
            typeof(ITestInterface).ImplementsInterface<ITestInterface>().Should().BeFalse();
        }

        [Fact]
        public void ReturnFalseIfTypeIsInterface()
        {
            typeof(ITestSubInterface).ImplementsInterface<ITestInterface>().Should().BeFalse();
        }

        [Fact]
        public void ReturnsTrueIfTypeImplementsInterface()
        {
            typeof(TestInterfaceClass).ImplementsInterface<ITestInterface>().Should().BeTrue();
        }

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
        [InlineData(typeof(HashSet<TestClass>), "System.Collections.Generic.HashSet<DesperateDevs.Extensions.Tests.Fixtures.TestClass>")]
        [InlineData(typeof(Dictionary<string, TestClass>), "System.Collections.Generic.Dictionary<string, DesperateDevs.Extensions.Tests.Fixtures.TestClass>")]
        // enum
        [InlineData(typeof(TestEnum), "DesperateDevs.Extensions.Tests.Fixtures.TestEnum")]
        [InlineData(typeof(TestNestedEnumClass.NestedEnum), "DesperateDevs.Extensions.Tests.Fixtures.TestNestedEnumClass.NestedEnum")]
        // custom types
        [InlineData(typeof(TestClass), "DesperateDevs.Extensions.Tests.Fixtures.TestClass")]
        [InlineData(typeof(TestNestedClass.TestInnerClass), "DesperateDevs.Extensions.Tests.Fixtures.TestNestedClass.TestInnerClass")]
        // mixed
        [InlineData(typeof(List<int>[,]), "System.Collections.Generic.List<int>[,]")]
        [InlineData(typeof(Dictionary<List<TestNestedEnumClass.NestedEnum>[,], TestClass>[]), "System.Collections.Generic.Dictionary<System.Collections.Generic.List<DesperateDevs.Extensions.Tests.Fixtures.TestNestedEnumClass.NestedEnum>[,], DesperateDevs.Extensions.Tests.Fixtures.TestClass>[]")]
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
        // [InlineData("System.Collections.Generic.HashSet<DesperateDevs.Extensions.Tests.Fixtures.TestClass>", typeof(HashSet<TestClass>))]
        // [InlineData("System.Collections.Generic.Dictionary<string, DesperateDevs.Extensions.Tests.Fixtures.TestClass>", typeof(Dictionary<string, TestClass>))]
        // enum
        [InlineData("DesperateDevs.Extensions.Tests.Fixtures.TestEnum", typeof(TestEnum))]
        [InlineData("DesperateDevs.Extensions.Tests.Fixtures.TestNestedEnumClass+NestedEnum", typeof(TestNestedEnumClass.NestedEnum))]
        // custom types
        [InlineData("DesperateDevs.Extensions.Tests.Fixtures.TestClass", typeof(TestClass))]
        [InlineData("DesperateDevs.Extensions.Tests.Fixtures.TestNestedClass+TestInnerClass", typeof(TestNestedClass.TestInnerClass))]
        // mixed
        [InlineData("System.Collections.Generic.List<int>[,]", typeof(List<int>[,]))]
        // [InlineData("System.Collections.Generic.Dictionary<System.Collections.Generic.List<DesperateDevs.Extensions.Tests.Fixtures.TestNestedEnumClass+NestedEnum>[,], DesperateDevs.Extensions.Tests.Fixtures.TestClass>[]", typeof(Dictionary<List<TestNestedEnumClass.NestedEnum>[,], TestClass>[]))]
//
        public void ToType(string typeName, Type type)
        {
            typeName.ToType().Should().Be(type);
        }

        [Fact]
        public void ToTypeReturnsNullWhenTypeNotFound()
        {
            "unknown".ToType().Should().BeNull();
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
}

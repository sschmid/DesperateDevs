using System;
using System.Collections.Generic;
using DesperateDevs.Utils;
using NSpec;
using Tests.Fixtures;

class describe_SerializationTypeExtension : nspec {

    static string toCompilable<T>() {
        return typeof(T).ToCompilableString();
    }

    static Type toType(string typeString) {
        return typeString.ToType();
    }

    void when_generating() {

        context["when generating compilable string from type"] = () => {

            context["built-in types"] = () => {
                // https://msdn.microsoft.com/en-us/library/ya5y69ds.aspx
                it["generates bool"] = () => toCompilable<bool>().should_be("bool");
                it["generates byte"] = () => toCompilable<byte>().should_be("byte");
                it["generates sbyte"] = () => toCompilable<sbyte>().should_be("sbyte");
                it["generates char"] = () => toCompilable<char>().should_be("char");
                it["generates decimal"] = () => toCompilable<decimal>().should_be("decimal");
                it["generates double"] = () => toCompilable<double>().should_be("double");
                it["generates float"] = () => toCompilable<float>().should_be("float");
                it["generates int"] = () => toCompilable<int>().should_be("int");
                it["generates uint"] = () => toCompilable<uint>().should_be("uint");
                it["generates long"] = () => toCompilable<long>().should_be("long");
                it["generates ulong"] = () => toCompilable<ulong>().should_be("ulong");
                it["generates object"] = () => toCompilable<object>().should_be("object");
                it["generates short"] = () => toCompilable<short>().should_be("short");
                it["generates ushort"] = () => toCompilable<ushort>().should_be("ushort");
                it["generates string"] = () => toCompilable<string>().should_be("string");
                it["generates void"] = () => typeof(void).ToCompilableString().should_be("void");
            };

            context["custom types"] = () => {
                it["generates type string with namespace"] = () => toCompilable<TestNamespaceClass>().should_be("Tests.Fixtures.TestNamespaceClass");
            };

            context["array"] = () => {
                it["generates array rank 1"] = () => toCompilable<int[]>().should_be("int[]");
                it["generates array rank 2"] = () => toCompilable<int[,]>().should_be("int[,]");
                it["generates array rank 3"] = () => toCompilable<int[,,]>().should_be("int[,,]");
                it["generates array of arrays"] = () => toCompilable<int[][]>().should_be("int[][]");
            };

            context["generics"] = () => {
                it["generates List<T>"] = () => toCompilable<List<int>>().should_be("System.Collections.Generic.List<int>");
                it["generates HashSet<T>"] = () => toCompilable<HashSet<TestNamespaceClass>>().should_be("System.Collections.Generic.HashSet<Tests.Fixtures.TestNamespaceClass>");
                it["generates Dictionary<T1, T2>"] = () => toCompilable<Dictionary<string, TestNamespaceClass>>().should_be("System.Collections.Generic.Dictionary<string, Tests.Fixtures.TestNamespaceClass>");
            };

            context["enum"] = () => {
                it["generates enum"] = () => toCompilable<TestEnum>().should_be("TestEnum");
                it["generates nested enum"] = () => toCompilable<TestNestedEnumClass.NestedEnum>().should_be("TestNestedEnumClass.NestedEnum");
            };

            context["nested"] = () => {
                it["generates nested class"] = () => toCompilable<TestNestedClass.TestInnerClass>().should_be("TestNestedClass.TestInnerClass");
            };

            context["mixed"] = () => {
                it["generates List<T>[,]"] = () => toCompilable<List<int>[,]>().should_be("System.Collections.Generic.List<int>[,]");
                it["generates Dictionary<List<T>[,], T2>[]"] = () => toCompilable<Dictionary<List<TestNestedEnumClass.NestedEnum>[,], TestNamespaceClass>[]>().should_be("System.Collections.Generic.Dictionary<System.Collections.Generic.List<TestNestedEnumClass.NestedEnum>[,], Tests.Fixtures.TestNamespaceClass>[]");
            };
        };

        context["when finding type from string"] = () => {

            context["built-in types"] = () => {
                it["finds bool"] = () => toType("bool").should_be(typeof(bool));
                it["finds byte"] = () => toType("byte").should_be(typeof(byte));
                it["finds sbyte"] = () => toType("sbyte").should_be(typeof(sbyte));
                it["finds char"] = () => toType("char").should_be(typeof(char));
                it["finds decimal"] = () => toType("decimal").should_be(typeof(decimal));
                it["finds double"] = () => toType("double").should_be(typeof(double));
                it["finds float"] = () => toType("float").should_be(typeof(float));
                it["finds int"] = () => toType("int").should_be(typeof(int));
                it["finds uint"] = () => toType("uint").should_be(typeof(uint));
                it["finds long"] = () => toType("long").should_be(typeof(long));
                it["finds ulong"] = () => toType("ulong").should_be(typeof(ulong));
                it["finds object"] = () => toType("object").should_be(typeof(object));
                it["finds short"] = () => toType("short").should_be(typeof(short));
                it["finds ushort"] = () => toType("ushort").should_be(typeof(ushort));
                it["finds string"] = () => toType("string").should_be(typeof(string));
                it["finds void"] = () => toType("void").should_be(typeof(void));
            };

            context["custom types"] = () => {
                it["finds type"] = () => toType("Tests.Fixtures.TestNamespaceClass").should_be(typeof(TestNamespaceClass));
            };

            context["array"] = () => {
                it["finds array rank 1"] = () => toType("int[]").should_be(typeof(int[]));
                it["finds array rank 2"] = () => toType("int[,]").should_be(typeof(int[,]));
                it["finds array rank 3"] = () => toType("int[,,]").should_be(typeof(int[,,]));
                it["finds array of arrays"] = () => toType("int[][]").should_be(typeof(int[][]));
            };

            context["generics"] = () => {
                it["finds List<T>"] = () => toType("System.Collections.Generic.List<int>").should_be(typeof(List<int>));
                xit["finds HashSet<T>"] = () => toType("System.Collections.Generic.HashSet<Tests.Fixtures.TestNamespaceClass>").should_be(typeof(HashSet<TestNamespaceClass>));
                xit["finds Dictionary<T1, T2>"] = () => toType("System.Collections.Generic.Dictionary<string, Tests.Fixtures.TestNamespaceClass>").should_be(typeof(Dictionary<string, TestNamespaceClass>));
            };

            context["enum"] = () => {
                it["generates enum"] = () => toType("TestEnum").should_be(typeof(TestEnum));
                it["generates nested enum"] = () => toType("TestNestedEnumClass+NestedEnum").should_be(typeof(TestNestedEnumClass.NestedEnum));
            };

            context["nested"] = () => {
                it["generates nested class"] = () => toType("TestNestedClass+TestInnerClass").should_be(typeof(TestNestedClass.TestInnerClass));
            };

            context["mixed"] = () => {
                it["generates List<T>[,]"] = () => toType("System.Collections.Generic.List<int>[,]").should_be(typeof(List<int>[,]));
                xit["generates Dictionary<List<T>[,], T2>[]"] = () => toType("System.Collections.Generic.Dictionary<System.Collections.Generic.List<TestNestedEnumClass+NestedEnum>[,], Tests.Fixtures.TestNamespaceClass>[]").should_be(typeof(Dictionary<List<TestNestedEnumClass.NestedEnum>[,], TestNamespaceClass>[]));
            };
        };

        context["short type name"] = () => {

            it["returns short type name for short type name"] = () => {
                "MyClass".ShortTypeName().should_be("MyClass");
            };

            it["returns short type name for full type name"] = () => {
                "Namespace.Module.MyClass".ShortTypeName().should_be("MyClass");
            };
        };

        context["removing dots"] = () => {

            it["returns type name for short type name"] = () => {
                "MyClass".RemoveDots().should_be("MyClass");
            };

            it["returns type name without dots for full type name"] = () => {
                "Namespace.Module.MyClass".RemoveDots().should_be("NamespaceModuleMyClass");
            };
        };
    }
}

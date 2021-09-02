using System;
using System.Collections.Generic;
using DesperateDevs.Utils;
using NSpec;
using Shouldly;
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
                it["generates bool"] = () => toCompilable<bool>().ShouldBe("bool");
                it["generates byte"] = () => toCompilable<byte>().ShouldBe("byte");
                it["generates sbyte"] = () => toCompilable<sbyte>().ShouldBe("sbyte");
                it["generates char"] = () => toCompilable<char>().ShouldBe("char");
                it["generates decimal"] = () => toCompilable<decimal>().ShouldBe("decimal");
                it["generates double"] = () => toCompilable<double>().ShouldBe("double");
                it["generates float"] = () => toCompilable<float>().ShouldBe("float");
                it["generates int"] = () => toCompilable<int>().ShouldBe("int");
                it["generates uint"] = () => toCompilable<uint>().ShouldBe("uint");
                it["generates long"] = () => toCompilable<long>().ShouldBe("long");
                it["generates ulong"] = () => toCompilable<ulong>().ShouldBe("ulong");
                it["generates object"] = () => toCompilable<object>().ShouldBe("object");
                it["generates short"] = () => toCompilable<short>().ShouldBe("short");
                it["generates ushort"] = () => toCompilable<ushort>().ShouldBe("ushort");
                it["generates string"] = () => toCompilable<string>().ShouldBe("string");
                it["generates void"] = () => typeof(void).ToCompilableString().ShouldBe("void");
            };

            context["custom types"] = () => {
                it["generates type string with namespace"] = () => toCompilable<TestNamespaceClass>().ShouldBe("Tests.Fixtures.TestNamespaceClass");
            };

            context["array"] = () => {
                it["generates array rank 1"] = () => toCompilable<int[]>().ShouldBe("int[]");
                it["generates array rank 2"] = () => toCompilable<int[,]>().ShouldBe("int[,]");
                it["generates array rank 3"] = () => toCompilable<int[,,]>().ShouldBe("int[,,]");
                it["generates array of arrays"] = () => toCompilable<int[][]>().ShouldBe("int[][]");
            };

            context["generics"] = () => {
                it["generates List<T>"] = () => toCompilable<List<int>>().ShouldBe("System.Collections.Generic.List<int>");
                it["generates HashSet<T>"] = () => toCompilable<HashSet<TestNamespaceClass>>().ShouldBe("System.Collections.Generic.HashSet<Tests.Fixtures.TestNamespaceClass>");
                it["generates Dictionary<T1, T2>"] = () => toCompilable<Dictionary<string, TestNamespaceClass>>().ShouldBe("System.Collections.Generic.Dictionary<string, Tests.Fixtures.TestNamespaceClass>");
            };

            context["enum"] = () => {
                it["generates enum"] = () => toCompilable<TestEnum>().ShouldBe("TestEnum");
                it["generates nested enum"] = () => toCompilable<TestNestedEnumClass.NestedEnum>().ShouldBe("TestNestedEnumClass.NestedEnum");
            };

            context["nested"] = () => {
                it["generates nested class"] = () => toCompilable<TestNestedClass.TestInnerClass>().ShouldBe("TestNestedClass.TestInnerClass");
            };

            context["mixed"] = () => {
                it["generates List<T>[,]"] = () => toCompilable<List<int>[,]>().ShouldBe("System.Collections.Generic.List<int>[,]");
                it["generates Dictionary<List<T>[,], T2>[]"] = () => toCompilable<Dictionary<List<TestNestedEnumClass.NestedEnum>[,], TestNamespaceClass>[]>().ShouldBe("System.Collections.Generic.Dictionary<System.Collections.Generic.List<TestNestedEnumClass.NestedEnum>[,], Tests.Fixtures.TestNamespaceClass>[]");
            };
        };

        context["when finding type from string"] = () => {

            context["built-in types"] = () => {
                it["finds bool"] = () => toType("bool").ShouldBe(typeof(bool));
                it["finds byte"] = () => toType("byte").ShouldBe(typeof(byte));
                it["finds sbyte"] = () => toType("sbyte").ShouldBe(typeof(sbyte));
                it["finds char"] = () => toType("char").ShouldBe(typeof(char));
                it["finds decimal"] = () => toType("decimal").ShouldBe(typeof(decimal));
                it["finds double"] = () => toType("double").ShouldBe(typeof(double));
                it["finds float"] = () => toType("float").ShouldBe(typeof(float));
                it["finds int"] = () => toType("int").ShouldBe(typeof(int));
                it["finds uint"] = () => toType("uint").ShouldBe(typeof(uint));
                it["finds long"] = () => toType("long").ShouldBe(typeof(long));
                it["finds ulong"] = () => toType("ulong").ShouldBe(typeof(ulong));
                it["finds object"] = () => toType("object").ShouldBe(typeof(object));
                it["finds short"] = () => toType("short").ShouldBe(typeof(short));
                it["finds ushort"] = () => toType("ushort").ShouldBe(typeof(ushort));
                it["finds string"] = () => toType("string").ShouldBe(typeof(string));
                it["finds void"] = () => toType("void").ShouldBe(typeof(void));
            };

            context["custom types"] = () => {
                it["finds type"] = () => toType("Tests.Fixtures.TestNamespaceClass").ShouldBe(typeof(TestNamespaceClass));
            };

            context["array"] = () => {
                it["finds array rank 1"] = () => toType("int[]").ShouldBe(typeof(int[]));
                it["finds array rank 2"] = () => toType("int[,]").ShouldBe(typeof(int[,]));
                it["finds array rank 3"] = () => toType("int[,,]").ShouldBe(typeof(int[,,]));
                it["finds array of arrays"] = () => toType("int[][]").ShouldBe(typeof(int[][]));
            };

            context["generics"] = () => {
                it["finds List<T>"] = () => toType("System.Collections.Generic.List<int>").ShouldBe(typeof(List<int>));
                xit["finds HashSet<T>"] = () => toType("System.Collections.Generic.HashSet<Tests.Fixtures.TestNamespaceClass>").ShouldBe(typeof(HashSet<TestNamespaceClass>));
                xit["finds Dictionary<T1, T2>"] = () => toType("System.Collections.Generic.Dictionary<string, Tests.Fixtures.TestNamespaceClass>").ShouldBe(typeof(Dictionary<string, TestNamespaceClass>));
            };

            context["enum"] = () => {
                it["generates enum"] = () => toType("TestEnum").ShouldBe(typeof(TestEnum));
                it["generates nested enum"] = () => toType("TestNestedEnumClass+NestedEnum").ShouldBe(typeof(TestNestedEnumClass.NestedEnum));
            };

            context["nested"] = () => {
                it["generates nested class"] = () => toType("TestNestedClass+TestInnerClass").ShouldBe(typeof(TestNestedClass.TestInnerClass));
            };

            context["mixed"] = () => {
                it["generates List<T>[,]"] = () => toType("System.Collections.Generic.List<int>[,]").ShouldBe(typeof(List<int>[,]));
                xit["generates Dictionary<List<T>[,], T2>[]"] = () => toType("System.Collections.Generic.Dictionary<System.Collections.Generic.List<TestNestedEnumClass+NestedEnum>[,], Tests.Fixtures.TestNamespaceClass>[]").ShouldBe(typeof(Dictionary<List<TestNestedEnumClass.NestedEnum>[,], TestNamespaceClass>[]));
            };
        };

        context["short type name"] = () => {

            it["returns short type name for short type name"] = () => {
                "MyClass".ShortTypeName().ShouldBe("MyClass");
            };

            it["returns short type name for full type name"] = () => {
                "Namespace.Module.MyClass".ShortTypeName().ShouldBe("MyClass");
            };
        };

        context["removing dots"] = () => {

            it["returns type name for short type name"] = () => {
                "MyClass".RemoveDots().ShouldBe("MyClass");
            };

            it["returns type name without dots for full type name"] = () => {
                "Namespace.Module.MyClass".RemoveDots().ShouldBe("NamespaceModuleMyClass");
            };
        };
    }
}

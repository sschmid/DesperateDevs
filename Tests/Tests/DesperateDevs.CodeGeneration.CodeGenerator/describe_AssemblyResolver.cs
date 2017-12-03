using System;
using DesperateDevs.CodeGeneration.CodeGenerator;
using NSpec;

class describe_AssemblyResolver : nspec {

    void when_resolving() {

        context["when no dependencies"] = () => {

            string dllPath = null;
            Type[] types = null;

            before = () => {
                dllPath = TestExtensions.GetProjectRoot() + "/Tests/TestDependencyBase/bin/Release/TestDependencyBase.dll";
                var resolver = new AssemblyResolver(AppDomain.CurrentDomain, new string[0]);
                resolver.Load(dllPath);
                types = resolver.GetTypes();
            };

            it["loads dll"] = () => {
                types.Length.should_be(1);
                types[0].FullName.should_be("TestDependencyBase.TestDependencyBaseClass");
            };

            it["instantiates type"] = () => {
                Activator.CreateInstance(types[0]);
            };
        };

        context["when dependencies"] = () => {

            string dllPath = null;
            Type[] types = null;

            before = () => {
                dllPath = TestExtensions.GetProjectRoot() + "/Tests/TestDependency/bin/Release/TestDependency.dll";
                var resolver = new AssemblyResolver(AppDomain.CurrentDomain, new string[0]);
                resolver.Load(dllPath);
                types = resolver.GetTypes();
            };

            it["loads dll with all dependencies"] = () => {
                types.Length.should_be(1);
                types[0].FullName.should_be("TestDependency.TestDependencyClass");
            };

            it["instantiates type"] = () => {
                Activator.CreateInstance(types[0]);
            };
        };
    }
}

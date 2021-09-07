using System;
using System.Linq;
using DesperateDevs.Utils;
using NSpec;
using Shouldly;

class describe_AssemblyResolver : nspec {

    void when_resolving() {

        var projectRoot = TestExtensions.GetProjectRoot();

        var basePathTestDependency = projectRoot + "/TestDependency/bin/Release";
        var basePathTestDependencyBase = projectRoot + "/TestDependencyBase/bin/Release";

        AssemblyResolver resolver = null;
        string assemblyName = null;

        context["reflection only"] = () => {

            context["when no dependencies"] = () => {

                it["loads assembly with full path"] = () => {
                    assemblyName = basePathTestDependencyBase + "/TestDependencyBase.dll";
                    resolver = new AssemblyResolver(true);
                    resolver.Load(assemblyName);

                    resolver.GetTypes()
                        .Select(t => t.FullName)
                        .ShouldContain("TestDependencyBase.TestDependencyBaseClass");

                    resolver.assemblies.Length.ShouldBe(1);
                };

                it["loads assembly by file name and basePath"] = () => {
                    assemblyName = "TestDependencyBase.dll";
                    resolver = new AssemblyResolver(true, basePathTestDependencyBase);
                    resolver.Load(assemblyName);

                    resolver.GetTypes()
                        .Select(t => t.FullName)
                        .ShouldContain("TestDependencyBase.TestDependencyBaseClass");

                    resolver.assemblies.Length.ShouldBe(1);
                };

                it["loads assembly by name and basePath"] = () => {
                    assemblyName = "TestDependencyBase";
                    resolver = new AssemblyResolver(true, basePathTestDependencyBase);
                    resolver.Load(assemblyName);

                    resolver.GetTypes()
                        .Select(t => t.FullName)
                        .ShouldContain("TestDependencyBase.TestDependencyBaseClass");

                    resolver.assemblies.Length.ShouldBe(1);
                };

                it["doesn't load type into appDomain"] = () => {
                    assemblyName = "TestDependencyBase";
                    resolver = new AssemblyResolver(true, basePathTestDependencyBase);
                    resolver.Load(assemblyName);

                    var types = resolver.GetTypes();

                    AppDomain.CurrentDomain
                        .GetAllTypes()
                        .Select(t => t.FullName)
                        .ShouldNotContain("TestDependencyBase.TestDependencyBaseClass");
                };

                it["retrieves the assembly from type"] = () => {
                    assemblyName = "TestDependencyBase";
                    resolver = new AssemblyResolver(true, basePathTestDependencyBase);
                    resolver.Load(assemblyName);

                    var types = resolver.GetTypes();
                    types[0].FullName.ShouldBe("TestDependencyBase.TestDependencyBaseClass");
                    types[0].Assembly.ShouldBeSameAs(resolver.assemblies[0]);
                };

                it["doesn't add same assembly twice"] = () => {
                    assemblyName = "TestDependencyBase";
                    resolver = new AssemblyResolver(true, basePathTestDependencyBase);
                    resolver.Load(assemblyName);
                    resolver.Load(assemblyName);

                    resolver.assemblies.Length.ShouldBe(1);
                };
            };

            context["when dependencies"] = () => {

                it["doesn't load any dependencies"] = () => {
                    assemblyName = "TestDependency";
                    resolver = new AssemblyResolver(true, basePathTestDependencyBase, basePathTestDependency);
                    resolver.Load(assemblyName);

                    var typeNames = resolver.GetTypes()
                        .Select(t => t.FullName)
                        .ToArray();

                    typeNames.Length.ShouldBe(1);
                    typeNames.ShouldContain("TestDependency.TestDependencyClass");

                    resolver.assemblies.Length.ShouldBe(1);
                };

                it["can reflect type with missing dependencies"] = () => {
                    assemblyName = "TestDependency";
                    resolver = new AssemblyResolver(true, basePathTestDependencyBase, basePathTestDependency);
                    resolver.Load(assemblyName);

                    var i = resolver.GetTypes()[0].GetInterfaces();
                    i.Length.ShouldBe(0);
                };
            };
        };

        context["when actually loading"] = () => {

            xcontext["when dependencies"] = () => {

                before = () => {
                    assemblyName = "TestDependency";
                    resolver = new AssemblyResolver(false, basePathTestDependencyBase, basePathTestDependency);
                    resolver.Load(assemblyName);
                };

                xit["doesn't load any dependencies"] = () => {
                    var types = resolver.GetTypes();
                    types.Length.ShouldBe(1);
                    types[0].FullName.ShouldBe("TestDependency.TestDependencyClass");
                    resolver.assemblies.Length.ShouldBe(1);
                };

                xit["instantiates type trigger loading dependencies"] = () => {
                    var types = resolver.GetTypes();
                    Activator.CreateInstance(types[0]);

                    var typeNames = resolver.GetTypes()
                        .Select(t => t.FullName)
                        .ToArray();

                    typeNames.Length.ShouldBe(2);
                    typeNames.ShouldContain("TestDependency.TestDependencyClass");
                    typeNames.ShouldContain("TestDependencyBase.TestDependencyBaseClass");

                    resolver.assemblies.Length.ShouldBe(2);
                };
            };

            xcontext["when no dependencies"] = () => {

                before = () => {
                    assemblyName = "TestDependencyBase";
                    resolver = new AssemblyResolver(false, basePathTestDependencyBase, basePathTestDependency);
                    resolver.Load(assemblyName);
                };

                xit["loads dll"] = () => {
                    var types = resolver.GetTypes();
                    types.Length.ShouldBe(1);
                    types[0].FullName.ShouldBe("TestDependencyBase.TestDependencyBaseClass");
                };

                it["instantiates type"] = () => {
                    var types = resolver.GetTypes();
                    Activator.CreateInstance(types[0]);
                    types.Length.ShouldBe(1);
                };
            };
        };
    }
}

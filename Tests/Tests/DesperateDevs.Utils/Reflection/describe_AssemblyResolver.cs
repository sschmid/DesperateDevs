using System;
using System.Linq;
using DesperateDevs.Utils;
using NSpec;

class describe_AssemblyResolver : nspec {

    void when_resolving() {

        var projectRoot = TestExtensions.GetProjectRoot();

        var basePathTestDependency = projectRoot + "/Tests/Fixtures/TestDependency/bin/Release";
        var basePathTestDependencyBase = projectRoot + "/Tests/Fixtures/TestDependencyBase/bin/Release";

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
                        .should_contain("TestDependencyBase.TestDependencyBaseClass");

                    resolver.assemblies.Length.should_be(1);
                };

                it["loads assembly by file name and basePath"] = () => {
                    assemblyName = "TestDependencyBase.dll";
                    resolver = new AssemblyResolver(true, basePathTestDependencyBase);
                    resolver.Load(assemblyName);

                    resolver.GetTypes()
                        .Select(t => t.FullName)
                        .should_contain("TestDependencyBase.TestDependencyBaseClass");

                    resolver.assemblies.Length.should_be(1);
                };

                it["loads assembly by name and basePath"] = () => {
                    assemblyName = "TestDependencyBase";
                    resolver = new AssemblyResolver(true, basePathTestDependencyBase);
                    resolver.Load(assemblyName);

                    resolver.GetTypes()
                        .Select(t => t.FullName)
                        .should_contain("TestDependencyBase.TestDependencyBaseClass");

                    resolver.assemblies.Length.should_be(1);
                };

                it["doesn't load type into appDomain"] = () => {
                    assemblyName = "TestDependencyBase";
                    resolver = new AssemblyResolver(true, basePathTestDependencyBase);
                    resolver.Load(assemblyName);

                    var types = resolver.GetTypes();

                    AppDomain.CurrentDomain
                        .GetAllTypes()
                        .Select(t => t.FullName)
                        .should_not_contain("TestDependencyBase.TestDependencyBaseClass");
                };

                it["retrieves the assembly from type"] = () => {
                    assemblyName = "TestDependencyBase";
                    resolver = new AssemblyResolver(true, basePathTestDependencyBase);
                    resolver.Load(assemblyName);

                    var types = resolver.GetTypes();
                    types[0].FullName.should_be("TestDependencyBase.TestDependencyBaseClass");
                    types[0].Assembly.should_be_same(resolver.assemblies[0]);
                };

                it["doesn't add same assembly twice"] = () => {
                    assemblyName = "TestDependencyBase";
                    resolver = new AssemblyResolver(true, basePathTestDependencyBase);
                    resolver.Load(assemblyName);
                    resolver.Load(assemblyName);

                    resolver.assemblies.Length.should_be(1);
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

                    typeNames.Length.should_be(1);
                    typeNames.should_contain("TestDependency.TestDependencyClass");

                    resolver.assemblies.Length.should_be(1);
                };

                it["can reflect type with missing dependencies"] = () => {
                    assemblyName = "TestDependency";
                    resolver = new AssemblyResolver(true, basePathTestDependencyBase, basePathTestDependency);
                    resolver.Load(assemblyName);

                    var i = resolver.GetTypes()[0].GetInterfaces();
                    i.Length.should_be(0);
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
                    types.Length.should_be(1);
                    types[0].FullName.should_be("TestDependency.TestDependencyClass");
                    resolver.assemblies.Length.should_be(1);
                };

                xit["instantiates type trigger loading dependencies"] = () => {
                    var types = resolver.GetTypes();
                    Activator.CreateInstance(types[0]);

                    var typeNames = resolver.GetTypes()
                        .Select(t => t.FullName)
                        .ToArray();

                    typeNames.Length.should_be(2);
                    typeNames.should_contain("TestDependency.TestDependencyClass");
                    typeNames.should_contain("TestDependencyBase.TestDependencyBaseClass");

                    resolver.assemblies.Length.should_be(2);
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
                    types.Length.should_be(1);
                    types[0].FullName.should_be("TestDependencyBase.TestDependencyBaseClass");
                };

                it["instantiates type"] = () => {
                    var types = resolver.GetTypes();
                    Activator.CreateInstance(types[0]);
                    types.Length.should_be(1);
                };
            };
        };
    }
}

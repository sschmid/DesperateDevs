using System;
using System.IO;
using System.Linq;
using DesperateDevs.Extensions;
using DesperateDevs.Reflection;
using DesperateDevs.Tests;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.Tests
{
    public class AssemblyResolverTests
    {
        static readonly string ProjectRoot = TestHelper.GetProjectRoot();

        const string Project1AssemblyName = "DesperateDevs.Tests.Project1";
        const string Project2AssemblyName = "DesperateDevs.Tests.Project2";
        const string Project1AssemblyFile = Project1AssemblyName + ".dll";
        const string Project2AssemblyFile = Project2AssemblyName + ".dll";
        const string Project1ClassType = "DesperateDevs.Tests.Project1.TestClass";
        const string Project2ClassType = "DesperateDevs.Tests.Project2.TestClass";
        static readonly string Project1BasePath = Path.Combine(ProjectRoot, "DesperateDevs", "fixtures", "DesperateDevs.Tests.Project1", "bin", "Release");
        static readonly string Project2BasePath = Path.Combine(ProjectRoot, "DesperateDevs", "fixtures", "DesperateDevs.Tests.Project2", "bin", "Release");

        AssemblyResolver Project1ResolverReflection { get; } = new AssemblyResolver(true, Project1BasePath);

        AssemblyResolver Project1Resolver
        {
            get
            {
                if (_resolver == null)
                {
                    _resolver = new AssemblyResolver(false, Project1BasePath);
                    _resolver.Load(Project1AssemblyName);
                }

                return _resolver;
            }
        }

        AssemblyResolver Project12Resolver
        {
            get
            {
                if (_resolver == null)
                {
                    _resolver = new AssemblyResolver(false, Project1BasePath, Project2BasePath);
                    _resolver.Load(Project2AssemblyName);
                }

                return _resolver;
            }
        }

        AssemblyResolver _resolver;

        [Fact]
        public void LoadsAssemblyWithFullPath()
        {
            var resolver = new AssemblyResolver(true);
            resolver.Load(Path.Combine(Project1BasePath, Project1AssemblyFile));

            resolver.GetTypes()
                .Select(t => t.FullName)
                .Should().Contain(Project1ClassType);

            resolver.assemblies.Length.Should().Be(1);
        }

        [Fact]
        public void LoadsAssemblyByFileNameAndBasePath()
        {
            Project1ResolverReflection.Load(Project1AssemblyFile);

            Project1ResolverReflection.GetTypes()
                .Select(t => t.FullName)
                .Should().Contain(Project1ClassType);

            Project1ResolverReflection.assemblies.Length.Should().Be(1);
        }

        [Fact]
        public void LoadsAssemblyByNameAndBasePath()
        {
            Project1ResolverReflection.Load(Project1AssemblyName);

            Project1ResolverReflection.GetTypes()
                .Select(t => t.FullName)
                .Should().Contain(Project1ClassType);

            Project1ResolverReflection.assemblies.Length.Should().Be(1);
        }

        [Fact(Skip = "Must be run separately (uses AppDomain)")]
        public void DoesNotLoadTypeIntoAppDomain()
        {
            Project1ResolverReflection.Load(Project1AssemblyName);
            Project1ResolverReflection.GetTypes();

            AppDomain.CurrentDomain
                .GetAllTypes()
                .Select(t => t.FullName)
                .Should().NotContain(Project1ClassType);
        }

        [Fact]
        public void RetrievesTheAssemblyFromType()
        {
            Project1ResolverReflection.Load(Project1AssemblyName);

            var types = Project1ResolverReflection.GetTypes();
            types[0].FullName.Should().Be(Project1ClassType);
            types[0].Assembly.Should().BeSameAs(Project1ResolverReflection.assemblies[0]);
        }

        [Fact]
        public void DoesNotAddSameAssemblyTwice()
        {
            Project1ResolverReflection.Load(Project1AssemblyName);
            Project1ResolverReflection.Load(Project1AssemblyName);
            Project1ResolverReflection.assemblies.Length.Should().Be(1);
        }

        [Fact]
        public void ReflectionOnlyDoesNotLoadAnyDependencies()
        {
            var resolver = new AssemblyResolver(true, Project1BasePath, Project2BasePath);
            resolver.Load(Project2AssemblyName);

            var typeNames = resolver.GetTypes()
                .Select(t => t.FullName)
                .ToArray();

            typeNames.Length.Should().Be(1);
            typeNames.Should().Contain(Project2ClassType);
            resolver.assemblies.Length.Should().Be(1);
        }

        [Fact]
        public void CanReflectTypeWithMissingDependencies()
        {
            var resolver = new AssemblyResolver(true, Project1BasePath, Project2BasePath);
            resolver.Load(Project2AssemblyName);

            var i = resolver.GetTypes()[0].GetInterfaces();
            i.Length.Should().Be(0);
        }

        [Fact]
        public void DoesNotLoadAnyDependencies()
        {
            var types = Project12Resolver.GetTypes();
            types.Length.Should().Be(1);
            types[0].FullName.Should().Be(Project2ClassType);
            Project12Resolver.assemblies.Length.Should().Be(1);
        }

        [Fact(Skip = "TODO")]
        public void InstantiatingTypeResultsInLoadingDependencies()
        {
            var types = Project12Resolver.GetTypes();
            Activator.CreateInstance(types[0]);

            var typeNames = Project12Resolver.GetTypes()
                .Select(t => t.FullName)
                .ToArray();

            typeNames.Length.Should().Be(2);
            typeNames.Should().Contain(Project1ClassType);
            typeNames.Should().Contain(Project2ClassType);
            Project12Resolver.assemblies.Length.Should().Be(2);
        }

        [Fact]
        public void LoadsDll()
        {
            var types = Project1Resolver.GetTypes();
            types.Length.Should().Be(1);
            types[0].FullName.Should().Be(Project1ClassType);
        }

        [Fact]
        public void InstantiatesType()
        {
            var types = Project1Resolver.GetTypes();
            Activator.CreateInstance(types[0]);
            types.Length.Should().Be(1);
        }
    }
}

﻿using System;
using System.IO;
using System.Linq;
using DesperateDevs.Tests;
using FluentAssertions;
using Sherlog;
using Sherlog.Formatters;
using Xunit;
using Xunit.Abstractions;

namespace DesperateDevs.Reflection.Tests
{
    public class AssemblyResolverTests : IDisposable
    {
        static readonly LogMessageFormatter Formatter = new LogMessageFormatter("[{1}]\t{0}: {2}");

        static readonly string ProjectRoot = TestHelper.GetProjectRoot();

        const string AssemblyName1 = "DesperateDevs.Reflection.Tests.Fixture1";
        const string AssemblyName2 = "DesperateDevs.Reflection.Tests.Fixture2";
        const string AssemblyFile1 = $"{AssemblyName1}.dll";
        const string AssemblyFile2 = $"{AssemblyName2}.dll";
        const string Type1 = $"{AssemblyName1}.TestClass";
        const string Type2 = $"{AssemblyName2}.TestClass";

        static readonly string BasePath1 = Path.Combine(ProjectRoot, "tests", AssemblyName1, "bin", "Release");
        static readonly string BasePath2 = Path.Combine(ProjectRoot, "tests", AssemblyName2, "bin", "Release");

        readonly ITestOutputHelper _output;

        public AssemblyResolverTests(ITestOutputHelper output)
        {
            _output = output;
            Logger.AddAppender((logger, level, message) => _output.WriteLine(Formatter.FormatMessage(logger, level, message)));
        }

        [Fact]
        public void LoadsAssemblyWithFullPath()
        {
            using var resolver = new AssemblyResolver();
            resolver.Load(Path.Combine(BasePath1, AssemblyFile1));

            resolver.Assemblies.Should().HaveCount(1);
            GetTypes(resolver)
                .Select(t => t.FullName)
                .Should().Contain(Type1);
        }

        [Fact]
        public void LoadsAssemblyByFileNameAndBasePath()
        {
            using var resolver = new AssemblyResolver(BasePath1);
            resolver.Load(AssemblyFile1);

            resolver.Assemblies.Should().HaveCount(1);
            GetTypes(resolver)
                .Select(t => t.FullName)
                .Should().Contain(Type1);
        }

        [Fact]
        public void LoadsAssemblyByNameAndBasePath()
        {
            using var resolver = new AssemblyResolver(BasePath1);
            resolver.Load(AssemblyName1);

            resolver.Assemblies.Should().HaveCount(1);
            GetTypes(resolver)
                .Select(t => t.FullName)
                .Should().Contain(Type1);
        }

        [Fact]
        public void RetrievesAssemblyFromType()
        {
            using var resolver = new AssemblyResolver(BasePath1);
            resolver.Load(AssemblyName1);
            var type = GetTypes(resolver).First();
            type.FullName.Should().Be(Type1);
            type.Assembly.Should().BeSameAs(resolver.Assemblies.First());
        }

        [Fact]
        public void DoesNotAddSameAssemblyTwice()
        {
            using var resolver = new AssemblyResolver(BasePath1);
            resolver.Load(AssemblyName1);
            resolver.Load(AssemblyName1);
            resolver.Assemblies.Should().HaveCount(1);
        }

        [Fact]
        public void InstantiatesType()
        {
            using var resolver = new AssemblyResolver(BasePath1);
            resolver.Load(AssemblyName1);
            Activator.CreateInstance(GetTypes(resolver).First());
        }

        [Fact]
        public void CanLoadTypeWithDependencies()
        {
            using var resolver = new AssemblyResolver(BasePath1, BasePath2);
            resolver.Load(AssemblyName2);
            var types = GetTypes(resolver);
            types.Should().HaveCount(1);
            Activator.CreateInstance(types[0]);
        }

        [Fact]
        public void DoesNotLoadAnyDependencies()
        {
            using var resolver = new AssemblyResolver(BasePath1, BasePath2);
            resolver.Load(AssemblyName2);
            var types = GetTypes(resolver);
            types.Should().HaveCount(1);
            types[0].FullName.Should().Be(Type2);
            resolver.Assemblies.Should().HaveCount(1);
        }

        Type[] GetTypes(AssemblyResolver resolver) => resolver.GetTypes()
            .Where(type => !type.FullName.StartsWith("Coverlet"))
            .Where(type => !type.FullName.StartsWith("Microsoft"))
            .Where(type => !type.FullName.StartsWith("System"))
            .ToArray();

        public void Dispose()
        {
            _output.WriteLine("Dispose");
            Logger.GlobalLogLevel = LogLevel.On;
            Logger.ClearAppenders();
            Logger.ClearLoggers();
        }
    }
}

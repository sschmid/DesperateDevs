using System.Collections.Generic;
using System.Linq;
using DesperateDevs.CodeGeneration;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.CodeGeneration.CodeGenerator.Tests
{
    public class CodeGeneratorTests
    {
        [Fact]
        public void ExecutesPreProcessorsDataProvidersGeneratorsAndPostProcessors()
        {
            var preStr = new List<string>();
            var generator = new CodeGenerator(
                new IPreProcessor[] {new Pre1PreProcessor(preStr)},
                new IDataProvider[] {new Data_1_2_Provider()},
                new ICodeGenerator[] {new DataFile1CodeGenerator()},
                new IPostProcessor[] {new Processed1PostProcessor()}
            );

            var files = generator.Generate();

            preStr.Count.Should().Be(1);
            preStr[0].Should().Be("Pre1");

            files.Length.Should().Be(2);

            files[0].fileName.Should().Be("Test1File0-Processed1");
            files[0].fileContent.Should().Be("data1");

            files[1].fileName.Should().Be("Test1File1-Processed1");
            files[1].fileContent.Should().Be("data2");
        }

        [Fact]
        public void UsesReturnedCodeGenFiles()
        {
            var generator = new CodeGenerator(
                new IPreProcessor[] {new Pre1PreProcessor(new List<string>())},
                new IDataProvider[] {new Data_1_2_Provider()},
                new ICodeGenerator[] {new DataFile1CodeGenerator()},
                new IPostProcessor[] {new Processed1PostProcessor(), new NoFilesPostProcessor()}
            );

            var files = generator.Generate();
            files.Length.Should().Be(1);
            files[0].fileName.Should().Be("Test1File0-Processed1");
        }

        [Fact]
        public void SkipsPluginsWhichDoNotRunInDryRun()
        {
            var preStr = new List<string>();
            var generator = new CodeGenerator(
                new IPreProcessor[] {new Pre1PreProcessor(preStr), new DisabledPreProcessor(preStr)},
                new IDataProvider[] {new Data_1_2_Provider(), new DisabledDataProvider()},
                new ICodeGenerator[] {new DataFile1CodeGenerator(), new DisabledCodeGenerator()},
                new IPostProcessor[] {new Processed1PostProcessor(), new DisabledPostProcessor()}
            );

            var files = generator.DryRun();

            preStr.Count.Should().Be(1);
            preStr[0].Should().Be("Pre1");

            files.Length.Should().Be(2);

            files[0].fileName.Should().Be("Test1File0-Processed1");
            files[1].fileName.Should().Be("Test1File1-Processed1");
        }

        [Fact]
        public void RunsPreProcessorsBasedOnPriority()
        {
            var preStr = new List<string>();
            var generator = new CodeGenerator(
                new IPreProcessor[] {new Pre2PreProcessor(preStr), new Pre1PreProcessor(preStr)},
                new IDataProvider[] {new Data_1_2_Provider()},
                new ICodeGenerator[] {new DataFile1CodeGenerator()},
                new IPostProcessor[] {new Processed1PostProcessor()}
            );

            generator.Generate();

            preStr.Count.Should().Be(2);
            preStr[0].Should().Be("Pre1");
            preStr[1].Should().Be("Pre2");
        }

        [Fact]
        public void RunsDataProviderBasedOnPriority()
        {
            var generator = new CodeGenerator(
                new IPreProcessor[] {new Pre1PreProcessor(new List<string>())},
                new IDataProvider[] {new Data_3_4_Provider(), new Data_1_2_Provider()},
                new ICodeGenerator[] {new DataFile1CodeGenerator()},
                new IPostProcessor[] {new Processed1PostProcessor()}
            );

            var files = generator.Generate();

            files.Length.Should().Be(4);

            files[0].fileName.Should().Be("Test1File0-Processed1");
            files[0].fileContent.Should().Be("data1");

            files[1].fileName.Should().Be("Test1File1-Processed1");
            files[1].fileContent.Should().Be("data2");

            files[2].fileName.Should().Be("Test1File2-Processed1");
            files[2].fileContent.Should().Be("data3");

            files[3].fileName.Should().Be("Test1File3-Processed1");
            files[3].fileContent.Should().Be("data4");
        }

        [Fact]
        public void RunsCodeGeneratorsBasedOnPriority()
        {
            var generator = new CodeGenerator(
                new IPreProcessor[] {new Pre1PreProcessor(new List<string>())},
                new IDataProvider[] {new Data_1_2_Provider()},
                new ICodeGenerator[] {new DataFile2CodeGenerator(), new DataFile1CodeGenerator()},
                new IPostProcessor[] {new Processed1PostProcessor()}
            );

            var files = generator.Generate();

            files.Length.Should().Be(4);

            files[0].fileName.Should().Be("Test1File0-Processed1");
            files[1].fileName.Should().Be("Test1File1-Processed1");
            files[2].fileName.Should().Be("Test2File0-Processed1");
            files[3].fileName.Should().Be("Test2File1-Processed1");
        }

        [Fact]
        public void RunsPostProcessorsBasedOnPriority()
        {
            var generator = new CodeGenerator(
                new IPreProcessor[] {new Pre1PreProcessor(new List<string>())},
                new IDataProvider[] {new Data_1_2_Provider()},
                new ICodeGenerator[] {new DataFile1CodeGenerator()},
                new IPostProcessor[] {new Processed2PostProcessor(), new Processed1PostProcessor()}
            );

            var files = generator.Generate();

            files.Length.Should().Be(2);

            files[0].fileName.Should().Be("Test1File0-Processed1-Processed2");
            files[1].fileName.Should().Be("Test1File1-Processed1-Processed2");
        }

        [Fact]
        public void CancelsRun()
        {
            var generator = new CodeGenerator(
                new IPreProcessor[] {new Pre1PreProcessor(new List<string>())},
                new IDataProvider[] {new Data_1_2_Provider()},
                new ICodeGenerator[] {new DataFile1CodeGenerator()},
                new IPostProcessor[] {new Processed1PostProcessor()}
            );

            generator.OnProgress += delegate { generator.Cancel(); };

            var files = generator.Generate();

            files.Length.Should().Be(0);
        }

        [Fact]
        public void CancelsDryRun()
        {
            var generator = new CodeGenerator(
                new IPreProcessor[] {new Pre1PreProcessor(new List<string>())},
                new IDataProvider[] {new Data_1_2_Provider()},
                new ICodeGenerator[] {new DataFile1CodeGenerator()},
                new IPostProcessor[] {new Processed1PostProcessor()}
            );

            generator.OnProgress += delegate { generator.Cancel(); };

            var files = generator.DryRun();

            files.Length.Should().Be(0);
        }

        [Fact]
        public void CanGenerateAgainAfterCancel()
        {
            var generator = new CodeGenerator(
                new IPreProcessor[] {new Pre1PreProcessor(new List<string>())},
                new IDataProvider[] {new Data_1_2_Provider()},
                new ICodeGenerator[] {new DataFile1CodeGenerator()},
                new IPostProcessor[] {new Processed1PostProcessor()}
            );

            void OnProgress(string title, string info, float progress) => generator.Cancel();

            generator.OnProgress += OnProgress;

            generator.Generate();

            generator.OnProgress -= OnProgress;

            var files = generator.Generate();

            files.Length.Should().Be(2);
        }

        [Fact]
        public void CanDoDryRunAgainAfterCancel()
        {
            var generator = new CodeGenerator(
                new IPreProcessor[] {new Pre1PreProcessor(new List<string>())},
                new IDataProvider[] {new Data_1_2_Provider()},
                new ICodeGenerator[] {new DataFile1CodeGenerator()},
                new IPostProcessor[] {new Processed1PostProcessor()}
            );

            void OnProgress(string title, string info, float progress) => generator.Cancel();

            generator.OnProgress += OnProgress;

            generator.Generate();

            generator.OnProgress -= OnProgress;

            var files = generator.DryRun();

            files.Length.Should().Be(2);
        }

        [Fact]
        public void RegistersObjectInSharedCache()
        {
            var generator = new CodeGenerator(
                new IPreProcessor[] {new Pre1PreProcessor(new List<string>())},
                new IDataProvider[] {new CachableProvider(), new CachableProvider()},
                new ICodeGenerator[] {new DataFile1CodeGenerator()},
                new IPostProcessor[] {new Processed1PostProcessor()}
            );

            var files = generator.Generate();
            files.Length.Should().Be(2);
            files[0].fileContent.Should().Be(files[1].fileContent);
        }

        [Fact]
        public void ResetsCacheBeforeEachNewRun()
        {
            var generator = new CodeGenerator(
                new IPreProcessor[] {new Pre1PreProcessor(new List<string>())},
                new IDataProvider[] {new CachableProvider(), new CachableProvider()},
                new ICodeGenerator[] {new DataFile1CodeGenerator()},
                new IPostProcessor[] {new Processed1PostProcessor()}
            );

            var result1 = generator.Generate()[0].fileContent;
            var result2 = generator.Generate()[0].fileContent;
            result1.Should().NotBe(result2);
        }
    }
}

public class Data_1_2_Provider : IDataProvider
{
    public string name => "";
    public int priority => 0;
    public bool runInDryMode => true;

    public CodeGeneratorData[] GetData()
    {
        var data1 = new CodeGeneratorData();
        data1.Add("testKey", "data1");

        var data2 = new CodeGeneratorData();
        data2.Add("testKey", "data2");

        return new[]
        {
            data1,
            data2
        };
    }
}

public class Data_3_4_Provider : IDataProvider
{
    public string name => "";
    public int priority => 5;
    public bool runInDryMode => true;

    public CodeGeneratorData[] GetData()
    {
        var data1 = new CodeGeneratorData();
        data1.Add("testKey", "data3");

        var data2 = new CodeGeneratorData();
        data2.Add("testKey", "data4");

        return new[]
        {
            data1,
            data2
        };
    }
}

public class DisabledDataProvider : IDataProvider
{
    public string name => "";
    public int priority => 5;
    public bool runInDryMode => false;

    public CodeGeneratorData[] GetData()
    {
        var data1 = new CodeGeneratorData();
        data1.Add("testKey", "data5");

        var data2 = new CodeGeneratorData();
        data2.Add("testKey", "data6");

        return new[]
        {
            data1,
            data2
        };
    }
}

public class DataFile1CodeGenerator : ICodeGenerator
{
    public string name => "";
    public int priority => 0;
    public bool runInDryMode => true;

    public CodeGenFile[] Generate(CodeGeneratorData[] data)
    {
        return data
            .Select((d, i) => new CodeGenFile(
                "Test1File" + i,
                d["testKey"].ToString(),
                "Test1CodeGenerator"
            )).ToArray();
    }
}

public class DataFile2CodeGenerator : ICodeGenerator
{
    public string name => "";
    public int priority => 5;
    public bool runInDryMode => true;

    public CodeGenFile[] Generate(CodeGeneratorData[] data)
    {
        return data
            .Select((d, i) => new CodeGenFile(
                "Test2File" + i,
                d["testKey"].ToString(),
                "Test2CodeGenerator"
            )).ToArray();
    }
}

public class DisabledCodeGenerator : ICodeGenerator
{
    public string name => "";
    public int priority => -5;
    public bool runInDryMode => false;

    public CodeGenFile[] Generate(CodeGeneratorData[] data)
    {
        return data
            .Select((d, i) => new CodeGenFile(
                "Test3File" + i,
                d["testKey"].ToString(),
                "DisabledCodeGenerator"
            )).ToArray();
    }
}

public class Processed1PostProcessor : IPostProcessor
{
    public string name => "";
    public int priority => 0;
    public bool runInDryMode => true;

    public CodeGenFile[] PostProcess(CodeGenFile[] files)
    {
        foreach (var file in files)
        {
            file.fileName += "-Processed1";
        }

        return files;
    }
}

public class Processed2PostProcessor : IPostProcessor
{
    public string name => "";
    public int priority => 5;
    public bool runInDryMode => true;

    public CodeGenFile[] PostProcess(CodeGenFile[] files)
    {
        foreach (var file in files)
        {
            file.fileName += "-Processed2";
        }

        return files;
    }
}

public class DisabledPostProcessor : IPostProcessor
{
    public string name => "";
    public int priority => 5;
    public bool runInDryMode => false;

    public CodeGenFile[] PostProcess(CodeGenFile[] files)
    {
        foreach (var file in files)
        {
            file.fileName += "-Disabled";
        }

        return files;
    }
}

public class NoFilesPostProcessor : IPostProcessor
{
    public string name => "";
    public int priority => -5;
    public bool runInDryMode => true;

    public CodeGenFile[] PostProcess(CodeGenFile[] files)
    {
        return new[] {files[0]};
    }
}

public class CachableProvider : IDataProvider, ICachable
{
    public string name => "";
    public int priority => 0;
    public bool runInDryMode => true;

    public Dictionary<string, object> objectCache { get; set; }

    public CodeGeneratorData[] GetData()
    {
        object o;
        if (!objectCache.TryGetValue("myObject", out o))
        {
            o = new object();
            objectCache.Add("myObject", o);
        }

        var data = new CodeGeneratorData();
        data.Add("testKey", o.GetHashCode());
        return new[] {data};
    }
}

public class Pre1PreProcessor : IPreProcessor
{
    public string name => "";
    public int priority => 0;
    public bool runInDryMode => true;

    List<string> _strings;

    public Pre1PreProcessor(List<string> strings)
    {
        _strings = strings;
    }

    public void PreProcess()
    {
        _strings.Add("Pre1");
    }
}

public class Pre2PreProcessor : IPreProcessor
{
    public string name => "";
    public int priority => 5;
    public bool runInDryMode => true;

    List<string> _strings;

    public Pre2PreProcessor(List<string> strings)
    {
        _strings = strings;
    }

    public void PreProcess()
    {
        _strings.Add("Pre2");
    }
}

public class DisabledPreProcessor : IPreProcessor
{
    public string name => "";
    public int priority => 0;
    public bool runInDryMode => false;

    List<string> _strings;

    public DisabledPreProcessor(List<string> strings)
    {
        _strings = strings;
    }

    public void PreProcess()
    {
        _strings.Add("DisabledPre");
    }
}
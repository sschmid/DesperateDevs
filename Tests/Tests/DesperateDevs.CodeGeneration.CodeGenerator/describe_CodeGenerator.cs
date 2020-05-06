using System.Collections.Generic;
using System.Linq;
using DesperateDevs.CodeGeneration;
using DesperateDevs.CodeGeneration.CodeGenerator;
using NSpec;
using Shouldly;

class describe_CodeGenerator : nspec {

    void when_generating() {

        context["generate"] = () => {

            it["executes pre processors, data providers, generators and post processors"] = () => {
                var preStr = new List<string>();
                var generator = new CodeGenerator(
                    new [] { new Pre1PreProcessor(preStr) },
                    new [] { new Data_1_2_Provider() },
                    new [] { new DataFile1CodeGenerator() },
                    new [] { new Processed1PostProcessor() }
                );

                var files = generator.Generate();

                preStr.Count.ShouldBe(1);
                preStr[0].ShouldBe("Pre1");

                files.Length.ShouldBe(2);

                files[0].fileName.ShouldBe("Test1File0-Processed1");
                files[0].fileContent.ShouldBe("data1");

                files[1].fileName.ShouldBe("Test1File1-Processed1");
                files[1].fileContent.ShouldBe("data2");
            };

            it["uses returned CodeGenFiles"] = () => {
                var generator = new CodeGenerator(
                    new [] { new Pre1PreProcessor(new List<string>()) },
                    new [] { new Data_1_2_Provider() },
                    new [] { new DataFile1CodeGenerator() },
                    new IPostProcessor[] { new Processed1PostProcessor(), new NoFilesPostProcessor() }
                );

                var files = generator.Generate();

                files.Length.ShouldBe(1);

                files[0].fileName.ShouldBe("Test1File0-Processed1");
            };
        };

        context["dry run"] = () => {

            it["skips plugins which don't run in dry run"] = () => {
                var preStr = new List<string>();
                var generator = new CodeGenerator(
                    new IPreProcessor[] { new Pre1PreProcessor(preStr), new DisabledPreProcessor(preStr) },
                    new IDataProvider[] { new Data_1_2_Provider(), new DisabledDataProvider() },
                    new ICodeGenerator[] { new DataFile1CodeGenerator(), new DisabledCodeGenerator() },
                    new IPostProcessor[] { new Processed1PostProcessor(), new DisabledPostProcessor() }
                );

                var files = generator.DryRun();

                preStr.Count.ShouldBe(1);
                preStr[0].ShouldBe("Pre1");

                files.Length.ShouldBe(2);

                files[0].fileName.ShouldBe("Test1File0-Processed1");
                files[1].fileName.ShouldBe("Test1File1-Processed1");
            };
        };

        context["priority"] = () => {

            it["runs pre processors based on priority"] = () => {
                var preStr = new List<string>();
                var generator = new CodeGenerator(
                    new IPreProcessor[] { new Pre2PreProcessor(preStr), new Pre1PreProcessor(preStr) },
                    new [] { new Data_1_2_Provider() },
                    new [] { new DataFile1CodeGenerator() },
                    new [] { new Processed1PostProcessor() }
                );

                var files = generator.Generate();

                preStr.Count.ShouldBe(2);
                preStr[0].ShouldBe("Pre1");
                preStr[1].ShouldBe("Pre2");
            };

            it["runs data provider based on priority"] = () => {
                var generator = new CodeGenerator(
                    new [] { new Pre1PreProcessor(new List<string>()) },
                    new IDataProvider[] { new Data_3_4_Provider(), new Data_1_2_Provider() },
                    new [] { new DataFile1CodeGenerator() },
                    new [] { new Processed1PostProcessor() }
                );

                var files = generator.Generate();

                files.Length.ShouldBe(4);

                files[0].fileName.ShouldBe("Test1File0-Processed1");
                files[0].fileContent.ShouldBe("data1");

                files[1].fileName.ShouldBe("Test1File1-Processed1");
                files[1].fileContent.ShouldBe("data2");

                files[2].fileName.ShouldBe("Test1File2-Processed1");
                files[2].fileContent.ShouldBe("data3");

                files[3].fileName.ShouldBe("Test1File3-Processed1");
                files[3].fileContent.ShouldBe("data4");
            };

            it["runs code generators based on priority"] = () => {
                var generator = new CodeGenerator(
                    new [] { new Pre1PreProcessor(new List<string>()) },
                    new [] { new Data_1_2_Provider() },
                    new ICodeGenerator[] { new DataFile2CodeGenerator(), new DataFile1CodeGenerator() },
                    new [] { new Processed1PostProcessor() }
                );

                var files = generator.Generate();

                files.Length.ShouldBe(4);

                files[0].fileName.ShouldBe("Test1File0-Processed1");
                files[1].fileName.ShouldBe("Test1File1-Processed1");
                files[2].fileName.ShouldBe("Test2File0-Processed1");
                files[3].fileName.ShouldBe("Test2File1-Processed1");
            };

            it["runs post processors based on priority"] = () => {
                var generator = new CodeGenerator(
                    new [] { new Pre1PreProcessor(new List<string>()) },
                    new [] { new Data_1_2_Provider() },
                    new [] { new DataFile1CodeGenerator() },
                    new IPostProcessor[] { new Processed2PostProcessor(), new Processed1PostProcessor() }
                );

                var files = generator.Generate();

                files.Length.ShouldBe(2);

                files[0].fileName.ShouldBe("Test1File0-Processed1-Processed2");
                files[1].fileName.ShouldBe("Test1File1-Processed1-Processed2");
            };
        };

        context["cancel"] = () => {

            it["cancels"] = () => {
                var generator = new CodeGenerator(
                    new [] { new Pre1PreProcessor(new List<string>()) },
                    new [] { new Data_1_2_Provider() },
                    new [] { new DataFile1CodeGenerator() },
                    new [] { new Processed1PostProcessor() }
                );

                generator.OnProgress += (title, info, progress) => generator.Cancel();

                var files = generator.Generate();

                files.Length.ShouldBe(0);
            };

            it["cancels dry run"] = () => {
                var generator = new CodeGenerator(
                    new [] { new Pre1PreProcessor(new List<string>()) },
                    new [] { new Data_1_2_Provider() },
                    new [] { new DataFile1CodeGenerator() },
                    new [] { new Processed1PostProcessor() }
                );

                generator.OnProgress += (title, info, progress) => generator.Cancel();

                var files = generator.DryRun();

                files.Length.ShouldBe(0);
            };

            it["can generate again after cancel"] = () => {
                var generator = new CodeGenerator(
                    new [] { new Pre1PreProcessor(new List<string>()) },
                    new [] { new Data_1_2_Provider() },
                    new [] { new DataFile1CodeGenerator() },
                    new [] { new Processed1PostProcessor() }
                );

                GeneratorProgress onProgress = (title, info, progress) => generator.Cancel();
                generator.OnProgress += onProgress;

                generator.Generate();

                generator.OnProgress -= onProgress;

                var files = generator.Generate();

                files.Length.ShouldBe(2);
            };

            it["can do dry run after cancel"] = () => {
                var generator = new CodeGenerator(
                    new [] { new Pre1PreProcessor(new List<string>()) },
                    new [] { new Data_1_2_Provider() },
                    new [] { new DataFile1CodeGenerator() },
                    new [] { new Processed1PostProcessor() }
                );

                GeneratorProgress onProgress = (title, info, progress) => generator.Cancel();
                generator.OnProgress += onProgress;

                generator.Generate();

                generator.OnProgress -= onProgress;

                var files = generator.DryRun();

                files.Length.ShouldBe(2);
            };
        };

        context["caching"] = () => {

            it["registers object to shared cache"] = () => {
                var generator = new CodeGenerator(
                    new [] { new Pre1PreProcessor(new List<string>()) },
                    new [] { new CachableProvider(), new CachableProvider() },
                    new [] { new DataFile1CodeGenerator() },
                    new [] { new Processed1PostProcessor() }
                );

                var files = generator.Generate();
                files.Length.ShouldBe(2);
                files[0].fileContent.ShouldBe(files[1].fileContent);
            };

            it["resets cache before each new run"] = () => {
                var generator = new CodeGenerator(
                    new [] { new Pre1PreProcessor(new List<string>()) },
                    new [] { new CachableProvider(), new CachableProvider() },
                    new [] { new DataFile1CodeGenerator() },
                    new [] { new Processed1PostProcessor() }
                );

                var result1 = generator.Generate()[0].fileContent;
                var result2 = generator.Generate()[0].fileContent;
                result1.ShouldNotBe(result2);
            };
        };
    }
}

public class Data_1_2_Provider : IDataProvider {

    public string name { get { return ""; } }
    public int priority { get { return 0; } }
    public bool runInDryMode { get { return true; } }

    public CodeGeneratorData[] GetData() {
        var data1 = new CodeGeneratorData();
        data1.Add("testKey", "data1");

        var data2 = new CodeGeneratorData();
        data2.Add("testKey", "data2");

        return new [] {
            data1,
            data2
        };
    }
}

public class Data_3_4_Provider : IDataProvider {

    public string name { get { return ""; } }
    public int priority { get { return 5; } }
    public bool runInDryMode { get { return true; } }

    public CodeGeneratorData[] GetData() {
        var data1 = new CodeGeneratorData();
        data1.Add("testKey", "data3");

        var data2 = new CodeGeneratorData();
        data2.Add("testKey", "data4");

        return new [] {
            data1,
            data2
        };
    }
}

public class DisabledDataProvider : IDataProvider {

    public string name { get { return ""; } }
    public int priority { get { return 5; } }
    public bool runInDryMode { get { return false; } }

    public CodeGeneratorData[] GetData() {
        var data1 = new CodeGeneratorData();
        data1.Add("testKey", "data5");

        var data2 = new CodeGeneratorData();
        data2.Add("testKey", "data6");

        return new [] {
            data1,
            data2
        };
    }
}

public class DataFile1CodeGenerator : ICodeGenerator {

    public string name { get { return ""; } }
    public int priority { get { return 0; } }
    public bool runInDryMode { get { return true; } }

    public CodeGenFile[] Generate(CodeGeneratorData[] data) {
        return data
            .Select((d, i) => new CodeGenFile(
                "Test1File" + i,
                d["testKey"].ToString(),
                "Test1CodeGenerator"
            )).ToArray();
    }
}

public class DataFile2CodeGenerator : ICodeGenerator {

    public string name { get { return ""; } }
    public int priority { get { return 5; } }
    public bool runInDryMode { get { return true; } }

    public CodeGenFile[] Generate(CodeGeneratorData[] data) {
        return data
            .Select((d, i) => new CodeGenFile(
                "Test2File" + i,
                d["testKey"].ToString(),
                "Test2CodeGenerator"
            )).ToArray();
    }
}

public class DisabledCodeGenerator : ICodeGenerator {

    public string name { get { return ""; } }
    public int priority { get { return -5; } }
    public bool runInDryMode { get { return false; } }

    public CodeGenFile[] Generate(CodeGeneratorData[] data) {
        return data
            .Select((d, i) => new CodeGenFile(
                "Test3File" + i,
                d["testKey"].ToString(),
                "DisabledCodeGenerator"
            )).ToArray();
    }
}

public class Processed1PostProcessor : IPostProcessor {

    public string name { get { return ""; } }
    public int priority { get { return 0; } }
    public bool runInDryMode { get { return true; } }

    public CodeGenFile[] PostProcess(CodeGenFile[] files) {
        foreach (var file in files) {
            file.fileName += "-Processed1";
        }

        return files;
    }
}

public class Processed2PostProcessor : IPostProcessor {

    public string name { get { return ""; } }
    public int priority { get { return 5; } }
    public bool runInDryMode { get { return true; } }

    public CodeGenFile[] PostProcess(CodeGenFile[] files) {
        foreach (var file in files) {
            file.fileName += "-Processed2";
        }

        return files;
    }
}

public class DisabledPostProcessor : IPostProcessor {

    public string name { get { return ""; } }
    public int priority { get { return 5; } }
    public bool runInDryMode { get { return false; } }

    public CodeGenFile[] PostProcess(CodeGenFile[] files) {
        foreach (var file in files) {
            file.fileName += "-Disabled";
        }

        return files;
    }
}

public class NoFilesPostProcessor : IPostProcessor {

    public string name { get { return ""; } }
    public int priority { get { return -5; } }
    public bool runInDryMode { get { return true; } }

    public CodeGenFile[] PostProcess(CodeGenFile[] files) {
        return new [] { files[0] };
    }
}

public class CachableProvider : IDataProvider, ICachable {

    public string name { get { return ""; } }
    public int priority { get { return 0; } }
    public bool runInDryMode { get { return true; } }

    public Dictionary<string, object> objectCache { get; set; }

    public CodeGeneratorData[] GetData() {
        object o;
        if (!objectCache.TryGetValue("myObject", out o)) {
            o = new object();
            objectCache.Add("myObject", o);
        }

        var data = new CodeGeneratorData();
        data.Add("testKey", o.GetHashCode());
        return new [] { data };
    }
}

public class Pre1PreProcessor : IPreProcessor {

    public string name { get { return ""; } }
    public int priority { get { return 0; } }
    public bool runInDryMode { get { return true; } }

    List<string> _strings;

    public Pre1PreProcessor(List<string> strings) {
        _strings = strings;
    }

    public void PreProcess() {
        _strings.Add("Pre1");
    }
}

public class Pre2PreProcessor : IPreProcessor {

    public string name { get { return ""; } }
    public int priority { get { return 5; } }
    public bool runInDryMode { get { return true; } }

    List<string> _strings;

    public Pre2PreProcessor(List<string> strings) {
        _strings = strings;
    }

    public void PreProcess() {
        _strings.Add("Pre2");
    }
}

public class DisabledPreProcessor : IPreProcessor {

    public string name { get { return ""; } }
    public int priority { get { return 0; } }
    public bool runInDryMode { get { return false; } }

    List<string> _strings;

    public DisabledPreProcessor(List<string> strings) {
        _strings = strings;
    }

    public void PreProcess() {
        _strings.Add("DisabledPre");
    }
}

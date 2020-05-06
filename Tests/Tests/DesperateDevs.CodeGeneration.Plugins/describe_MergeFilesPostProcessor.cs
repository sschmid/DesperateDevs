using DesperateDevs.CodeGeneration;
using DesperateDevs.CodeGeneration.Plugins;
using NSpec;
using Shouldly;

class describe_MergeFilesPostProcessor : nspec {

    void when_post_processing() {

        it["merges files with same filename"] = () => {
            var files = new[] {
                new CodeGenFile("file1", "content1", "gen1"),
                new CodeGenFile("file1", "content2", "gen2"),
                new CodeGenFile("file3", "content3", "gen3")
            };

            var postprocessor = new MergeFilesPostProcessor();
            files =  postprocessor.PostProcess(files);

            files.Length.ShouldBe(2);
            files[0].fileName.ShouldBe("file1");
            files[1].fileName.ShouldBe("file3");

            files[0].fileContent.ShouldBe("content1\ncontent2");
            files[0].generatorName.ShouldBe("gen1, gen2");
        };
    }
}

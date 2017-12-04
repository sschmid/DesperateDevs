using DesperateDevs.CodeGeneration;
using DesperateDevs.CodeGeneration.Plugins;
using NSpec;

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

            files.Length.should_be(2);
            files[0].fileName.should_be("file1");
            files[1].fileName.should_be("file3");

            files[0].fileContent.should_be("content1\ncontent2");
            files[0].generatorName.should_be("gen1, gen2");
        };
    }
}

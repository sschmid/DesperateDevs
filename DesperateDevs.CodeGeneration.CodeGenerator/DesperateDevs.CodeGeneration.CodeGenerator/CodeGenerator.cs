using System.Collections.Generic;
using System.Linq;

namespace DesperateDevs.CodeGeneration.CodeGenerator {

    public delegate void GeneratorProgress(string title, string info, float progress);

    public class CodeGenerator {

        public event GeneratorProgress OnProgress;

        readonly IDataProvider[] _dataProviders;
        readonly ICodeGenerator[] _codeGenerators;
        readonly IPostProcessor[] _postProcessors;

        readonly Dictionary<string, object> _objectCache;

        bool _cancel;

        public CodeGenerator(IDataProvider[] dataProviders, ICodeGenerator[] codeGenerators, IPostProcessor[] postProcessors) {
            _dataProviders = dataProviders.OrderBy(i => i.priority).ToArray();
            _codeGenerators = codeGenerators.OrderBy(i => i.priority).ToArray();
            _postProcessors = postProcessors.OrderBy(i => i.priority).ToArray();
            _objectCache = new Dictionary<string, object>();
        }

        public CodeGenFile[] DryRun() {
            return generate(
                "[Dry Run] ",
                _dataProviders.Where(i => i.runInDryMode).ToArray(),
                _codeGenerators.Where(i => i.runInDryMode).ToArray(),
                _postProcessors.Where(i => i.runInDryMode).ToArray()
            );
        }

        public CodeGenFile[] Generate() {
            return generate(
                string.Empty,
                _dataProviders,
                _codeGenerators,
                _postProcessors
            );
        }

        CodeGenFile[] generate(string messagePrefix,
            IDataProvider[] dataProviders,
            ICodeGenerator[] codeGenerators,
            IPostProcessor[] postProcessors) {
            _cancel = false;

            _objectCache.Clear();

            var cachables = ((ICodeGeneratorBase[])dataProviders)
                .Concat(codeGenerators)
                .Concat(postProcessors)
                .OfType<ICachable>();

            foreach (var cachable in cachables) {
                cachable.objectCache = _objectCache;
            }

            var data = new List<CodeGeneratorData>();

            var total = dataProviders.Length + codeGenerators.Length + postProcessors.Length;
            var progress = 0;

            foreach (var dataProvider in dataProviders) {
                if (_cancel) {
                    return new CodeGenFile[0];
                }

                progress += 1;
                if (OnProgress != null) {
                    OnProgress(messagePrefix + "Creating model", dataProvider.name, (float)progress / total);
                }

                data.AddRange(dataProvider.GetData());
            }

            var files = new List<CodeGenFile>();
            var dataArray = data.ToArray();
            foreach (var generator in codeGenerators) {
                if (_cancel) {
                    return new CodeGenFile[0];
                }

                progress += 1;
                if (OnProgress != null) {
                    OnProgress(messagePrefix + "Creating files", generator.name, (float)progress / total);
                }

                files.AddRange(generator.Generate(dataArray));
            }

            var generatedFiles = files.ToArray();
            foreach (var postProcessor in postProcessors) {
                if (_cancel) {
                    return new CodeGenFile[0];
                }

                progress += 1;
                if (OnProgress != null) {
                    OnProgress(messagePrefix + "Processing files", postProcessor.name, (float)progress / total);
                }

                generatedFiles = postProcessor.PostProcess(generatedFiles);
            }

            return generatedFiles;
        }

        public void Cancel() {
            _cancel = true;
        }
    }
}

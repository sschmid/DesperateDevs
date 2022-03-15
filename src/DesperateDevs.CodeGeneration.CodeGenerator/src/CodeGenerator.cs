using System;
using System.Collections.Generic;
using System.Linq;
using DesperateDevs;

namespace DesperateDevs.CodeGeneration.CodeGenerator {

    public delegate void GeneratorProgress(string title, string info, float progress);

    public class CodeGenerator {

        public static string defaultPropertiesPath { get { return "Jenny.properties"; } }

        public event GeneratorProgress OnProgress;

        readonly IPreProcessor[] _preProcessors;
        readonly IDataProvider[] _dataProviders;
        readonly ICodeGenerator[] _codeGenerators;
        readonly IPostProcessor[] _postProcessors;

        readonly Dictionary<string, object> _objectCache;

        bool _cancel;

        public CodeGenerator(
            IPreProcessor[] preProcessors,
            IDataProvider[] dataProviders,
            ICodeGenerator[] codeGenerators,
            IPostProcessor[] postProcessors) {
            _preProcessors = preProcessors.OrderBy(i => i.priority).ToArray();
            _dataProviders = dataProviders.OrderBy(i => i.priority).ToArray();
            _codeGenerators = codeGenerators.OrderBy(i => i.priority).ToArray();
            _postProcessors = postProcessors.OrderBy(i => i.priority).ToArray();
            _objectCache = new Dictionary<string, object>();
        }

        public CodeGenFile[] DryRun() {
            return generate(
                "[Dry Run] ",
                _preProcessors.Where(i => i.runInDryMode).ToArray(),
                _dataProviders.Where(i => i.runInDryMode).ToArray(),
                _codeGenerators.Where(i => i.runInDryMode).ToArray(),
                _postProcessors.Where(i => i.runInDryMode).ToArray()
            );
        }

        public CodeGenFile[] Generate() {
            var files = generate(
                string.Empty,
                _preProcessors,
                _dataProviders,
                _codeGenerators,
                _postProcessors
            );

            return files;
        }

        CodeGenFile[] generate(string messagePrefix,
            IPreProcessor[] preProcessors,
            IDataProvider[] dataProviders,
            ICodeGenerator[] codeGenerators,
            IPostProcessor[] postProcessors) {
            _cancel = false;

            _objectCache.Clear();

            var cachables = ((ICodeGenerationPlugin[])preProcessors)
                .Concat(dataProviders)
                .Concat(codeGenerators)
                .Concat(postProcessors)
                .OfType<ICachable>();

            foreach (var cachable in cachables) {
                cachable.objectCache = _objectCache;
            }

            var total = preProcessors.Length + dataProviders.Length + codeGenerators.Length + postProcessors.Length;
            var progress = 0;

            foreach (var preProcessor in preProcessors) {
                if (_cancel) {
                    return new CodeGenFile[0];
                }

                progress += 1;
                if (OnProgress != null) {
                    OnProgress(messagePrefix + "Pre Processing", preProcessor.name, (float)progress / total);
                }

                preProcessor.PreProcess();
            }

            var data = new List<CodeGeneratorData>();
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
                    OnProgress(messagePrefix + "Post Processing", postProcessor.name, (float)progress / total);
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

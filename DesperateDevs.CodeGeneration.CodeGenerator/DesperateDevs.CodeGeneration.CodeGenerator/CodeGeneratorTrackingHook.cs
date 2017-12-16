using DesperateDevs.Analytics;

namespace DesperateDevs.CodeGeneration.CodeGenerator {

    public abstract class CodeGeneratorTrackingHook : AbstractTrackingHook {

        protected IPreProcessor[] _preProcessors;
        protected IDataProvider[] _dataProviders;
        protected ICodeGenerator[] _codeGenerators;
        protected IPostProcessor[] _postProcessors;
        protected CodeGenFile[] _files;

        public void Track(IPreProcessor[] preProcessors, IDataProvider[] dataProviders, ICodeGenerator[] codeGenerators, IPostProcessor[] postProcessors, CodeGenFile[] files) {
            _preProcessors = preProcessors;
            _dataProviders = dataProviders;
            _codeGenerators = codeGenerators;
            _postProcessors = postProcessors;
            _files = files;

            Track();
        }
    }
}

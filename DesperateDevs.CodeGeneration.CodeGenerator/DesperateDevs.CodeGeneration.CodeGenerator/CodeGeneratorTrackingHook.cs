using DesperateDevs.Analytics;

namespace DesperateDevs.CodeGeneration.CodeGenerator {

    public abstract class CodeGeneratorTrackingHook : AbstractTrackingHook {

        protected IDataProvider[] _dataProviders;
        protected ICodeGenerator[] _codeGenerators;
        protected IPostProcessor[] _postProcessors;
        protected CodeGenFile[] _files;

        public void Track(IDataProvider[] dataProviders, ICodeGenerator[] codeGenerators, IPostProcessor[] postProcessors, CodeGenFile[] files) {
            _dataProviders = dataProviders;
            _codeGenerators = codeGenerators;
            _postProcessors = postProcessors;
            _files = files;

            Track();
        }
    }
}

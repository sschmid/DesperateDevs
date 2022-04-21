namespace DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor
{
    public static class CodeGeneratorMenuItems
    {
        public const string preferences = "Tools/Jenny/Preferences... #%j";
        public const string generate = "Tools/Jenny/Generate #%g";
        public const string generate_server = "Tools/Jenny/Generate with Server %&g";
    }

    public static class CodeGeneratorMenuItemPriorities
    {
        public const int preferences = 1;
        public const int generate = 2;
        public const int generate_server = 3;
    }
}

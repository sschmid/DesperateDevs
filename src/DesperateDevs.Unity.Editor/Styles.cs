using UnityEngine;

namespace DesperateDevs.Unity.Editor
{
    public static class Styles
    {
        static GUIStyle _sectionHeader;

        public static GUIStyle SectionHeader => _sectionHeader ??= new GUIStyle("OL Title");

        static GUIStyle _sectionContent;

        public static GUIStyle SectionContent =>
            _sectionContent ??= new GUIStyle("OL Box")
            {
                stretchHeight = false
            };

        static GUIStyle _errorLabel;

        public static GUIStyle ErrorLabel =>
            _errorLabel ??= new GUIStyle(GUI.skin.label)
            {
                wordWrap = true,
                normal =
                {
                    textColor = Color.red
                }
            };

        static GUIStyle _toolbarSearchTextField;

        public static GUIStyle ToolbarSearchTextField =>
            _toolbarSearchTextField ??= GUI.skin.FindStyle("ToolbarSeachTextField");

        static GUIStyle _toolbarSearchCancelButtonStyle;

        public static GUIStyle ToolbarSearchCancelButtonStyle =>
            _toolbarSearchCancelButtonStyle ??= GUI.skin.FindStyle("ToolbarSeachCancelButton");
    }
}

using UnityEngine;

namespace DesperateDevs.Unity.Editor
{
    public static class Styles
    {
        static GUIStyle _sectionHeader;

        public static GUIStyle SectionHeader => _sectionHeader ?? (_sectionHeader = new GUIStyle("OL Title"));

        static GUIStyle _sectionContent;

        public static GUIStyle SectionContent
        {
            get
            {
                if (_sectionContent == null)
                {
                    _sectionContent = new GUIStyle("OL Box");
                    _sectionContent.stretchHeight = false;
                }

                return _sectionContent;
            }
        }
    }
}

using DesperateDevs.Serialization;

namespace DesperateDevs.Unity.Editor {

    public abstract class AbstractPreferencesDrawer : IPreferencesDrawer {

        public abstract string title { get; }

        protected bool _drawContent = true;

        public abstract void Initialize(Preferences preferences);
        public abstract void DrawHeader(Preferences preferences);

        public virtual void DrawContent(Preferences preferences) {
            _drawContent = EditorLayout.DrawSectionHeaderToggle(title, _drawContent);
            if (_drawContent) {
                EditorLayout.BeginSectionContent();
                {
                    drawContent(preferences);
                }
                EditorLayout.EndSectionContent();
            }
        }

        protected abstract void drawContent(Preferences preferences);
    }
}

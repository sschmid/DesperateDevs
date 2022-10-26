using DesperateDevs.Serialization;

namespace DesperateDevs.Unity.Editor
{
    public abstract class AbstractPreferencesDrawer : IPreferencesDrawer
    {
        public abstract string Title { get; }

        protected bool _drawContent = true;

        public abstract void Initialize(Preferences preferences);
        public abstract void DrawHeader(Preferences preferences);

        public virtual void DrawContent(Preferences preferences)
        {
            var title = _drawContent ? $"▼ {Title}" : $"▶ {Title}";
            _drawContent = EditorLayout.DrawSectionHeaderToggle(title, _drawContent);
            if (_drawContent)
            {
                EditorLayout.BeginSectionContent();
                {
                    OnDrawContent(preferences);
                }
                EditorLayout.EndSectionContent();
            }
        }

        protected abstract void OnDrawContent(Preferences preferences);
    }
}

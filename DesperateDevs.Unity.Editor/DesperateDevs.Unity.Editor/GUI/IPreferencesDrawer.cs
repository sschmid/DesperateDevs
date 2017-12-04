using DesperateDevs.Serialization;

namespace DesperateDevs.Unity.Editor {

    public interface IPreferencesDrawer {

        int priority { get; }
        string title { get; }

        void Initialize(Preferences preferences);

        void Draw(Preferences preferences);
    }
}

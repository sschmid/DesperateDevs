namespace DesperateDevs.Serialization.Tests.Fixtures
{
    public class TestPreferences : Preferences
    {
        public TestPreferences(string properties, string userProperties = null, bool doubleQuotedValues = false)
            : base(
                new Properties(properties, doubleQuotedValues),
                new Properties(userProperties ?? string.Empty, doubleQuotedValues),
                doubleQuotedValues
            ) { }
    }
}

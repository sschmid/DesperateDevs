using System.Collections.Generic;
using DesperateDevs.Serialization;
using NSpec;
using Shouldly;

class describe_Preferences : nspec {

    void when_creating_preferences() {

        Preferences preferences = null;

        context["when properties"] = () => {

            before = () => {
                preferences = new TestPreferences("key = value");
            };

            it["sets properties"] = () => {
                preferences.properties.count.ShouldBe(1);
                preferences.properties.HasKey("key").ShouldBeTrue();
                preferences.userProperties.count.ShouldBe(0);
            };

            it["gets value for key"] = () => {
                preferences["key"].ShouldBe("value");
            };

            it["throws"] = expect<KeyNotFoundException>(() => {
                var x = preferences["unknown"];
            });

            it["has key"] = () => {
                preferences.HasKey("key").ShouldBeTrue();
            };

            it["sets key"] = () => {
                preferences["key2"] = "value2";
                preferences["key2"].ShouldBe("value2");
                preferences.HasKey("key2").ShouldBeTrue();
            };

            it["can ToString"] = () => {
                preferences.ToString().ShouldBe("key = value\n");
            };

            xit["supports double quote mode"] = () => {
                preferences.doubleQuoteMode = true;
                preferences["key2"] = "value2";
                preferences["key2"].ShouldBe("\"value2\"");
            };
        };

        context["when user properties"] = () => {

            before = () => {
                preferences = new TestPreferences(
                    "key = ${userName}",
                    "userName = Max");
            };

            it["has key"] = () => {
                preferences.HasKey("userName").ShouldBeTrue();
            };

            it["resolves placeholder from user properties"] = () => {
                preferences["key"].ShouldBe("Max");
            };

            it["doesn't overwrite value when not different"] = () => {
                preferences["key"] = "Max";
                preferences.properties.ToString().Contains("Max").ShouldBeFalse();
            };

            it["overwrites value when different"] = () => {
                preferences["key"] = "Jack";
                preferences.properties.ToString().Contains("Jack").ShouldBeTrue();
            };

            it["user properties overwrite default properties"] = () => {
                preferences = new TestPreferences(
                    "key = ${userName}",
                    "key = Overwrite");
                preferences["key"].ShouldBe("Overwrite");
            };

            it["resets only properties"] = () => {
                preferences["newKey"] = "newValue";
                preferences.Reset();
                preferences.HasKey("newKey").ShouldBeFalse();
                preferences.HasKey("userName").ShouldBeTrue();
            };

            it["resets properties and user"] = () => {
                preferences["newKey"] = "newValue";
                preferences.Reset(true);
                preferences.HasKey("newKey").ShouldBeFalse();
                preferences.HasKey("userName").ShouldBeFalse();
            };

            it["can ToString"] = () => {
                preferences.ToString().ShouldBe("key = ${userName}\nuserName = Max\n");
            };
        };
    }
}

﻿using System.Collections.Generic;
using DesperateDevs.Serialization;
using NSpec;

class describe_Preferences : nspec {

    readonly string pathPrefix = TestExtensions.GetProjectRoot() + "/Tests/TestFixtures/DesperateDevs.Serialization/";

    void when_creating_preferences() {

        Preferences preferences = null;

        context["when properties"] = () => {

            before = () => {
                preferences = new Preferences(pathPrefix + "Preferences.properties", pathPrefix + "EmptyUser.properties");
            };

            it["properties exist"] = () => {
                preferences.propertiesExist.should_be_true();
                preferences.userPropertiesExist.should_be_true();
            };

            it["sets properties"] = () => {
                preferences.properties.count.should_be(1);
                preferences.properties.HasKey("key").should_be_true();
                preferences.userProperties.count.should_be(0);
            };

            it["gets value for key"] = () => {
                preferences["key"].should_be("value");
            };

            it["throws"] = expect<KeyNotFoundException>(() => {
                var x = preferences["unknown"];
            });

            it["has key"] = () => {
                preferences.HasKey("key").should_be_true();
            };

            it["sets key"] = () => {
                preferences["key2"] = "value2";
                preferences["key2"].should_be("value2");
                preferences.HasKey("key2").should_be_true();
            };

            it["can ToString"] = () => {
                preferences.ToString().should_be("key = value\n");
            };

            it["resets preferences"] = () => {
                preferences["newKey"] = "newValue";
                preferences.Reset();
                preferences.HasKey("newKey").should_be_false();
            };
        };

        context["when user properties"] = () => {

            before = () => {
                preferences = new Preferences(pathPrefix + "PreferencesWithUser.properties", pathPrefix + "UserWithKey.properties");
            };

            it["has key"] = () => {
                preferences.HasKey("userName").should_be_true();
            };

            it["resolves placeholder from user properties"] = () => {
                preferences["key"].should_be("Max");
            };

            it["doesn't overwrite value when not different"] = () => {
                preferences["key"] = "Max";
                preferences.properties.ToString().Contains("Max").should_be_false();
            };

            it["overwrites value when different"] = () => {
                preferences["key"] = "Jack";
                preferences.properties.ToString().Contains("Jack").should_be_true();
            };

            it["user properties overwrite default properties"] = () => {
                preferences = new Preferences(pathPrefix + "PreferencesWithUser.properties", pathPrefix + "UserOverwrite.properties");
                preferences["key"].should_be("Overwrite");
            };

            it["can ToString"] = () => {
                preferences.ToString().should_be("key = ${userName}\nuserName = Max\n");
            };
        };
    }
}

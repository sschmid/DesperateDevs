using System.Collections.Generic;
using DesperateDevs.Serialization;
using NSpec;
using Shouldly;

class describe_Properties : nspec {

    void assertProperties(string input, string expectedOutput, Dictionary<string, string> expectedProperties, Properties properties = null) {
        var p = properties ?? new Properties(input);
        var expectedCount = expectedProperties != null ? expectedProperties.Count : 0;
        p.count.ShouldBe(expectedCount);
        p.ToString().ShouldBe(expectedOutput);
        if (expectedProperties != null) {
            foreach (var kv in expectedProperties) {
                p.HasKey(kv.Key).ShouldBeTrue();
                p[kv.Key].ShouldBe(kv.Value);
            }
        }
    }

    void assertProperties(Dictionary<string, string> input, string expectedOutput, Dictionary<string, string> expectedProperties) {
        var p = new Properties(input);
        var expectedCount = expectedProperties != null ? expectedProperties.Count : 0;
        p.count.ShouldBe(expectedCount);
        p.ToString().ShouldBe(expectedOutput);
        if (expectedProperties != null) {
            foreach (var kv in expectedProperties) {
                p.HasKey(kv.Key).ShouldBeTrue();
                p[kv.Key].ShouldBe(kv.Value);
            }
        }
    }

    void when_creating_properties() {

        context["when empty"] = () => {
            it["is empty"] = () => assertProperties(string.Empty, string.Empty, null);
        };

        context["when invalid input string"] = () => {

            it["throws when invalid key"] = () => {
                var input =
                    "some.key.1 = some value 1" + "\n" +
                    "some.key.2" + "\n" +
                    "some.key.3 = some value 3" + "\n";

                InvalidKeyPropertiesException exception = null;
                try {
                    new Properties(input);
                } catch(InvalidKeyPropertiesException ex) {
                    exception = ex;
                }

                exception.ShouldNotBeNull();
                exception.key.ShouldBe("some.key.2");
            };
        };

        context["when single line"] = () => {

            it["creates Properties from single line input string"] = () => {
                const string input = "some.key=some value";

                const string expectedOutput = "some.key = some value\n";
                var expectedProperties = new Dictionary<string, string> {
                    { "some.key", "some value" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };
        };

        context["ignores whitespace"] = () => {

            it["ignores whitespace between key and value"] = () => {
                const string input = "some.key  =  some value";

                const string expectedOutput = "some.key = some value\n";
                var expectedProperties = new Dictionary<string, string> {
                    { "some.key", "some value" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };

            it["ignores whitespace before key"] = () => {
                const string input = "  some.key = some value";

                const string expectedOutput = "some.key = some value\n";
                var expectedProperties = new Dictionary<string, string> {
                    { "some.key", "some value" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };

            it["removes whitespace after value"] = () => {
                const string input = "some.key = some value ";

                const string expectedOutput = "some.key = some value\n";
                var expectedProperties = new Dictionary<string, string> {
                    { "some.key", "some value" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };
        };

        context["when multiline"] = () => {

            it["creates Properties from multiline input string"] = () => {
                var input =
                    "some.key.1=some value 1" + "\n" +
                    " some.key.2 = some value 2" + "\n" +
                    "some.key.3=some value 3" + "\n";

                const string expectedOutput =
                    "some.key.1 = some value 1\n" +
                    "some.key.2 = some value 2\n" +
                    "some.key.3 = some value 3\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "some.key.1", "some value 1" },
                    { "some.key.2", "some value 2" },
                    { "some.key.3", "some value 3" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };

            it["creates Properties from multiline input string where values contain ="] = () => {
                var input =
                    "some.key.1=some=value 1" + "\n" +
                    "some.key.2 ==some value 2" + "\n" +
                    "some.key.3=some value=" + "\n";

                const string expectedOutput =
                    "some.key.1 = some=value 1\n" +
                    "some.key.2 = =some value 2\n" +
                    "some.key.3 = some value=\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "some.key.1", "some=value 1" },
                    { "some.key.2", "=some value 2" },
                    { "some.key.3", "some value=" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };

            it["ignores blank lines"] = () => {
                var input =
                    "\n" +
                    "some.key.1=some value 1" + "\n" +
                    "\n" +
                    " some.key.2 = some value 2" + "\n" +
                    "\n" +
                    "some.key.3=some value 3" + "\n";

                const string expectedOutput =
                    "some.key.1 = some value 1\n" +
                    "some.key.2 = some value 2\n" +
                    "some.key.3 = some value 3\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "some.key.1", "some value 1" },
                    { "some.key.2", "some value 2" },
                    { "some.key.3", "some value 3" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };

            it["ignores lines starting with #"] = () => {
                var input =
                    "#some.key.1=some value 1" + "\n" +
                    "  #some.key.2 = some value 2 " + "\n" +
                    "some.key.3=some value 3" + "\n";

                const string expectedOutput =
                    "some.key.3 = some value 3\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "some.key.3", "some value 3" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };

            it["supports multiline values ending with \\"] = () => {
                var input =
                    "some.key=some val\\" + "\n" + "ue" + "\n" +
                    "some.other.key=other val\\" + "\n" + "ue" + "\n";

                const string expectedOutput =
                    "some.key = some value\n" +
                    "some.other.key = other value\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "some.key", "some value" },
                    { "some.other.key", "other value" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };

            it["trims leading whitespace of multilines"] = () => {
                var input =
                    "some.key=some val\\" + "\n" + "   ue" + "\n" +
                    "some.other.key=other val\\" + "\n" + "   ue" + "\n";

                const string expectedOutput =
                    "some.key = some value\n" +
                    "some.other.key = other value\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "some.key", "some value" },
                    { "some.other.key", "other value" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };

            it["has all keys"] = () => {
                var input =
                    "some.key.1=some value 1" + "\n" +
                    " some.key.2 = some value 2 " + "\n" +
                    "some.key.3=some value 3" + "\n";

                var keys = new Properties(input).keys;
                keys.Length.ShouldBe(3);
                keys.ShouldContain("some.key.1");
                keys.ShouldContain("some.key.2");
                keys.ShouldContain("some.key.3");
            };

            it["has all values"] = () => {
                var input =
                    "some.key.1=some value 1" + "\n" +
                    " some.key.2 = some value 2 " + "\n" +
                    "some.key.3=some value 3" + "\n";

                var values = new Properties(input).values;
                values.Length.ShouldBe(3);
                values.ShouldContain("some value 1");
                values.ShouldContain("some value 2");
                values.ShouldContain("some value 3");
            };

            it["gets a dictionary"] = () => {
                var input =
                    "some.key.1=some value 1" + "\n" +
                    " some.key.2 = some value 2 " + "\n" +
                    "some.key.3=some value 3" + "\n";

                var dict = new Properties(input).ToDictionary();
                dict.Count.ShouldBe(3);
                dict.ContainsKey("some.key.1").ShouldBeTrue();
                dict.ContainsKey("some.key.2").ShouldBeTrue();
                dict.ContainsKey("some.key.3").ShouldBeTrue();
            };

            it["gets a dictionary copy"] = () => {
                var input =
                    "some.key.1=some value 1" + "\n" +
                    " some.key.2 = some value 2 " + "\n" +
                    "some.key.3=some value 3" + "\n";

                var properties = new Properties(input);
                properties.ToDictionary().ShouldNotBeSameAs(properties.ToDictionary());
            };
        };

        context["when replacing special characters in values"] = () => {

            it["replaces \\n with newline"] = () => {
                var input =
                    @"some.key=some\nvalue" + "\n" +
                    @"some.other.key=other\nvalue" + "\n";

                const string expectedOutput =
                    @"some.key = some\nvalue" + "\n" +
                    @"some.other.key = other\nvalue" + "\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "some.key", "some\nvalue" },
                    { "some.other.key", "other\nvalue" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };

            it["replaces \\t with tabs"] = () => {
                var input =
                    @"some.key=some\tvalue" + "\n" +
                    @"some.other.key=other\tvalue" + "\n";

                const string expectedOutput =
                    @"some.key = some\tvalue" + "\n" +
                    @"some.other.key = other\tvalue" + "\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "some.key", "some\tvalue" },
                    { "some.other.key", "other\tvalue" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };
        };

        context["adding properties"] = () => {

            Properties p = null;

            before = () => {
                p = new Properties();
            };

            it["set new property"] = () => {
                p["key"] = "value";
                p["key"].ShouldBe("value");
            };

            it["trims key"] = () => {
                p[" key "] = "value";
                p["key"].ShouldBe("value");
            };

            it["trims start of value"] = () => {
                p["key"] = " value";
                p["key"].ShouldBe("value");
            };

            it["removes trailing whitespace of value"] = () => {
                p["key"] = "value";
                p["key"].ShouldBe("value");
            };

            it["adds properties from dictionary"] = () => {
                var dict = new Dictionary<string, string> {
                    { "key1", "value1"},
                    { "key2", "value2"}
                };

                p.AddProperties(dict, true);

                p.count.ShouldBe(dict.Count);
                p["key1"].ShouldBe("value1");
                p["key2"].ShouldBe("value2");
            };

            it["overwrites existing properties from dictionary"] = () => {
                var dict = new Dictionary<string, string> {
                    { "key1", "value1"},
                    { "key2", "value2"}
                };

                p["key1"] = "existingKey";
                p.AddProperties(dict, true);

                p.count.ShouldBe(dict.Count);
                p["key1"].ShouldBe("value1");
                p["key2"].ShouldBe("value2");
            };

            it["only adds missing properties from dictionary"] = () => {
                var dict = new Dictionary<string, string> {
                    { "key1", "value1"},
                    { "key2", "value2"}
                };

                p["key1"] = "existingKey";
                p.AddProperties(dict, false);

                p.count.ShouldBe(dict.Count);
                p["key1"].ShouldBe("existingKey");
                p["key2"].ShouldBe("value2");
            };
        };

        context["removing properties"] = () => {

            Properties p = null;

            before = () => {
                p = new Properties();
                p["key"] = "value";
            };

            it["set new property"] = () => {
                p.RemoveProperty("key");
                p.HasKey("key").ShouldBeFalse();
            };
        };

        context["placeholder"] = () => {

            it["replaces placeholder within ${...}"] = () => {
                var input =
                    "project.name = Project" + "\n" +
                    "project.domain = com.sschmid" + "\n" +
                    "project.bundleId = ${project.domain}.${project.name}" + "\n";

                const string expectedOutput =
                    "project.name = Project\n" +
                    "project.domain = com.sschmid\n" +
                    "project.bundleId = ${project.domain}.${project.name}\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "project.name", "Project" },
                    { "project.domain", "com.sschmid" },
                    { "project.bundleId", "com.sschmid.Project" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };

            it["replaces placeholder when adding new property"] = () => {
                var input =
                    "project.name = Project" + "\n" +
                    "project.domain = com.sschmid" + "\n";

                const string expectedOutput =
                    "project.name = Project\n" +
                    "project.domain = com.sschmid\n" +
                    "project.bundleId = ${project.domain}.${project.name}\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "project.name", "Project" },
                    { "project.domain", "com.sschmid" },
                    { "project.bundleId", "com.sschmid.Project" }
                };

                var p = new Properties(input);
                p["project.bundleId"] = "${project.domain}.${project.name}";

                assertProperties(input, expectedOutput, expectedProperties, p);
            };

            it["doesn't replace placeholder when not resolvable"] = () => {
                var input =
                    "project.name = Project" + "\n" +
                    "project.domain = com.sschmid" + "\n" +
                    "project.bundleId = ${Xproject.domain}.${Xproject.name}" + "\n";

                const string expectedOutput =
                    "project.name = Project\n" +
                    "project.domain = com.sschmid\n" +
                    "project.bundleId = ${Xproject.domain}.${Xproject.name}\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "project.name", "Project" },
                    { "project.domain", "com.sschmid" },
                    { "project.bundleId", "${Xproject.domain}.${Xproject.name}" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };
        };

        context["different line endings"] = () => {

            it["converts and normalizes line endings"] = () => {
                var input =
                    "project.name = Project" + "\n" +
                    "project.domain = com.sschmid" + "\r" +
                    "project.bundleId = ${project.domain}.${project.name}" + "\r\n";

                const string expectedOutput =
                    "project.name = Project\n" +
                    "project.domain = com.sschmid\n" +
                    "project.bundleId = ${project.domain}.${project.name}\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "project.name", "Project" },
                    { "project.domain", "com.sschmid" },
                    { "project.bundleId", "com.sschmid.Project" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };
        };

        context["minified string"] = () => {

            it["puts values in one line"] = () => {
                var properties = new Properties(
@"key = value1, value2, value3
key2 = value4");

                properties.ToMinifiedString().ShouldBe(
@"key=value1, value2, value3
key2=value4
");
            };
        };

        xcontext["double quotes mode"] = () => {

            it["surrounds added values with double quotes"] = () => {
                var properties = new Properties(string.Empty);
                properties.doubleQuoteMode = true;
                properties["key"] = "value";
                properties["key"].ShouldBe("\"value\"");
            };

            it["doesn't surround added values with double quotes when already double quoted"] = () => {
                var properties = new Properties(string.Empty);
                properties.doubleQuoteMode = true;
                properties["key"] = "\"value\"";
                properties["key"].ShouldBe("\"value\"");
            };

            it["surrounds existing values with double quotes"] = () => {
                var properties = new Properties(string.Empty);
                properties["key"] = "value";
                properties.doubleQuoteMode = true;
                properties["key"].ShouldBe("\"value\"");
            };

            it["surrounds other properties values with double quotes"] = () => {
                var properties = new Properties(new Dictionary<string, string> {
                    { "key", "value" }
                });
                properties.doubleQuoteMode = true;
                properties["key"].ShouldBe("\"value\"");
            };

            it["removes double quotes when reading values"] = () => {
                var properties = new Properties(string.Empty);
                properties["key"] = "\"value\"";
                properties["key"].ShouldBe("\"value\"");
                properties.doubleQuoteMode = true;
                properties["key"].ShouldBe("value");
            };

            it["disables double quotes mode"] = () => {
                var properties = new Properties(string.Empty);
                properties["key"] = "\"value\"";
                properties["key"].ShouldBe("\"value\"");
                properties.doubleQuoteMode = true;
                properties.doubleQuoteMode = false;
                properties["key"].ShouldBe("\"value\"");
            };
        };
    }

    void when_creating_properties_from_dictionary() {

        it["creates properties from dictionary"] = () => {
            var input = new Dictionary<string, string> {
                { "key1", "value1"},
                { "key2", "value2"}
            };

            assertProperties(
                input,
                "key1 = value1" + "\n" +
                "key2 = value2" + "\n",
                input
            );
        };

        it["uses copy of original dictionary"] = () => {
            var input = new Dictionary<string, string> {
                { "key1", "value1"},
                { "key2", "value2"}
            };

            var p = new Properties(input);
            p["key1"] = "newValue1";

            input["key1"].ShouldBe("value1");
            p["key1"].ShouldBe("newValue1");
        };

        context["placeholder"] = () => {

            it["replaces placeholder within ${...}"] = () => {
                var input = new Dictionary<string, string> {
                    { "project.name", "Project"},
                    { "project.domain", "com.sschmid"},
                    { "project.bundleId", "${project.domain}.${project.name}"}
                };

                const string expectedOutput =
                    "project.name = Project\n" +
                    "project.domain = com.sschmid\n" +
                    "project.bundleId = ${project.domain}.${project.name}\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "project.name", "Project" },
                    { "project.domain", "com.sschmid" },
                    { "project.bundleId", "com.sschmid.Project" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };
        };
    }
}

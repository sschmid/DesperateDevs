using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.Serialization.Tests
{
    public class PropertiesTests
    {
        [Fact]
        public void IsEmpty()
        {
            AssertProperties(string.Empty, string.Empty, null);
        }

        [Fact]
        public void CreatesPropertiesFromSingleLineInputString()
        {
            AssertProperties(
                "some.key = some value",
                "some.key = some value\n", new Dictionary<string, string>
                {
                    {"some.key", "some value"}
                }
            );
        }

        [Fact]
        public void TryGetsValueForKey()
        {
            var properties = new Properties("some.key = some value\n");
            properties.TryGetValue("some.key", out var value).Should().BeTrue();
            value.Should().Be("some value");
        }

        [Fact]
        public void DoesNotTryGetValueForUnknownKey()
        {
            var properties = new Properties("some.key = some value\n");
            properties.TryGetValue("unknown", out var value).Should().BeFalse();
            value.Should().BeNull();
        }

        [Fact]
        public void CreatesPropertiesFromMinifiedSingleLineInputString()
        {
            AssertProperties(
                "some.key=some value",
                "some.key = some value\n", new Dictionary<string, string>
                {
                    {"some.key", "some value"}
                }
            );
        }

        [Fact]
        public void IgnoresWhitespaceBetweenKeyAndValue()
        {
            AssertProperties(
                "some.key  =  some value",
                "some.key = some value\n", new Dictionary<string, string>
                {
                    {"some.key", "some value"}
                }
            );
        }

        [Fact]
        public void IgnoresWhitespaceBeforeKey()
        {
            AssertProperties(
                "  some.key = some value",
                "some.key = some value\n",
                new Dictionary<string, string>
                {
                    {"some.key", "some value"}
                }
            );
        }

        [Fact]
        public void RemovesTrailingWhitespace()
        {
            AssertProperties(
                "some.key = some value ",
                "some.key = some value\n",
                new Dictionary<string, string>
                {
                    {"some.key", "some value"}
                }
            );
        }

        [Fact]
        public void ToStringFormatsCommaSeparatedValues()
        {
            AssertProperties(
                "some.key = one, two, three,four ",
                "some.key = one, \\\n" +
                "           two, \\\n" +
                "           three, \\\n" +
                "           four\n",
                new Dictionary<string, string>
                {
                    {"some.key", "one, two, three,four"}
                }
            );
        }

        [Fact]
        public void CreatesPropertiesFromMultilineInputString()
        {
            AssertProperties(
                "some.key.1 = some value 1\n" +
                "some.key.2 = some value 2\n" +
                "some.key.3 = some value 3\n",
                "some.key.1 = some value 1\n" +
                "some.key.2 = some value 2\n" +
                "some.key.3 = some value 3\n",
                new Dictionary<string, string>
                {
                    {"some.key.1", "some value 1"},
                    {"some.key.2", "some value 2"},
                    {"some.key.3", "some value 3"}
                }
            );
        }

        [Fact]
        public void CreatesPropertiesFromMultilineInputStringWhereValuesContainsEquals()
        {
            AssertProperties(
                "some.key.1 = some=value 1\n" +
                "some.key.2 = =some value 2\n" +
                "some.key.3 = some value=\n",
                "some.key.1 = some=value 1\n" +
                "some.key.2 = =some value 2\n" +
                "some.key.3 = some value=\n",
                new Dictionary<string, string>
                {
                    {"some.key.1", "some=value 1"},
                    {"some.key.2", "=some value 2"},
                    {"some.key.3", "some value="}
                }
            );
        }

        [Fact]
        public void ThrowsWhenInvalidProperty()
        {
            FluentActions.Invoking(() =>
                    new Properties("some.key.1 = some value 1\n" +
                                   "some.key.2\n" +
                                   "some.key.3 = some value 3\n")
                ).Should().Throw<InvalidPropertyException>()
                .Which.Key.Should().Be("some.key.2");
        }

        [Fact]
        public void IgnoresBlankLines()
        {
            AssertProperties(
                "\n" +
                "some.key.1 = some value 1\n" +
                "\n" +
                " some.key.2 = some value 2\n" +
                "\n" +
                "some.key.3 = some value 3\n",
                "some.key.1 = some value 1\n" +
                "some.key.2 = some value 2\n" +
                "some.key.3 = some value 3\n",
                new Dictionary<string, string>
                {
                    {"some.key.1", "some value 1"},
                    {"some.key.2", "some value 2"},
                    {"some.key.3", "some value 3"}
                }
            );
        }

        [Fact]
        public void IgnoresComments()
        {
            AssertProperties(
                "#some.key.1 = some value 1\n" +
                "  #some.key.2 = some value 2\n" +
                "some.key.3 = some value #3\n",
                "some.key.3 = some value #3\n",
                new Dictionary<string, string>
                {
                    {"some.key.3", "some value #3"}
                }
            );
        }

        [Fact]
        public void SupportsMultilineValues()
        {
            AssertProperties(
                "some.key = some val\\\nue\n" +
                "some.other.key = other val\\\nue\n",
                "some.key = some value\n" +
                "some.other.key = other value\n",
                new Dictionary<string, string>
                {
                    {"some.key", "some value"},
                    {"some.other.key", "other value"}
                }
            );
        }

        [Fact]
        public void TrimsLeadingWhitespaceOfMultilineValues()
        {
            AssertProperties(
                "some.key = some val\\\n   ue\n" +
                "some.other.key = other val\\\n   ue\n",
                "some.key = some value\n" +
                "some.other.key = other value\n",
                new Dictionary<string, string>
                {
                    {"some.key", "some value"},
                    {"some.other.key", "other value"}
                }
            );
        }

        [Fact]
        public void HasAllKeys()
        {
            var keys = new Properties(
                "some.key.1 = some value 1\n" +
                "some.key.2 = some value 2\n" +
                "some.key.3 = some value 3\n"
            ).Keys.ToArray();

            keys.Length.Should().Be(3);
            keys.Should().Contain("some.key.1");
            keys.Should().Contain("some.key.2");
            keys.Should().Contain("some.key.3");
        }

        [Fact]
        public void HasAllValues()
        {
            var values = new Properties(
                "some.key.1 = some value 1\n" +
                "some.key.2 = some value 2\n" +
                "some.key.3 = some value 3\n"
            ).Values.ToArray();

            values.Length.Should().Be(3);
            values.Should().Contain("some value 1");
            values.Should().Contain("some value 2");
            values.Should().Contain("some value 3");
        }

        [Fact]
        public void GetsDictionary()
        {
            var properties = new Properties(
                "some.key.1 = some value 1\n" +
                "some.key.2 = some value 2\n" +
                "some.key.3 = some value 3\n"
            );
            var dict = properties.ToDictionary();
            dict.Count.Should().Be(3);
            dict.ContainsKey("some.key.1").Should().BeTrue();
            dict.ContainsKey("some.key.2").Should().BeTrue();
            dict.ContainsKey("some.key.3").Should().BeTrue();

            properties.ToDictionary().Should().NotBeSameAs(properties.ToDictionary());
        }

        [Fact]
        public void ReplacesNewline()
        {
            AssertProperties(
                @"some.key = some\nvalue" + "\n" +
                @"some.other.key = other\nvalue" + "\n",
                @"some.key = some\nvalue" + "\n" +
                @"some.other.key = other\nvalue" + "\n",
                new Dictionary<string, string>
                {
                    {"some.key", "some\nvalue"},
                    {"some.other.key", "other\nvalue"}
                }
            );
        }

        [Fact]
        public void ReplacesTabs()
        {
            AssertProperties(
                @"some.key = some\tvalue" + "\n" +
                @"some.other.key = other\tvalue" + "\n",
                @"some.key = some\tvalue" + "\n" +
                @"some.other.key = other\tvalue" + "\n",
                new Dictionary<string, string>
                {
                    {"some.key", "some\tvalue"},
                    {"some.other.key", "other\tvalue"}
                }
            );
        }

        [Fact]
        public void SetsNewProperty()
        {
            var properties = new Properties();
            properties["key"] = "value";
            properties["key"].Should().Be("value");
        }

        [Fact]
        public void DoesNotTrimKey()
        {
            var properties = new Properties();
            properties[" key "] = "value";
            properties[" key "].Should().Be("value");
        }

        [Fact]
        public void DoesNotTrimValue()
        {
            var properties = new Properties();
            properties["key"] = " value ";
            properties["key"].Should().Be(" value ");
        }

        [Fact]
        public void AddsPropertiesFromDictionary()
        {
            var properties = new Properties();
            var dict = new Dictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"}
            };

            properties.AddProperties(dict, true);

            properties.Count.Should().Be(dict.Count);
            properties["key1"].Should().Be("value1");
            properties["key2"].Should().Be("value2");
        }

        [Fact]
        public void OverwritesExistingPropertiesFromDictionary()
        {
            var properties = new Properties();
            var dict = new Dictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"}
            };

            properties["key1"] = "existingKey";
            properties.AddProperties(dict, true);

            properties.Count.Should().Be(dict.Count);
            properties["key1"].Should().Be("value1");
            properties["key2"].Should().Be("value2");
        }

        [Fact]
        public void OnlyAddsMissingKeysFromDictionary()
        {
            var properties = new Properties();
            var dict = new Dictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"}
            };

            properties["key1"] = "existingKey";
            properties.AddProperties(dict, false);

            properties.Count.Should().Be(dict.Count);
            properties["key1"].Should().Be("existingKey");
            properties["key2"].Should().Be("value2");
        }

        [Fact]
        public void RemovesKey()
        {
            var properties = new Properties();
            properties["key"] = "value";
            properties.RemoveProperty("key");
            properties.HasKey("key").Should().BeFalse();
        }

        [Fact]
        public void ReplacesPlaceholder()
        {
            AssertProperties(
                "project.name = Project\n" +
                "project.domain = com.desperatedevs\n" +
                "project.bundleId = ${project.domain}.${project.name}\n",
                "project.name = Project\n" +
                "project.domain = com.desperatedevs\n" +
                "project.bundleId = ${project.domain}.${project.name}\n",
                new Dictionary<string, string>
                {
                    {"project.name", "Project"},
                    {"project.domain", "com.desperatedevs"},
                    {"project.bundleId", "com.desperatedevs.Project"}
                }
            );
        }

        [Fact]
        public void ReplacesPlaceholderWhenAddingNewProperty()
        {
            AssertProperties(
                new Properties("project.name = Project\nproject.domain = com.desperatedevs\n")
                {
                    ["project.bundleId"] = "${project.domain}.${project.name}"
                },
                "project.name = Project\n" +
                "project.domain = com.desperatedevs\n" +
                "project.bundleId = ${project.domain}.${project.name}\n",
                new Dictionary<string, string>
                {
                    {"project.name", "Project"},
                    {"project.domain", "com.desperatedevs"},
                    {"project.bundleId", "com.desperatedevs.Project"}
                }
            );
        }

        [Fact]
        public void DoesNotReplacePlaceholderWhenNotResolvable()
        {
            AssertProperties(
                "project.name = Project\n" +
                "project.domain = com.desperatedevs\n" +
                "project.bundleId = ${unknown.domain}.${unknown.name}\n",
                "project.name = Project\n" +
                "project.domain = com.desperatedevs\n" +
                "project.bundleId = ${unknown.domain}.${unknown.name}\n",
                new Dictionary<string, string>
                {
                    {"project.name", "Project"},
                    {"project.domain", "com.desperatedevs"},
                    {"project.bundleId", "${unknown.domain}.${unknown.name}"}
                }
            );
        }

        [Fact]
        public void ConvertsLineEndings()
        {
            AssertProperties(
                "some.key.1 = some value 1\n" +
                "some.key.2 = some value 2\r" +
                "some.key.3 = some value 3\r\n",
                "some.key.1 = some value 1\n" +
                "some.key.2 = some value 2\n" +
                "some.key.3 = some value 3\n",
                new Dictionary<string, string>
                {
                    {"some.key.1", "some value 1"},
                    {"some.key.2", "some value 2"},
                    {"some.key.3", "some value 3"}
                }
            );
        }

        [Fact]
        public void ToMinifiedString()
        {
            var properties = new Properties(
                @"key = value1, value2, value3
key2 = value4");

            properties.ToMinifiedString().Should().Be(
                @"key=value1, value2, value3
key2=value4
");
        }

        [Fact]
        public void ToDoubleQuotedValueMinifiedString()
        {
            var properties = new Properties(
                @"key = ""value1, value2, value3""
key2 = ""value4""", true);

            properties.ToMinifiedString().Should().Be(
                @"key=""value1, value2, value3""
key2=""value4""
");
        }

        [Fact]
        public void CreatesPropertiesFromDoubleQuotedValueInputString()
        {
            AssertProperties(
                new Properties("some.key.1 = \"some value 1\"\nsome.key.2 = \"some value 2\"\n", true),
                "some.key.1 = \"some value 1\"\n" +
                "some.key.2 = \"some value 2\"\n",
                new Dictionary<string, string>
                {
                    {"some.key.1", "some value 1"},
                    {"some.key.2", "some value 2"}
                }
            );
        }

        [Fact]
        public void DoubleQuotedValuesFromAddedValues()
        {
            AssertProperties(
                new Properties("some.key.1 = \"some value 1\"\nsome.key.2 = \"some value 2\"\n", true)
                {
                    ["some.key.3"] = "some value 3"
                },
                "some.key.1 = \"some value 1\"\n" +
                "some.key.2 = \"some value 2\"\n" +
                "some.key.3 = \"some value 3\"\n",
                new Dictionary<string, string>
                {
                    {"some.key.1", "some value 1"},
                    {"some.key.2", "some value 2"},
                    {"some.key.3", "some value 3"}
                }
            );
        }

        void AssertProperties(string input, string expectedOutput, Dictionary<string, string> expectedProperties)
        {
            AssertProperties(new Properties(input), expectedOutput, expectedProperties);
        }

        void AssertProperties(Properties properties, string expectedOutput, Dictionary<string, string> expectedProperties)
        {
            var expectedCount = expectedProperties?.Count ?? 0;
            properties.Count.Should().Be(expectedCount);
            properties.ToString().Should().Be(expectedOutput);
            if (expectedProperties != null)
            {
                foreach (var kvp in expectedProperties)
                {
                    properties.HasKey(kvp.Key).Should().BeTrue();
                    properties[kvp.Key].Should().Be(kvp.Value);
                }
            }
        }
    }
}

using System.Collections.Generic;
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
            const string input = "some.key=some value";
            const string expectedOutput = "some.key = some value\n";
            var expectedProperties = new Dictionary<string, string>
            {
                {"some.key", "some value"}
            };

            AssertProperties(input, expectedOutput, expectedProperties);
        }

        [Fact]
        public void IgnoresWhitespaceBetweenKeyAndValue()
        {
            const string input = "some.key  =  some value";
            const string expectedOutput = "some.key = some value\n";
            var expectedProperties = new Dictionary<string, string>
            {
                {"some.key", "some value"}
            };

            AssertProperties(input, expectedOutput, expectedProperties);
        }

        [Fact]
        public void IgnoresWhitespaceBeforeKey()
        {
            const string input = "  some.key = some value";
            const string expectedOutput = "some.key = some value\n";
            var expectedProperties = new Dictionary<string, string>
            {
                {"some.key", "some value"}
            };

            AssertProperties(input, expectedOutput, expectedProperties);
        }

        [Fact]
        public void RemovesTrailingWhitespace()
        {
            const string input = "some.key = some value ";
            const string expectedOutput = "some.key = some value\n";
            var expectedProperties = new Dictionary<string, string>
            {
                {"some.key", "some value"}
            };

            AssertProperties(input, expectedOutput, expectedProperties);
        }

        [Fact]
        public void CreatesPropertiesFromMultilineInputString()
        {
            var input =
                "some.key.1=some value 1" + "\n" +
                " some.key.2 = some value 2" + "\n" +
                "some.key.3=some value 3" + "\n";

            const string expectedOutput =
                "some.key.1 = some value 1\n" +
                "some.key.2 = some value 2\n" +
                "some.key.3 = some value 3\n";

            var expectedProperties = new Dictionary<string, string>
            {
                {"some.key.1", "some value 1"},
                {"some.key.2", "some value 2"},
                {"some.key.3", "some value 3"}
            };

            AssertProperties(input, expectedOutput, expectedProperties);
        }

        [Fact]
        public void CreatesPropertiesFromMultilineInputStringWhereValuesContainsEquals()
        {
            var input =
                "some.key.1=some=value 1" + "\n" +
                "some.key.2 ==some value 2" + "\n" +
                "some.key.3=some value=" + "\n";

            const string expectedOutput =
                "some.key.1 = some=value 1\n" +
                "some.key.2 = =some value 2\n" +
                "some.key.3 = some value=\n";

            var expectedProperties = new Dictionary<string, string>
            {
                {"some.key.1", "some=value 1"},
                {"some.key.2", "=some value 2"},
                {"some.key.3", "some value="}
            };

            AssertProperties(input, expectedOutput, expectedProperties);
        }

        [Fact]
        public void ThrowsWhenInvalidKey()
        {
            var input =
                "some.key.1 = some value 1" + "\n" +
                "some.key.2" + "\n" +
                "some.key.3 = some value 3" + "\n";

            InvalidKeyPropertiesException exception = null;
            try
            {
                new Properties(input);
            }
            catch (InvalidKeyPropertiesException ex)
            {
                exception = ex;
            }

            exception.Should().NotBeNull();
            exception.key.Should().Be("some.key.2");
        }

        [Fact]
        public void IgnoresBlankLines()
        {
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

            var expectedProperties = new Dictionary<string, string>
            {
                {"some.key.1", "some value 1"},
                {"some.key.2", "some value 2"},
                {"some.key.3", "some value 3"}
            };

            AssertProperties(input, expectedOutput, expectedProperties);
        }

        [Fact]
        public void IgnoresComments()
        {
            var input =
                "#some.key.1=some value 1" + "\n" +
                "  #some.key.2 = some value 2 " + "\n" +
                "some.key.3=some value 3" + "\n";

            const string expectedOutput =
                "some.key.3 = some value 3\n";

            var expectedProperties = new Dictionary<string, string>
            {
                {"some.key.3", "some value 3"}
            };

            AssertProperties(input, expectedOutput, expectedProperties);
        }

        [Fact]
        public void SupportsMultilineValues()
        {
            var input =
                "some.key=some val\\" + "\n" + "ue" + "\n" +
                "some.other.key=other val\\" + "\n" + "ue" + "\n";

            const string expectedOutput =
                "some.key = some value\n" +
                "some.other.key = other value\n";

            var expectedProperties = new Dictionary<string, string>
            {
                {"some.key", "some value"},
                {"some.other.key", "other value"}
            };

            AssertProperties(input, expectedOutput, expectedProperties);
        }

        [Fact]
        public void TrimsLeadingWhitespaceOfMultilineValues()
        {
            var input =
                "some.key=some val\\" + "\n" + "   ue" + "\n" +
                "some.other.key=other val\\" + "\n" + "   ue" + "\n";

            const string expectedOutput =
                "some.key = some value\n" +
                "some.other.key = other value\n";

            var expectedProperties = new Dictionary<string, string>
            {
                {"some.key", "some value"},
                {"some.other.key", "other value"}
            };

            AssertProperties(input, expectedOutput, expectedProperties);
        }

        [Fact]
        public void HasAllKeys()
        {
            var input =
                "some.key.1=some value 1" + "\n" +
                " some.key.2 = some value 2 " + "\n" +
                "some.key.3=some value 3" + "\n";

            var keys = new Properties(input).keys;
            keys.Length.Should().Be(3);
            keys.Should().Contain("some.key.1");
            keys.Should().Contain("some.key.2");
            keys.Should().Contain("some.key.3");
        }

        [Fact]
        public void HasAllValues()
        {
            var input =
                "some.key.1=some value 1" + "\n" +
                " some.key.2 = some value 2 " + "\n" +
                "some.key.3=some value 3" + "\n";

            var values = new Properties(input).values;
            values.Length.Should().Be(3);
            values.Should().Contain("some value 1");
            values.Should().Contain("some value 2");
            values.Should().Contain("some value 3");
        }

        [Fact]
        public void GetsDictionary()
        {
            var input =
                "some.key.1=some value 1" + "\n" +
                " some.key.2 = some value 2 " + "\n" +
                "some.key.3=some value 3" + "\n";

            var properties = new Properties(input);
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
            var input =
                @"some.key=some\nvalue" + "\n" +
                @"some.other.key=other\nvalue" + "\n";

            const string expectedOutput =
                @"some.key = some\nvalue" + "\n" +
                @"some.other.key = other\nvalue" + "\n";

            var expectedProperties = new Dictionary<string, string>
            {
                {"some.key", "some\nvalue"},
                {"some.other.key", "other\nvalue"}
            };

            AssertProperties(input, expectedOutput, expectedProperties);
        }

        [Fact]
        public void ReplacesTabs()
        {
            var input =
                @"some.key=some\tvalue" + "\n" +
                @"some.other.key=other\tvalue" + "\n";

            const string expectedOutput =
                @"some.key = some\tvalue" + "\n" +
                @"some.other.key = other\tvalue" + "\n";

            var expectedProperties = new Dictionary<string, string>
            {
                {"some.key", "some\tvalue"},
                {"some.other.key", "other\tvalue"}
            };

            AssertProperties(input, expectedOutput, expectedProperties);
        }

        [Fact]
        public void SetsNewProperty()
        {
            var p = new Properties();
            p["key"] = "value";
            p["key"].Should().Be("value");
        }

        [Fact]
        public void TrimsKey()
        {
            var p = new Properties();
            p[" key "] = "value";
            p["key"].Should().Be("value");
        }

        [Fact]
        public void TrimsStartOfValue()
        {
            var p = new Properties();
            p["key"] = " value";
            p["key"].Should().Be("value");
        }

        [Fact]
        public void RemovesTrailingWhiteSpace()
        {
            var p = new Properties();
            p["key"] = "value";
            p["key"].Should().Be("value");
        }

        [Fact]
        public void AddsPropertiesFromDictionary()
        {
            var p = new Properties();
            var dict = new Dictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"}
            };

            p.AddProperties(dict, true);

            p.count.Should().Be(dict.Count);
            p["key1"].Should().Be("value1");
            p["key2"].Should().Be("value2");
        }

        [Fact]
        public void OverwritesExistingPropertiesFromDictionary()
        {
            var p = new Properties();
            var dict = new Dictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"}
            };

            p["key1"] = "existingKey";
            p.AddProperties(dict, true);

            p.count.Should().Be(dict.Count);
            p["key1"].Should().Be("value1");
            p["key2"].Should().Be("value2");
        }

        [Fact]
        public void OnlyAddsMissingKeysFromDictionary()
        {
            var p = new Properties();
            var dict = new Dictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"}
            };

            p["key1"] = "existingKey";
            p.AddProperties(dict, false);

            p.count.Should().Be(dict.Count);
            p["key1"].Should().Be("existingKey");
            p["key2"].Should().Be("value2");
        }

        [Fact]
        public void RemovesKey()
        {
            var p = new Properties();
            p["key"] = "value";
            p.RemoveProperty("key");
            p.HasKey("key").Should().BeFalse();
        }

        [Fact]
        public void ReplacesPlaceholder()
        {
            var input =
                "project.name = Project" + "\n" +
                "project.domain = com.desperatedevs" + "\n" +
                "project.bundleId = ${project.domain}.${project.name}" + "\n";

            const string expectedOutput =
                "project.name = Project\n" +
                "project.domain = com.desperatedevs\n" +
                "project.bundleId = ${project.domain}.${project.name}\n";

            var expectedProperties = new Dictionary<string, string>
            {
                {"project.name", "Project"},
                {"project.domain", "com.desperatedevs"},
                {"project.bundleId", "com.desperatedevs.Project"}
            };

            AssertProperties(input, expectedOutput, expectedProperties);
        }

        [Fact]
        public void ReplacesPlaceholderWhenAddingNewProperty()
        {
            var input =
                "project.name = Project" + "\n" +
                "project.domain = com.desperatedevs" + "\n";

            const string expectedOutput =
                "project.name = Project\n" +
                "project.domain = com.desperatedevs\n" +
                "project.bundleId = ${project.domain}.${project.name}\n";

            var expectedProperties = new Dictionary<string, string>
            {
                {"project.name", "Project"},
                {"project.domain", "com.desperatedevs"},
                {"project.bundleId", "com.desperatedevs.Project"}
            };

            var p = new Properties(input);
            p["project.bundleId"] = "${project.domain}.${project.name}";

            AssertProperties(input, expectedOutput, expectedProperties, p);
        }

        [Fact]
        public void DoesNotReplacePlaceholderWhenNotResolvable()
        {
            var input =
                "project.name = Project" + "\n" +
                "project.domain = com.desperatedevs" + "\n" +
                "project.bundleId = ${Xproject.domain}.${Xproject.name}" + "\n";

            const string expectedOutput =
                "project.name = Project\n" +
                "project.domain = com.desperatedevs\n" +
                "project.bundleId = ${Xproject.domain}.${Xproject.name}\n";

            var expectedProperties = new Dictionary<string, string>
            {
                {"project.name", "Project"},
                {"project.domain", "com.desperatedevs"},
                {"project.bundleId", "${Xproject.domain}.${Xproject.name}"}
            };

            AssertProperties(input, expectedOutput, expectedProperties);
        }

        [Fact]
        public void ConvertsLineEndings()
        {
            var input =
                "project.name = Project" + "\n" +
                "project.domain = com.desperatedevs" + "\r" +
                "project.bundleId = ${project.domain}.${project.name}" + "\r\n";

            const string expectedOutput =
                "project.name = Project\n" +
                "project.domain = com.desperatedevs\n" +
                "project.bundleId = ${project.domain}.${project.name}\n";

            var expectedProperties = new Dictionary<string, string>
            {
                {"project.name", "Project"},
                {"project.domain", "com.desperatedevs"},
                {"project.bundleId", "com.desperatedevs.Project"}
            };

            AssertProperties(input, expectedOutput, expectedProperties);
        }

        [Fact]
        public void RemovesWhitespace()
        {
            var properties = new Properties(
                @"key = value1, value2, value3
key2 = value4");

            properties.ToMinifiedString().Should().Be(
                @"key=value1, value2, value3
key2=value4
");
        }

        [Fact(Skip = "TODO")]
        public void SurroundsAddedValuesWithDoubleQuotes()
        {
            var properties = new Properties(string.Empty);
            properties.doubleQuoteMode = true;
            properties["key"] = "value";
            properties["key"].Should().Be("\"value\"");
        }

        [Fact(Skip = "TODO")]
        public void DoesNotSurroundAddedValuesWithDoubleQuotesWhenAlreadyDoubleQuoted()
        {
            var properties = new Properties(string.Empty);
            properties.doubleQuoteMode = true;
            properties["key"] = "\"value\"";
            properties["key"].Should().Be("\"value\"");
        }

        [Fact(Skip = "TODO")]
        public void SurroundsExistingValuesWithDoubleQuotes()
        {
            var properties = new Properties(string.Empty);
            properties["key"] = "value";
            properties.doubleQuoteMode = true;
            properties["key"].Should().Be("\"value\"");
        }

        [Fact(Skip = "TODO")]
        public void SurroundsOtherPropertiesValuesWithDoubleQuotes()
        {
            var properties = new Properties(new Dictionary<string, string>
            {
                {"key", "value"}
            });
            properties.doubleQuoteMode = true;
            properties["key"].Should().Be("\"value\"");
        }

        [Fact]
        public void RemovesDoubleQuotesWhenReadingValues()
        {
            var properties = new Properties(string.Empty);
            properties["key"] = "\"value\"";
            properties["key"].Should().Be("\"value\"");
            properties.doubleQuoteMode = true;
            properties["key"].Should().Be("value");
        }

        [Fact]
        public void DisablesDoubleQuotesMode()
        {
            var properties = new Properties(string.Empty);
            properties["key"] = "\"value\"";
            properties["key"].Should().Be("\"value\"");
            properties.doubleQuoteMode = true;
            properties.doubleQuoteMode = false;
            properties["key"].Should().Be("\"value\"");
        }

        [Fact]
        public void CreatesPropertiesFromDictionary()
        {
            var input = new Dictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"}
            };

            AssertProperties(
                input,
                "key1 = value1" + "\n" +
                "key2 = value2" + "\n",
                input
            );
        }

        [Fact]
        public void UsesCopyOfOriginalDictionary()
        {
            var input = new Dictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"}
            };

            var p = new Properties(input);
            p["key1"] = "newValue1";

            input["key1"].Should().Be("value1");
            p["key1"].Should().Be("newValue1");
        }

        [Fact]
        public void ReplacesPlaceholderFromDict()
        {
            var input = new Dictionary<string, string>
            {
                {"project.name", "Project"},
                {"project.domain", "com.desperatedevs"},
                {"project.bundleId", "${project.domain}.${project.name}"}
            };

            const string expectedOutput =
                "project.name = Project\n" +
                "project.domain = com.desperatedevs\n" +
                "project.bundleId = ${project.domain}.${project.name}\n";

            var expectedProperties = new Dictionary<string, string>
            {
                {"project.name", "Project"},
                {"project.domain", "com.desperatedevs"},
                {"project.bundleId", "com.desperatedevs.Project"}
            };

            AssertProperties(input, expectedOutput, expectedProperties);
        }

        void AssertProperties(string input, string expectedOutput, Dictionary<string, string> expectedProperties, Properties properties = null)
        {
            var p = properties ?? new Properties(input);
            var expectedCount = expectedProperties?.Count ?? 0;
            p.count.Should().Be(expectedCount);
            p.ToString().Should().Be(expectedOutput);
            if (expectedProperties != null)
            {
                foreach (var kvp in expectedProperties)
                {
                    p.HasKey(kvp.Key).Should().BeTrue();
                    p[kvp.Key].Should().Be(kvp.Value);
                }
            }
        }

        void AssertProperties(Dictionary<string, string> input, string expectedOutput, Dictionary<string, string> expectedProperties)
        {
            var p = new Properties(input);
            var expectedCount = expectedProperties?.Count ?? 0;
            p.count.Should().Be(expectedCount);
            p.ToString().Should().Be(expectedOutput);
            if (expectedProperties != null)
            {
                foreach (var kvp in expectedProperties)
                {
                    p.HasKey(kvp.Key).Should().BeTrue();
                    p[kvp.Key].Should().Be(kvp.Value);
                }
            }
        }
    }
}

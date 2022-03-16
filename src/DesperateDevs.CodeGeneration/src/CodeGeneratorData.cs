using System.Collections.Generic;
using System.Linq;
using DesperateDevs;
using DesperateDevs.Extensions;

namespace DesperateDevs.CodeGeneration {

    public class CodeGeneratorData : Dictionary<string, object> {

        public CodeGeneratorData() {
        }

        public CodeGeneratorData(CodeGeneratorData data) : base(data) {
        }

        public string ReplacePlaceholders(string template) {
            return this.Aggregate(template, (current, kv) => ReplacePlaceholders(current, kv.Key, kv.Value.ToString()));
        }

        public string ReplacePlaceholders(string template, string key, string value) {
            const string placeholderFormat = @"${{{0}}}";

            var upperKey = string.Format(placeholderFormat, key.UppercaseFirst());
            var lowerKey = string.Format(placeholderFormat, key.LowercaseFirst());
            var upperValue = value.UppercaseFirst();
            var lowerValue = value.LowercaseFirst();

            template = template.Replace(upperKey, upperValue);
            template = template.Replace(lowerKey, lowerValue);

            return template;
        }
    }
}

using System;
using System.Linq;
using DesperateDevs.Logging;
using DesperateDevs.Serialization;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    public enum UserDecision {
        Accept,
        Cancel,
        Ignore
    }

    public static class APIUtil {

        static readonly Logger _logger = fabl.GetLogger("Jenny");

        public static string[] GetUnusedKeys(string[] requiredKeys, Preferences preferences) {
            return preferences.keys
                .Where(key => !requiredKeys.Contains(key))
                .ToArray();
        }

        public static string[] GetMissingKeys(string[] requiredKeys, Preferences preferences) {
            return requiredKeys
                .Where(key => !preferences.HasKey(key))
                .ToArray();
        }

        public static bool GetUserDecision(char accept = 'y', char cancel = 'n') {
            char keyChar;
            do {
                keyChar = Console.ReadKey(true).KeyChar;
            } while (keyChar != accept && keyChar != cancel);

            return keyChar == accept;
        }

        public static UserDecision GetUserDecisionOrIgnore(char accept = 'y', char cancel = 'n', char ignore = 'i') {
            char keyChar;
            do {
                keyChar = Console.ReadKey(true).KeyChar;
            } while (keyChar != accept && keyChar != cancel && keyChar != ignore);

            if (keyChar == ignore) {
                return UserDecision.Ignore;
            }

            return keyChar == accept
                ? UserDecision.Accept
                : UserDecision.Cancel;
        }

        public static char GetGenericUserDecision(char[] chars) {
            char keyChar;
            do {
                keyChar = Console.ReadKey(true).KeyChar;
            } while (!chars.Contains(keyChar));

            return keyChar;
        }

        public static void ForceAddKey(string message, string key, string value, Preferences preferences) {
            _logger.Info("ℹ️  " + message + ": '" + key + "' (press any key)");
            Console.ReadKey(true);
            AddKey(key, value, preferences);
        }

        public static void AskAddKey(string question, string key, string value, Preferences preferences) {
            _logger.Info("✅  " + question + ": '" + key + "' ? (y / n)");
            if (GetUserDecision()) {
                AddKey(key, value, preferences);
            }
        }

        public static void AddKey(string key, string value, Preferences preferences) {
            preferences[key] = value;
            preferences.Save();
            _logger.Info("Added: " + key);
        }

        public static void AskAddValue(string question, string value, string[] values, Action<string[]> updateAction, Preferences preferences) {
            _logger.Info("✅  " + question + ": '" + value + "' ? (y / n)");
            if (GetUserDecision()) {
                AddValue(value, values, updateAction, preferences);
            }
        }

        public static void AddValue(string value, string[] values, Action<string[]> updateAction, Preferences preferences) {
            var valueList = values.ToList();
            valueList.Add(value);
            updateAction(valueList.ToArray());
            preferences.Save();
            _logger.Info("Added: " + value);
        }

        public static void AskRemoveKey(string question, string key, Preferences preferences) {
            _logger.Warn("❌  " + question + ": '" + key + "' ? (y / n)");
            if (GetUserDecision()) {
                RemoveKey(key, preferences);
            }
        }

        public static void AskRemoveOrIgnoreKey(string question, string key, CLIConfig cliConfig, Preferences preferences) {
            _logger.Warn("❌  " + question + ": '" + key + "' ? (y / n / (i)gnore)");
            var userDecision = GetUserDecisionOrIgnore();
            if (userDecision == UserDecision.Accept) {
                RemoveKey(key, preferences);
            } else if (userDecision == UserDecision.Ignore) {
                AddValue(
                    key,
                    cliConfig.ignoreUnusedKeys,
                    values => cliConfig.ignoreUnusedKeys = values,
                    preferences);
            }
        }

        public static void RemoveKey(string key, Preferences preferences) {
            preferences.properties.RemoveProperty(key);
            preferences.Save();
            _logger.Warn("Removed: " + key);
        }

        public static void AskRemoveValue(string question, string value, string[] values, Action<string[]> updateAction, Preferences preferences) {
            _logger.Warn("❌  " + question + ": '" + value + "' ? (y / n)");
            if (GetUserDecision()) {
                RemoveValue(value, values, updateAction, preferences);
            }
        }

        public static void RemoveValue(string value, string[] values, Action<string[]> updateAction, Preferences preferences) {
            var valueList = values.ToList();
            if (valueList.Remove(value)) {
                updateAction(valueList.ToArray());
                preferences.Save();
                _logger.Warn("Removed: " + value);
            } else {
                _logger.Warn("Value does not exist: " + value);
            }
        }
    }
}

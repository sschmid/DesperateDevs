using System;
using System.Linq;
using DesperateDevs.Logging;

namespace DesperateDevs.Serialization.CLI.Utils {

    public enum UserDecision {
        Accept,
        Cancel,
        Ignore
    }

    public static class PreferencesExtension {

        static readonly Logger _logger = fabl.GetLogger(typeof(PreferencesExtension));

        public static string[] GetUnusedKeys(this Preferences preferences, string[] requiredKeys) {
            return preferences.keys
                .Where(key => !requiredKeys.Contains(key))
                .ToArray();
        }

        public static string[] GetMissingKeys(this Preferences preferences, string[] requiredKeys) {
            return requiredKeys
                .Where(key => !preferences.HasKey(key))
                .ToArray();
        }

        public static void NotifyForceAddKey(this Preferences preferences, string message, string key, string value) {
            _logger.Info("ℹ️  " + message + ": '" + key + "' (press any key)");
            Console.ReadKey(true);
            AddKey(preferences, key, value);
        }

        public static void AskAddKey(this Preferences preferences, string question, string key, string value) {
            _logger.Info("✅  " + question + ": '" + key + "' ? (y / n)");
            if (GetUserDecision()) {
                AddKey(preferences, key, value);
            }
        }

        public static void AddKey(this Preferences preferences, string key, string value) {
            preferences[key] = value;
            preferences.Save();
            _logger.Info("Added: " + key);
        }

        public static void AskAddValue(this Preferences preferences, string question, string value, string[] values, Action<string[]> updateAction) {
            _logger.Info("✅  " + question + ": '" + value + "' ? (y / n)");
            if (GetUserDecision()) {
                AddValue(preferences, value, values, updateAction);
            }
        }

        public static void AddValue(this Preferences preferences, string value, string[] values, Action<string[]> updateAction) {
            var valueList = values.ToList();
            valueList.Add(value);
            updateAction(valueList.ToArray());
            preferences.Save();
            _logger.Info("Added: " + value);
        }

        public static void AskRemoveKey(this Preferences preferences, string question, string key) {
            _logger.Warn("❌  " + question + ": '" + key + "' ? (y / n)");
            if (GetUserDecision()) {
                RemoveKey(preferences, key);
            }
        }

        public static void AskRemoveOrIgnoreKey(this Preferences preferences, string question, string key, string[] values, Action<string[]> updateAction) {
            _logger.Warn("❌  " + question + ": '" + key + "' ? (y / n / (i)gnore)");
            var userDecision = GetUserDecisionOrIgnore();
            if (userDecision == UserDecision.Accept) {
                RemoveKey(preferences, key);
            } else if (userDecision == UserDecision.Ignore) {
                AddValue(preferences, key, values, updateAction);
            }
        }

        public static void RemoveKey(this Preferences preferences, string key) {
            preferences.properties.RemoveProperty(key);
            preferences.Save();
            _logger.Warn("Removed: " + key);
        }

        public static void AskRemoveValue(this Preferences preferences, string question, string value, string[] values, Action<string[]> updateAction) {
            _logger.Warn("❌  " + question + ": '" + value + "' ? (y / n)");
            if (GetUserDecision()) {
                RemoveValue(preferences, value, values, updateAction);
            }
        }

        public static void RemoveValue(this Preferences preferences, string value, string[] values, Action<string[]> updateAction) {
            var valueList = values.ToList();
            if (valueList.Remove(value)) {
                updateAction(valueList.ToArray());
                preferences.Save();
                _logger.Warn("Removed: " + value);
            } else {
                _logger.Warn("Value does not exist: " + value);
            }
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
    }
}

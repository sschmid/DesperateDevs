using System;
using System.Collections.Generic;
using System.Linq;
using Sherlog;

namespace DesperateDevs.Serialization.Cli.Utils
{
    public static class PreferencesExtension
    {
        static readonly Logger Logger = Logger.GetLogger(typeof(PreferencesExtension));

        public static IEnumerable<string> GetUnusedKeys(this Preferences preferences, IEnumerable<string> requiredKeys) =>
            preferences.Keys.Where(key => !requiredKeys.Contains(key));

        public static IEnumerable<string> GetMissingKeys(this Preferences preferences, IEnumerable<string> requiredKeys) =>
            requiredKeys.Where(key => !preferences.HasKey(key));

        public static void NotifyForceAddKey(this Preferences preferences, string message, string key, string value)
        {
            Console.WriteLine($"ℹ️  {message}: '{key}' (press any key)");
            Console.ReadKey(true);
            AddKey(preferences, key, value);
        }

        public static void AskAddKey(this Preferences preferences, string question, string key, string value)
        {
            Console.WriteLine($"✅  {question}: '{key}' ? (y / n)");
            if (GetUserDecision())
                AddKey(preferences, key, value);
        }

        public static void AddKey(this Preferences preferences, string key, string value)
        {
            preferences[key] = value;
            preferences.Save();
            Logger.Info($"Added: {key}");
        }

        public static void AskAddValue(this Preferences preferences, string question, string value, IEnumerable<string> values, Action<IEnumerable<string>> updateAction)
        {
            Console.WriteLine($"✅  {question}: '{value}' ? (y / n)");
            if (GetUserDecision())
                AddValue(preferences, value, values, updateAction);
        }

        public static void AddValue(this Preferences preferences, string value, IEnumerable<string> values, Action<IEnumerable<string>> updateAction)
        {
            var list = values.ToList();
            list.Add(value);
            updateAction(list);
            preferences.Save();
            Logger.Info($"Added: {value}");
        }

        public static void AskRemoveKey(this Preferences preferences, string question, string key)
        {
            Console.WriteLine($"❌  {question}: '{key}' ? (y / n)");
            if (GetUserDecision())
                RemoveKey(preferences, key);
        }

        public static void RemoveKey(this Preferences preferences, string key)
        {
            preferences.Properties.RemoveProperty(key);
            preferences.Save();
            Logger.Warn($"Removed: {key}");
        }

        public static void AskRemoveValue(this Preferences preferences, string question, string value, IEnumerable<string> values, Action<IEnumerable<string>> updateAction)
        {
            Console.WriteLine($"❌  {question}: '{value}' ? (y / n)");
            if (GetUserDecision())
                RemoveValue(preferences, value, values, updateAction);
        }

        public static void RemoveValue(this Preferences preferences, string value, IEnumerable<string> values, Action<IEnumerable<string>> updateAction)
        {
            var list = values.ToList();
            if (list.Remove(value))
            {
                updateAction(list);
                preferences.Save();
                Logger.Warn($"Removed: {value}");
            }
            else
            {
                Logger.Warn($"Value does not exist: {value}");
            }
        }

        public static bool GetUserDecision(char accept = 'y', char cancel = 'n')
        {
            char keyChar;
            do
            {
                keyChar = Console.ReadKey(true).KeyChar;
            } while (keyChar != accept && keyChar != cancel);

            return keyChar == accept;
        }

        public static char GetGenericUserDecision(char[] chars)
        {
            char keyChar;
            do
            {
                keyChar = Console.ReadKey(true).KeyChar;
            } while (!chars.Contains(keyChar));

            return keyChar;
        }
    }
}

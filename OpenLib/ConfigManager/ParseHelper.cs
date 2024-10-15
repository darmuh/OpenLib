using BepInEx;
using System;
using System.Collections.Generic;

namespace OpenLib.ConfigManager
{
    public class ParseHelper
    {
        public static List<KeyValuePair<string, string>> ParseJsonManually(string jsonString)
        {
            // Assuming a very simple JSON format for demonstration
            // {"key1":"value1","key2":"value2"}
            var keyValuePairs = jsonString.Trim(new char[] { '{', '}' }).Split(',');
            List<KeyValuePair<string, string>> returnValue = [];

            foreach (var pair in keyValuePairs)
            {
                var keyValue = pair.Split(':');
                if (keyValue.Length == 2)
                {
                    string key = keyValue[0].Trim(new char[] { '\"' }).Trim();
                    string value = keyValue[1].Trim(new char[] { '\"' }).Trim();
                    KeyValuePair<string, string> foundPair = new(key, value);
                    returnValue.Add(foundPair);
                }
            }

            return returnValue;
        }

        public static Dictionary<string, string> ParseKeyValuePairs(string data)
        {
            var dictionary = new Dictionary<string, string>();
            if (data.IsNullOrWhiteSpace())
                return dictionary;

            // Split by comma to get key-value pairs
            var pairs = data.Split(new[] { ";:;" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var pair in pairs)
            {
                var keyValue = pair.Split(':');

                if (keyValue.Length == 2)
                {
                    var key = keyValue[0].Trim();
                    var value = keyValue[1].Trim();

                    // Add to dictionary
                    dictionary[key] = value;
                }
                else
                {
                    Plugin.WARNING($"Invalid pair format: {pair}");
                }
            }

            return dictionary;
        }
    }
}

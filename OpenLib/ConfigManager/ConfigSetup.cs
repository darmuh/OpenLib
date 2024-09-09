using BepInEx.Configuration;
using OpenLib.CoreMethods;
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using OpenLib.Common;
using BepInEx;
using System.IO.Compression;
using System.Text;
using LobbyCompatibility.Configuration;

namespace OpenLib.ConfigManager
{
    public static class ConfigSetup
    {
        public static List<ManagedConfig> defaultManaged = [];
        public static MainListing defaultListing;
        public static ConfigEntry<bool> ExtensiveLogging { get; internal set; }
        public static ConfigEntry<bool> DeveloperLogging { get; internal set; }

        public static void BindConfigSettings()
        {
            Plugin.Log.LogInfo("Binding configuration settings");

            ExtensiveLogging = MakeBool(Plugin.instance.Config, "Debug", "ExtensiveLogging", false, "Enable or Disable extensive logging for this mod.");
            DeveloperLogging = MakeBool(Plugin.instance.Config, "Debug", "DeveloperLogging", false, "Enable or Disable developer logging for this mod. (this will fill your log file FAST)");

            //ReadConfigAndAssignValues(Plugin.instance.Config, managedItems);
        }

        public static ManagedConfig AddManagedBool(ConfigEntry<bool> boolEntry, List<ManagedConfig> managedItems, bool isNetworked = false, string category = "", string configString = "", Func<string> mainAction = null, int commandType = 0, bool clearText = true, Func<string> confirmAction = null, Func<string> denyAction = null, string confirmText = "confirm", string denyText = "deny", string special = "", int specialNum = -1, string nodeName = "", string itemList = "", int price = 0, string storeName = "", bool alwaysInStock = true, int maxStock = 0, bool reuseFunc = false)
        {
            List<string> keywordList = CommonStringStuff.GetKeywordsPerConfigItem(configString);

            if (ManagedBoolGet.TryGetItemByName(managedItems, boolEntry.Definition.Key, 0, out ManagedConfig resultBool))
            {
                resultBool.SetManagedBoolValues(boolEntry.Definition.Key, boolEntry.Value, boolEntry.Description.Description, isNetworked, category, keywordList, mainAction, commandType, clearText, confirmAction, denyAction, confirmText, denyText, special, specialNum, nodeName, itemList, price, storeName, alwaysInStock, maxStock, reuseFunc);

                return resultBool;
            }
            else
            {
                ManagedConfig managedBool = new();
                managedBool.SetManagedBoolValues(boolEntry.Definition.Key, boolEntry.Value, boolEntry.Description.Description, isNetworked, category, keywordList, mainAction, commandType, clearText, confirmAction, denyAction, confirmText, denyText, special, specialNum, nodeName, itemList, price, storeName, alwaysInStock, maxStock, reuseFunc);

                managedItems.Add(managedBool);

                return managedBool;
            }
        }

        public static ManagedConfig AddManagedBool(ConfigEntry<bool> boolEntry, List<ManagedConfig> managedItems, bool isNetworked = false, string category = "", ConfigEntry<string> configString = null, Func<string> mainAction = null, int commandType = 0, bool clearText = true,  Func<string> confirmAction = null, Func<string> denyAction = null, string confirmText = "confirm", string denyText = "deny", string special = "", int specialNum = -1, string nodeName = "", string itemList = "", int price = 0, string storeName = "", bool alwaysInStock = true, int maxStock = 0, bool reuseFunc = false)
        {
            List<string> keywordList = [];
            bool isStringNull = true;
            if (configString != null)
            {
                isStringNull = false;
                keywordList = CommonStringStuff.GetKeywordsPerConfigItem(configString.Value);
            }

            if (ManagedBoolGet.TryGetItemByName(managedItems, boolEntry.Definition.Key, 0, out ManagedConfig resultBool))
            {
                resultBool.SetManagedBoolValues(boolEntry.Definition.Key, boolEntry.Value, boolEntry.Description.Description, isNetworked, category, keywordList, mainAction, commandType, clearText, confirmAction, denyAction, confirmText, denyText, special, specialNum, nodeName, itemList, price, storeName, alwaysInStock, maxStock, reuseFunc);

                if (!isStringNull)
                {
                    AddManagedString(configString, ref managedItems, resultBool);
                }
                return resultBool;
            }
            else
            {
                ManagedConfig managedBool = new();
                managedBool.SetManagedBoolValues(boolEntry.Definition.Key, boolEntry.Value, boolEntry.Description.Description, isNetworked, category, keywordList, mainAction, commandType, clearText, confirmAction, denyAction, confirmText, denyText, special, specialNum, nodeName, itemList, price, storeName, alwaysInStock, maxStock, reuseFunc);


                managedItems.Add(managedBool);
                
                if (!isStringNull)
                {
                    AddManagedString(configString, ref managedItems, managedBool);
                }

                return managedBool;
            }
        }

        public static ManagedConfig NewManagedBool(ref List<ManagedConfig> managedItems, string configItemName, bool isEnabled, string configDescription, bool isNetworked = false, string category = "", List<string> keywordList = null, Func<string> mainAction = null, int commandType = 0, bool clearText = true, Func<string> confirmAction = null, Func<string> denyAction = null, string confirmText = "confirm", string denyText = "deny", string special = "", int specialNum = -1, string nodeName = "", string itemList = "", int price = 0, string storeName = "", bool alwaysInStock = true, int maxStock = 0, bool reuseFunc = false)
        {
            if(ManagedBoolGet.TryGetItemByName(managedItems, configItemName, 0, out ManagedConfig resultBool))
            {
                resultBool.SetManagedBoolValues(configItemName, isEnabled, configDescription, isNetworked, category, keywordList, mainAction, commandType, clearText, confirmAction, denyAction, confirmText, denyText, special, specialNum, nodeName, itemList, price, storeName, alwaysInStock, maxStock, reuseFunc);
                return resultBool;
            }
            else
            {
                ManagedConfig managedBool = new();
                managedBool.SetManagedBoolValues(configItemName, isEnabled, configDescription, isNetworked, category, keywordList, mainAction, commandType, clearText, confirmAction, denyAction, confirmText, denyText, special, specialNum, nodeName, itemList, price, storeName, alwaysInStock, maxStock, reuseFunc);

                managedItems.Add(managedBool);
                return managedBool;
            } 
        }

        public static ConfigEntry<bool> MakeBool(ConfigFile ModConfig, string section, string configItemName, bool defaultValue, string configDescription)
        {
            return ModConfig.Bind<bool>(section, configItemName, defaultValue, configDescription);
        }

        public static ConfigEntry<int> MakeInt(ConfigFile ModConfig, string section, string configItemName, int defaultValue, string configDescription)
        {
            return ModConfig.Bind<int>(section, configItemName, defaultValue, configDescription);
        }

        public static ConfigEntry<string> MakeClampedString(ConfigFile ModConfig, string section, string configItemName, string defaultValue, string configDescription, AcceptableValueList<string> acceptedValues)
        {
            return ModConfig.Bind(section, configItemName, defaultValue, new ConfigDescription(configDescription, acceptedValues));
        }

        public static ConfigEntry<int> MakeClampedInt(ConfigFile ModConfig, string section, string configItemName, int defaultValue, string configDescription, int minValue, int maxValue)
        {
            return ModConfig.Bind(section, configItemName, defaultValue, new ConfigDescription(configDescription, new AcceptableValueRange<int>(minValue, maxValue)));
        }

        public static ConfigEntry<float> MakeClampedFloat(ConfigFile ModConfig, string section, string configItemName, float defaultValue, string configDescription, float minValue, float maxValue)
        {
            return ModConfig.Bind(section, configItemName, defaultValue, new ConfigDescription(configDescription, new AcceptableValueRange<float>(minValue, maxValue)));
        }

        public static ConfigEntry<string> MakeString(ConfigFile ModConfig, string section, string configItemName, string defaultValue, string configDescription)
        {
            return ModConfig.Bind(section, configItemName, defaultValue, configDescription);
        }

        public static void AddManagedString(ConfigEntry<String> configItem, ref List<ManagedConfig> managedItems, ManagedConfig relatedConfigItem)
        {
            ManagedConfig managedString = new()
            {
                ConfigItemName = configItem.Definition.Key,
                configDescription = configItem.Description.Description,
                StringValue = configItem.Value,
                relatedConfigItem = relatedConfigItem,
                ConfigType = 1
            };

            managedItems.Add(managedString);
        }

        public static void RemoveOrphanedEntries(ConfigFile ModConfig)
        {
            Plugin.MoreLogs("removing orphaned entries (credits to Kittenji)");
            PropertyInfo orphanedEntriesProp = ModConfig.GetType().GetProperty("OrphanedEntries", BindingFlags.NonPublic | BindingFlags.Instance);

            var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(ModConfig, null);

            orphanedEntries.Clear(); // Clear orphaned entries (Unbinded/Abandoned entries)
            ModConfig.Save(); // Save the config file
        }

        public static void WebConfig(ConfigFile ModConfig)
        {
            List<string> lines = [];
            string configName = ModConfig.ConfigFilePath.Substring(ModConfig.ConfigFilePath.LastIndexOf('\\'));
            lines.Add($"<html><title>{configName.Replace("\\", "")} Generator</title><body class=\"body\">");
            
            //css style, dont bother editing this in C# just cut/paste
            lines.Add("<style>\r\n  .body {\r\n  background-image: linear-gradient(to bottom right,#2c2b2b, #0a0a0a);\r\n  color:whitesmoke;\r\n  font: Monospace;\r\n  padding-top: 1px;\r\n  padding-right: 1px;\r\n  padding-bottom: 1px;\r\n  padding-left: 0px;\r\n  }\r\n  .slider {\r\n    margin-top: 3px;\r\n  }\r\n  .numberInput {\r\n  background: transparent;\r\n  color: white;\r\n  text-align: center;\r\n  font-weight: bold;\r\n  font-size: 12px;\r\n  border: 0px solid #ccc;\r\n  border-radius: 4px;\r\n  vertical-align: super;\r\n  }\r\n .checkbox{\r\n    margin-right: 2px;\r\n    margin-bottom: 4px;\r\n    display: inline-block;\r\n    margin-left: 0px;\r\n  }\r\ntextarea {\r\n  background-image: linear-gradient(to bottom right, #1B231A, #0a0a0a);\r\n  color: white;\r\n  width: 80%; \r\n  height: 60px;\r\n  }\r\n  .stringInput {\r\n  background: #EDEFED;\r\n  color: #171817;\r\n  text-align: left;\r\n  font-size: 12px;\r\n  border: 0px solid #ccc;\r\n  border-radius: 4px;\r\n  width: 40%;\r\n  padding: 2px;\r\n  margin-top: 4px;\r\n  }\r\n\r\n</style>");

            lines.Add("<form id=\"configForm\">");

            Dictionary<ConfigDefinition, ConfigEntryBase> configItems = new Dictionary<ConfigDefinition, ConfigEntryBase>();
            foreach (ConfigEntryBase value in ModConfig.GetConfigEntries())
            {
                configItems.Add(value.Definition, value);
                Plugin.Spam($"added {value.Definition} to list of configItems to check");
            }

            
            string lastSection = "";

            foreach (KeyValuePair<ConfigDefinition, ConfigEntryBase> pair in configItems)
            {

                if (pair.Key.Section != lastSection)
                {
                    if (lastSection != "")
                        lines.Add($"</fieldset>");
                    lines.Add($"<fieldset>\r\n<legend>{pair.Key.Section}</legend>");
                    lastSection = pair.Key.Section;
                }

                if (pair.Value.BoxedValue.GetType() == typeof(bool))
                {
                    Plugin.Spam($"bool config detected - {pair.Key.Key}");
                    if ((bool)pair.Value.DefaultValue)
                    {
                        Plugin.Spam("default is TRUE");
                        lines.Add($"<p><input name=\"{pair.Key.Key}\" class=\"checkbox\" checked=\"checked\" type=\"checkbox\"/> <label for=\"{pair.Key.Key}\">{pair.Key.Key}</label><br>{pair.Value.Description.Description}<br></p>");
                    }
                    else
                    {
                        Plugin.Spam("default is FALSE");
                        lines.Add($"<p><input name=\"{pair.Key.Key}\" class=\"checkbox\" type=\"checkbox\"/> <label for=\"{pair.Key.Key}\">{pair.Key.Key}</label><br>{pair.Value.Description.Description}<br></p>");
                    }
                    
                    
                }
                else if (pair.Value.BoxedValue.GetType() == typeof(string))
                {
                    if(pair.Value.Description.AcceptableValues != null)
                    {
                        lines.Add($"<p>{pair.Key.Key}<br>{pair.Value.Description.Description}<br />");
                        List<string> acceptableValues = ConfigHelper.GetAcceptableValues(pair.Value.Description.AcceptableValues);
                        int num = 1;
                        foreach(string value in acceptableValues)
                        {
                            string ToHtml = "";
                            if (value == (string)pair.Value.DefaultValue)
                                ToHtml = WebHelper.AddValueToHTMLCode(value, pair.Key.Key, num, true);
                            else
                                ToHtml = WebHelper.AddValueToHTMLCode(value, pair.Key.Key, num, false);

                            lines.Add(ToHtml);
                            num++;
                        }
                        lines.Add("</p>");
                        Plugin.Spam($"clamped string config detected - {pair.Key.Key}");
                    }
                    else
                    {
                        lines.Add($"<p><label for=\"{pair.Key.Key}\">{pair.Key.Key}</label><br>{pair.Value.Description.Description}<br /><input name=\"{pair.Key.Key}\" type=\"text\" class=\"stringInput\" value=\"{pair.Value.DefaultValue}\" /><br /></p>");
                        Plugin.Spam($"string config detected - {pair.Key.Key}");
                    }
                    
                }
                else if (pair.Value.BoxedValue.GetType() == typeof(int) || pair.Value.BoxedValue.GetType() == typeof(float))
                {
                    if (pair.Value.Description.AcceptableValues != null)
                    {
                        lines.Add($"<p>{pair.Key.Key}<br>{pair.Value.Description.Description}<br />");
                        List<float> acceptableValues = ConfigHelper.GetAcceptableValueF(pair.Value.Description.AcceptableValues);
                        lines.Add(WebHelper.AddValueToHTMLCode(acceptableValues, pair.Key.Key, pair.Value.DefaultValue.ToString()));
                        lines.Add("</p>");
                        Plugin.Spam($"clamped number-type config detected - {pair.Key.Key}");
                    }
                    else
                    {
                        lines.Add($"<p><label for=\"{pair.Key.Key}\">{pair.Key.Key}</label><br>{pair.Value.Description.Description}<br /><input name=\"{pair.Key.Key}\" type=\"number\" class=\"numberInput\" value=\"{pair.Value.DefaultValue}\" /><br /></p>");
                        Plugin.Spam($"number-type config detected - {pair.Key.Key}");
                    }
                }
            }
            lines.Add($"</fieldset><br /></form>");

            // Add the compression script
            lines.Add(@"<script src=""https://cdnjs.cloudflare.com/ajax/libs/pako/2.1.0/pako.min.js""></script>
	<script>
        function serializeForm() {
            const form = document.getElementById('configForm');
            const elements = form.elements;
            let result = [];

            for (let element of elements) {
                if (element.name) {
                    if (element.type === 'radio') {
                        if (element.checked) {
                            result.push(`${element.name}:${element.value}`);
                        }
                    } else if(element.type === 'checkbox') {
                        if (element.checked) {
                            result.push(`${element.name}:true`);
                        } else {
                            result.push(`${element.name}:false`);
                        }
                    } else {
                        result.push(`${element.name}:${element.value}`);
                    }
                }
            }
			
			const compressedData = compressData(result.join('~ '));
            document.getElementById('rawData').textContent = result.join('~ ');
            document.getElementById('compressedData').textContent = compressedData;
        }

        function clearText() {
        document.getElementById('rawData').textContent = '';
        document.getElementById('compressedData').textContent = '';
		
        }

		function compressData(data) {

		// Convert query string to a Uint8Array
		const uint8Array = new TextEncoder().encode(data);

		// Compress using pako
		const compressed = pako.gzip(uint8Array);

		// Convert compressed data to Base64
		return btoa(String.fromCharCode(...new Uint8Array(compressed)));
		}
    </script>");

            lines.Add("<br /><center><button type='button' onclick='serializeForm()'>Get Form Code</button> " +
                "<button type='button' onclick='clearText()'>Clear Results</button><br>");
            lines.Add("<br>Raw data:<br><textarea id='rawData' readonly=true></textarea><br>" +
                "<br>Code:<br><textarea id='compressedData' readonly=true></textarea></center>");

            lines.Add("</body></html>");
            if (!Directory.Exists($"{Paths.ConfigPath}/webconfig"))
                Directory.CreateDirectory($"{Paths.ConfigPath}/webconfig");
            File.WriteAllLines($"{Paths.ConfigPath}/webconfig/{configName}_generator.htm", lines);
        }

        static string DecompressBase64Gzip(string base64)
        {
            byte[] gzipBytes = Convert.FromBase64String(base64);

            using (var compressedStream = new MemoryStream(gzipBytes))
            using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var reader = new StreamReader(gzipStream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        public static void ReadCompressedConfig(ref ConfigEntry<string> configEntry, ConfigFile ModConfig)
        {
            string compressedDataBase64 = configEntry.Value;
            string jsonString = DecompressBase64Gzip(compressedDataBase64);

            Dictionary<string,string> fromString = ParseHelper.ParseKeyValuePairs(jsonString);

            if (fromString.Count == 0)
                return;

            foreach(KeyValuePair<string, string> pair in fromString)
            {
                if(TryFindConfigItem(pair.Key, ModConfig, out ConfigEntryBase configItem))
                {
                    if(configItem.BoxedValue.GetType() == typeof(bool))
                    {
                        ConfigHelper.ChangeBool(ModConfig, configItem, pair.Value.ToLower());
                    }
                    else if(configItem.BoxedValue.GetType() == typeof(string))
                    {
                        ConfigHelper.ChangeString(ModConfig, configItem, pair.Value);
                    }
                    else if (configItem.BoxedValue.GetType() == typeof(int))
                    {
                        ConfigHelper.ChangeInt(ModConfig, configItem, pair.Value);
                    }
                    else if (configItem.BoxedValue.GetType() == typeof(float))
                    {
                        ConfigHelper.ChangeFloat(ModConfig, configItem, pair.Value);
                    }
                }
            }

            configEntry.Value = "";
        }

        public static bool TryFindConfigItem(string query, ConfigFile ModConfig, out ConfigEntryBase configItem)
        {
            Dictionary<ConfigDefinition, ConfigEntryBase> configItems = [];
            foreach (ConfigEntryBase value in ModConfig.GetConfigEntries())
            {
                configItems.Add(value.Definition, value);
                Plugin.Spam($"added {value.Definition} to list of configItems to check");
            }
            foreach (KeyValuePair<ConfigDefinition, ConfigEntryBase> pair in configItems)
            {
                if(pair.Key.Key == query)
                {
                    configItem = pair.Value;
                    return true;
                }

            } 
            configItem = null!;
            return false;
        }


        public static void NetworkingCheck(bool NetworkConfigOption, ConfigFile ModConfig, List<ManagedConfig> managedBools)
        {
            Plugin.Log.LogInfo("Checking if networking is disabled...");

            if (NetworkConfigOption)
                return;

            List<ConfigEntry<bool>> configBools = [];

            Dictionary<ConfigDefinition, ConfigEntryBase> configItems = [];
            foreach (ConfigEntryBase value in ModConfig.GetConfigEntries())
            {
                configItems.Add(value.Definition, value);
                Plugin.Spam($"added {value.Definition} to list of configItems to check");
            }

            foreach (KeyValuePair<ConfigDefinition, ConfigEntryBase> pair in configItems)
            {
                if (pair.Value.BoxedValue.GetType() == typeof(bool))
                {
                    if (ModConfig.TryGetEntry<bool>(pair.Key, out ConfigEntry<bool> entry))
                    {
                        if (ManagedBoolGet.TryGetItemByName(managedBools, pair.Key.Key, 0, out ManagedConfig result))
                        {
                            if (result.ConfigType != 0)
                                Plugin.Spam("ManagedItem is type 0, bool");

                            if (result.RequiresNetworking)
                            {
                                configBools.Add(entry);
                                Plugin.Spam($"Adding {pair.Key.Key} to bools list to check against networking");
                            }
                            else
                                Plugin.Spam($"{pair.Key.Key} is not listed as requiring networking");
                        }
                        else
                            Plugin.Spam($"entry is not a managed bool");
                    }
                }
                else
                    Plugin.Spam($"entry is not a bool");
            }

            foreach (ConfigEntry<bool> configItem in configBools)
            {
                if (configItem.Value)
                {
                    configItem.Value = false;
                    Plugin.Log.LogWarning($"Setting {configItem.Definition.Key} to false. Networking is disabled and this setting requires networking!");
                }
            }

            ModConfig.Save(); // Save the config file
        }

        public static void ReadConfigAndAssignValues(ConfigFile ModConfig, List<ManagedConfig> managedBools) //good for config reload events
        {
            Plugin.Log.LogInfo("attempting to read config and assign values");
            //List<ConfigDefinition> configKeys = [.. ModConfig.Keys];

            Dictionary<ConfigDefinition, ConfigEntryBase> configItems = [];
            foreach (ConfigEntryBase value in ModConfig.GetConfigEntries())
            {
                configItems.Add(value.Definition, value);
                Plugin.Spam($"added {value.Definition} to list of configItems to check");
            }

            foreach (KeyValuePair<ConfigDefinition, ConfigEntryBase> pair in configItems)
            {
                Plugin.Spam("checking item");
                if (pair.Value.BoxedValue.GetType() == typeof(bool))
                {
                    if (ModConfig.TryGetEntry<bool>(pair.Key, out ConfigEntry<bool> entry))
                    {
                        Plugin.Spam("bool entry found");
                        Plugin.Spam($"{entry.Definition.Key}");
                        if (ManagedBoolGet.TryGetItemByName(managedBools, entry.Definition.Key, 0, out ManagedConfig match))
                        {
                            match.BoolValue = entry.Value;
                            Plugin.Spam($"Assigned ManagedConfig: {match.ConfigItemName} to configValue: {entry.Value}");
                        }
                        else
                            Plugin.Log.LogWarning($"Could not find ManagedConfig for {pair.Key.Key}");
                    }
                }
                else if ((pair.Value.BoxedValue.GetType() == typeof(string)))
                {
                    if (ModConfig.TryGetEntry<string>(pair.Key, out ConfigEntry<string> entry))
                    {
                        Plugin.Spam("string entry found");
                        Plugin.Spam($"{entry.Definition.Key}");
                        if (ManagedBoolGet.TryGetItemByName(managedBools, entry.Definition.Key, 1, out ManagedConfig match))
                        {
                            match.StringValue = entry.Value;
                            Plugin.Spam($"Assigned ManagedConfig: {match.ConfigItemName} to configValue: {entry.Value}");
                        }
                        else
                            Plugin.Log.LogWarning($"Could not find ManagedConfig for {pair.Key.Key}");
                    }
                }
                else
                    Plugin.Log.LogWarning($"Unable to read configItem {pair.Key.Key} and match to Managed Config Item");
            }
        }
    }
}
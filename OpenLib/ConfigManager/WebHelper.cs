using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using static OpenLib.ConfigManager.ConfigHelper;


namespace OpenLib.ConfigManager
{
    public class WebHelper
    {
        public static string AddValueToHTMLCode(string value, string configName, int num, bool isDefaultValue)
        {

            if (isDefaultValue)
            {
                Plugin.Spam($"{configName} is default value, setting this to checked");
                return $"<input type=\"radio\" id=\"{configName}{num}\" checked=\"checked\" name=\"{configName}\" value=\"{value}\">\r\n            <label for=\"{configName}{num}\">{value}</label><br>";
            }

            return $"<input type=\"radio\" id=\"{configName}{num}\" name=\"{configName}\" value=\"{value}\">\r\n            <label for=\"{configName}{num}\">{value}</label><br>";
        }

        public static string AddValueToHTMLCode(List<float> values, string configName, string defaultValue, int sliderItem)
        {

            if (values.Count != 2)
                return "";

            configName = Common.CommonStringStuff.RemovePunctuation(configName).Replace(" ", "");

            if (values[1] > 999)
            {
                Plugin.Spam($"clamped number-type max value too high for slider - {values[1]}");
                return $"<input name=\"{configName}\" type=\"number\" onkeypress=\"return /[0-9.]/i.test(event.key)\" class=\"stringInput\" min=\"{values[0]}\" max=\"{values[1]}\" value=\"{defaultValue}\" />";
            }

            if (values[0].ToString().Contains(".") || values[1] <= 1)
                return $"<input type=\"range\" name=\"{configName}\" id=\"number_slider{sliderItem}\" value=\"{defaultValue}\" step=\"0.001\" min=\"{values[0]}\" max=\"{values[1]}\" oninput=\"number_text{sliderItem}.value = number_slider{sliderItem}.value\">\r\n" +
                    $"<input id=\"number_text{sliderItem}\" type=\"text\" class=\"numberInput\" size=\"4\" min=\"{values[0]}\" max=\"{values[1]}\" value=\"{defaultValue}\" onkeypress=\"return /[0-9.]/i.test(event.key)\" oninput=\"number_slider{sliderItem}.value = number_text{sliderItem}.value\">";



            return $"<input type=\"range\" name=\"{configName}\" id=\"number_slider{sliderItem}\" value=\"{defaultValue}\" min=\"{values[0]}\" max=\"{values[1]}\" oninput=\"number_text{sliderItem}.value = number_slider{sliderItem}.value\">\r\n" +
                $"<input id=\"number_text{sliderItem}\" type=\"text\" class=\"numberInput\" size=\"4\" min=\"{values[0]}\" max=\"{values[1]}\" value=\"{defaultValue}\" onkeypress=\"return /[0-9]/i.test(event.key)\" oninput=\"number_slider{sliderItem}.value = number_text{sliderItem}.value\">";
        }

        public static void WebConfig(ConfigFile ModConfig)
        {
            List<string> lines = [];
            int colorItem = 0;
            int sliderItem = 0;
            string configName = ModConfig.ConfigFilePath.Substring(ModConfig.ConfigFilePath.LastIndexOf('\\'));
            lines.Add($"<html><title>{configName.Replace("\\", "")} Generator</title><body class=\"body\">");

            //css style, dont bother editing this in C# just cut/paste
            lines.Add("<style>\r\n    .body {\r\n        background-image: linear-gradient(to bottom right,#1a1919, #0a0a0a);\r\n        color: whitesmoke;\r\n        font: Monospace;\r\n        padding-top: 1px;\r\n        padding-right: 1px;\r\n        padding-bottom: 1px;\r\n        padding-left: 0px;\r\n        margin-left: 20%;\r\n        margin-right: 20%;\r\n    }\r\n\r\n    .slider {\r\n        margin-top: 3%;\r\n    }\r\n\r\n    .slider:focus {\r\n        outline: none;\r\n        box-shadow: 0 0 10px #fafff9;\r\n    }\r\n\r\n    .numberInput {\r\n        background: transparent;\r\n        color: white;\r\n        text-align: center;\r\n        font-weight: bold;\r\n        font-size: 12px;\r\n        border: 1px solid #ccc;\r\n        border-radius: 4%;\r\n        vertical-align: super;\r\n    }\r\n\r\n    .checkbox {\r\n        margin-right: 0%;\r\n        margin-bottom: 1%;\r\n        display: inline-block;\r\n        margin-left: 0%;\r\n    }\r\n\r\n    .checkbox:focus {\r\n        outline: none;\r\n        box-shadow: 0 0 10px #fafff9;\r\n    }\r\n\r\n    textarea {\r\n        background-image: linear-gradient(to bottom right, #1B231A, #0a0a0a);\r\n        color: #C6A97C;\r\n        width: 80%;\r\n        height: 60px;\r\n        resize: vertical;\r\n    }\r\n\r\n    .stringInput {\r\n        background: #EDEFED;\r\n        background-color: #C6A97C;\r\n        text-align: left;\r\n        font-size: 12px;\r\n        border: 0px solid #ccc;\r\n        border-radius: 4%;\r\n        width: 40%;\r\n        padding: 2px;\r\n        margin-top: 1%;\r\n    }\r\n\r\n    .colorText {\r\n        width: 10%;\r\n        border: 1px solid #ddd;\r\n        border-radius: 5%;\r\n        box-shadow: 0 0 0px #ddd;\r\n    }\r\n\r\n    input[type=color] {\r\n        border-width: 0px;\r\n        border: none;\r\n        background: none;\r\n        height: 24px;\r\n        width: 24px;\r\n        vertical-align: sub;\r\n        margin: 0;\r\n        -webkit-appearance: none;\r\n    }\r\n\r\n    input[type=color]:focus {\r\n        box-shadow: 0 0 10px #fafff9;\r\n        border-width: 0px;\r\n        border: 1px solid #fafff9;\r\n        outline: none;\r\n    }\r\n    input[type=text]:focus {\r\n        box-shadow: 0 0 10px #fafff9;\r\n        border-color: #ddd;\r\n        outline: none;\r\n    }\r\n</style>");

            lines.Add($"<h1><center>{configName.Replace("\\", "")} Generator</h1></center><center><p>Upload your config:<br><input type=\"file\" id=\"fileInput\" accept=\".cfg\"> <button type=\"button\" onclick=\"loadFileAsText()\"> Submit Config</button></p></center>");

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
                        lines.Add($"<p><input id=\"{pair.Key.Key}\" name=\"{pair.Key.Key}\" class=\"checkbox\" checked=\"checked\" type=\"checkbox\"/> <label for=\"{pair.Key.Key}\">{pair.Key.Key}</label><br>{pair.Value.Description.Description}<br></p>");
                    }
                    else
                    {
                        Plugin.Spam("default is FALSE");
                        lines.Add($"<p><input id=\"{pair.Key.Key}\" name=\"{pair.Key.Key}\" class=\"checkbox\" type=\"checkbox\"/> <label for=\"{pair.Key.Key}\">{pair.Key.Key}</label><br>{pair.Value.Description.Description}<br></p>");
                    }


                }
                else if (pair.Value.BoxedValue.GetType() == typeof(string))
                {
                    if (pair.Value.Description.AcceptableValues != null)
                    {
                        lines.Add($"<p>{pair.Key.Key}<br>{pair.Value.Description.Description}<br />");
                        List<string> acceptableValues = GetAcceptableValues(pair.Value.Description.AcceptableValues);
                        int num = 1;
                        foreach (string value in acceptableValues)
                        {
                            string ToHtml = "";
                            if (value == (string)pair.Value.DefaultValue)
                                ToHtml = AddValueToHTMLCode(value, pair.Key.Key, num, true);
                            else
                                ToHtml = AddValueToHTMLCode(value, pair.Key.Key, num, false);

                            lines.Add(ToHtml);
                            num++;
                        }
                        lines.Add("</p>");
                        Plugin.Spam($"clamped string config detected - {pair.Key.Key}");
                    }
                    else
                    {
                        string defaultValue = pair.Value.DefaultValue as string;
                        if (defaultValue.StartsWith("#") && defaultValue.Length == 7)
                        {
                            lines.Add($"<p><label for=\"{pair.Key.Key}\">{pair.Key.Key}</label><br><input type=\"color\" id=\"color{colorItem}\" name=\"{pair.Key.Key}\" value=\"{defaultValue}\" oninput=\"color{colorItem}_text.value = color{colorItem}.value\"> " +
                                $"<input type=\"text\" class=\"colorText\" name=\"{pair.Key.Key}_text\" id=\"color{colorItem}_text\" value=\"{defaultValue}\" onkeypress=\"return /[0-9a-zA-Z#]/i.test(event.key)\" pattern=\"#[0-9a-fA-F]{{3}}([0-9a-fA-F]{{3}})?\" oninput=\"color{colorItem}.value = color{colorItem}_text.value\"></p>");
                            colorItem++;
                        }
                        else
                        {
                            lines.Add($"<p><label for=\"{pair.Key.Key}\">{pair.Key.Key}</label><br>{pair.Value.Description.Description}<br /><input id=\"{pair.Key.Key}\" name=\"{pair.Key.Key}\" type=\"text\" class=\"stringInput\" value=\"{pair.Value.DefaultValue}\" /><br /></p>");
                        }
                        Plugin.Spam($"string config detected - {pair.Key.Key}");
                    }

                }
                else if (pair.Value.BoxedValue.GetType() == typeof(int) || pair.Value.BoxedValue.GetType() == typeof(float))
                {
                    if (pair.Value.Description.AcceptableValues != null)
                    {
                        lines.Add($"<p>{pair.Key.Key}<br>{pair.Value.Description.Description}<br />");
                        List<float> acceptableValues = GetAcceptableValueF(pair.Value.Description.AcceptableValues);
                        lines.Add(AddValueToHTMLCode(acceptableValues, pair.Key.Key, pair.Value.DefaultValue.ToString(), sliderItem));
                        sliderItem++;
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

    const compressedData = compressData(result.join(';:; '));
    document.getElementById('rawData').textContent = result.join(';:; ');
    document.getElementById('compressedData').textContent = compressedData;
}

//MinCodes = 4
function parseConfig(text) {
    const lines = text.split('\n');
    const notConfig = ['#', '['];

    lines.forEach(str => {
        if (!notConfig.some(char => str.startsWith(char))) {
            const pair = str.split("" = "");
            console.log(""attempting to update config item on site for "" + str);
            if(pair[0] && pair[1]) //truthy
                updateConfig(pair[0], pair[1]);
        }
        else {
            console.log(""below line is not a config item\n"" + str);
        }
    });
}

function updateConfig(key, value) {
    if (key === null || value === null) {
        console.warn(""Cannot update key-value pair, one item is NULL"");
        return;
    }

    key = key.trim();
    value = value.trim();

    const matching = document.getElementsByName(key);
    if (matching.length === 1) {
        const element = matching[0];
        let typ = matching[0].getAttribute(""type"");
        let next = element.nextElementSibling;
        if (typ === ""checkbox"") {
            if (value === ""true"")
                element.setAttribute(""checked"", ""checked"");
            else
                element.removeAttribute(""checked"");
        } else if (element.hasAttribute(""value"")) {
            if (typ === ""range"") {
                if (next !== null) {
                    next.setAttribute(""value"", value);
                    next.textContent = value;
                }
                element.setAttribute(""value"", value);
            } else {
                element.setAttribute(""value"", value);
                element.textContent = value;
                console.log(""not a range setting values"");
            }
            
        } else {
            console.warn(""Unable to find attribute to update for: "" + key + value);
        }
    } else {
        matching.forEach(doc => {
            if (doc.hasAttribute(""type"") && doc.hasAttribute(""value"")) {
                let atr = doc.getAttribute(""value"");
                let typ = doc.getAttribute(""type"");
                if (typ === ""radio"") {
                    if (atr === value) {
                        doc.setAttribute(""checked"", ""checked"");
                    } else {
                        console.log(""not the button we are looking for, skipping & removing attribute"");
                        doc.removeAttribute(""checked"");
                    }
                }
            }
        });
    }
}


function loadFileAsText() {
    var fileToLoad = document.getElementById(""fileInput"").files[0];

    var fileReader = new FileReader();
    fileReader.onload = function (fileLoadedEvent) {
        var textFromFileLoaded = fileLoadedEvent.target.result;
        parseConfig(textFromFileLoaded);
    };

    fileReader.readAsText(fileToLoad, ""UTF-8"");
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

            using var compressedStream = new MemoryStream(gzipBytes);
            using var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
            using var reader = new StreamReader(gzipStream, Encoding.UTF8);
            return reader.ReadToEnd();
        }

        public static void ReadCompressedConfig(ref ConfigEntry<string> configEntry, ConfigFile ModConfig)
        {
            string compressedDataBase64 = configEntry.Value;
            string decompressed = DecompressBase64Gzip(compressedDataBase64);

            Dictionary<string, string> fromString = ParseHelper.ParseKeyValuePairs(decompressed);

            if (fromString.Count == 0)
                return;

            foreach (KeyValuePair<string, string> pair in fromString)
            {
                if (TryFindConfigItem(pair.Key, ModConfig, out ConfigEntryBase configItem))
                {
                    if (configItem.BoxedValue.GetType() == typeof(bool))
                    {
                        ChangeBool(ModConfig, configItem, pair.Value.ToLower());
                    }
                    else if (configItem.BoxedValue.GetType() == typeof(string))
                    {
                        ChangeString(ModConfig, configItem, pair.Value);
                    }
                    else if (configItem.BoxedValue.GetType() == typeof(int))
                    {
                        ChangeInt(ModConfig, configItem, pair.Value);
                    }
                    else if (configItem.BoxedValue.GetType() == typeof(float))
                    {
                        ChangeFloat(ModConfig, configItem, pair.Value);
                    }
                }
            }

            configEntry.Value = "";
        }
    }
}

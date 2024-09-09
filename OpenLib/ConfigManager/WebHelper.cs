using System.Collections.Generic;


namespace OpenLib.ConfigManager
{
    public class WebHelper
    {
        public static string AddValueToHTMLCode(string value, string configName, int num, bool isDefaultValue)
        {
            /*
            <input type="radio" id="age1" name="age" value="30">
            <label for="age1">0 - 30</label><br>
            <input type="radio" id="age2" name="age" value="60">
            <label for="age2">31 - 60</label><br>  
            <input type="radio" id="age3" name="age" value="100">
            <label for="age3">61 - 100</label>
            */

            if (isDefaultValue)
            {
                Plugin.Spam($"{configName} is default value, setting this to checked");
                return $"<input type=\"radio\" id=\"{configName}{num}\" checked=\"checked\" name=\"{configName}\" value=\"{value}\">\r\n            <label for=\"{configName}{num}\">{value}</label><br>";
            }

            return $"<input type=\"radio\" id=\"{configName}{num}\" name=\"{configName}\" value=\"{value}\">\r\n            <label for=\"{configName}{num}\">{value}</label><br>";
        }

        public static string AddValueToHTMLCode(List<float> values, string configName, string defaultValue)
        {
            /*
            <input type="range" value="24" min="1" max="100" oninput="this.nextElementSibling.value = this.value">
            <output>24</output>

            <input type="range" id="ageInputId" value="24" min="1" max="100" oninput="ageOutputId.value = ageInputId.value">
            <input id="ageOutputId" style="text-align:center" type="text" size="2" value="24" oninput="ageInputId.value = ageOutputId.value">
            */

            if (values.Count != 2)
                return "";

            if (values[1] > 999)
            {
                Plugin.Spam($"clamped number-type max value too high for slider - {values[1]}");
                return $"<input name=\"{configName}\" type=\"number\" class=\"stringInput\" min=\"{values[0]}\" max=\"{values[1]}\" value=\"{defaultValue}\" />";   
            }

            if (values[0].ToString().Contains("."))
                return $"<input type=\"range\" name=\"{configName}\" id=\"{configName}_slider\" value=\"{defaultValue}\" step=\"0.001\" min=\"{values[0]}\" max=\"{values[1]}\" oninput=\"{configName}_text.value = {configName}_slider.value\">\r\n" +
                    $"<input id=\"{configName}_text\" type=\"text\" class=\"numberInput\" size=\"4\" min=\"{values[0]}\" max=\"{values[1]}\" value=\"{defaultValue}\" oninput=\"{configName}_slider.value = {configName}_text.value\">";



            return $"<input type=\"range\" name=\"{configName}\" id=\"{configName}_slider\" value=\"{defaultValue}\" min=\"{values[0]}\" max=\"{values[1]}\" oninput=\"{configName}_text.value = {configName}_slider.value\">\r\n" +
                $"<input id=\"{configName}_text\" type=\"text\" class=\"numberInput\" size=\"4\" min=\"{values[0]}\" max=\"{values[1]}\" value=\"{defaultValue}\" oninput=\"{configName}_slider.value = {configName}_text.value\">";
        }
    }
}

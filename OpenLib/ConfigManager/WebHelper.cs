using System.Collections.Generic;


namespace OpenLib.ConfigManager
{
    public class WebHelper
    {
        public static string AddValueToHTMLCode(string value, string configName, int num)
        {
            /*
            <input type="radio" id="age1" name="age" value="30">
            <label for="age1">0 - 30</label><br>
            <input type="radio" id="age2" name="age" value="60">
            <label for="age2">31 - 60</label><br>  
            <input type="radio" id="age3" name="age" value="100">
            <label for="age3">61 - 100</label>
            */

            return $"<input type=\"radio\" id=\"{configName}{num}\" name=\"{configName}\" value=\"{value}\">\r\n            <label for=\"{configName}{num}\">{value}</label><br>";
        }

        public static string AddValueToHTMLCode(List<float> values, string configName)
        {
            /*
            <input type="range" value="24" min="1" max="100" oninput="this.nextElementSibling.value = this.value">
            <output>24</output>
            */

            if (values.Count != 2)
                return "";

            if (values[1] > 999)
            {
                Plugin.Spam($"clamped number-type max value too high for slider - {values[1]}");
                return $"<input name=\"{configName}\" type=\"number\" value=\"{values[1]}\" />";   
            }

            if (values[0].ToString().Contains("."))
                return $"<input type=\"range\" name=\"{configName}\" value=\"{values[0]}\" step=\"0.001\" min=\"{values[0]}\" max=\"{values[1]}\" oninput=\"this.nextElementSibling.value = this.value\">\r\n            <output>{values[0]}</output>";

            return $"<input type=\"range\" name=\"{configName}\" value=\"{values[0]}\" min=\"{values[0]}\" max=\"{values[1]}\" oninput=\"this.nextElementSibling.value = this.value\">\r\n            <output>{values[0]}</output>";
        }
    }
}

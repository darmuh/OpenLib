using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TerminalStuff.StringStuff;

namespace TerminalStuff
{
    internal class LevelCommands
    {
        internal static List<string> bannedWeather = [];
        internal static List<string> bannedWeatherConfig = [];
        //ChangeLevelServerRpc
        internal static string RouteRandomCommand()
        {
            string displayText;

            if (Plugin.instance.Terminal.groupCredits < ConfigSettings.routeRandomCost.Value)
            {
                displayText = $"You cannot afford to run the 'route random' command.\r\n\r\n\tRoute Random Cost: [{ConfigSettings.routeRandomCost.Value}]\r\n\tYour credits: <color=#BD3131>[{Plugin.instance.Terminal.groupCredits}]</color>\r\n\r\n\r\n";
                return displayText;
            }
                
            bannedWeatherConfig = GetKeywordsPerConfigItem(ConfigSettings.routeRandomBannedWeather.Value);
            bannedWeather = GetListToLower(bannedWeatherConfig);

            List<SelectableLevel> validLevels = [];
            //StartOfRound.Instance.levels
            //Plugin.instance.Terminal.moonsCatalogueList

            foreach (SelectableLevel level in Plugin.instance.Terminal.moonsCatalogueList)
            {
                if (bannedWeather.Contains(level.currentWeather.ToString().ToLower()))
                {
                    Plugin.MoreLogs($"{level.PlanetName} has banned weather: {level.currentWeather}");
                }
                else
                {
                    validLevels.Add(level);
                    Plugin.MoreLogs($"Added {level.PlanetName} to valid random planets");
                }
            }

            if(validLevels.Count < 1)
            {
                displayText = $"Route Random was unable to select a valid moon and you have not been charged.\r\n\r\nThis may be due to all moons have banned weather attributes...\r\n\r\n\r\n";
                return displayText;
            }

            int randomIndex = Random.Range(0, validLevels.Count);
            Plugin.MoreLogs($"{validLevels[randomIndex].PlanetName} has been chosen!");

            StartOfRound.Instance.ChangeLevelServerRpc(validLevels[randomIndex].levelID, Plugin.instance.Terminal.groupCredits);

            int newCreds = CostCommands.CalculateNewCredits(Plugin.instance.Terminal.groupCredits, ConfigSettings.routeRandomCost.Value, Plugin.instance.Terminal);

            displayText = $"Your new balance is ■{newCreds} Credits.\r\n\r\nRoute Random has chosen {validLevels[randomIndex].PlanetName}!\r\n\r\n\tEnjoy!\r\n\r\n";
            return displayText;
        }
    }
}

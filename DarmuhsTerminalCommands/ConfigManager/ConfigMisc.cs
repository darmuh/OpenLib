using System;

namespace OpenLib.ConfigManager
{
    public class ConfigMisc
    {
        public enum TornadoWeatherType
        {
            Fire,
            Blood,
            Windy,
            Smoke,
            Water,
            Electric
        }

        internal static void OnConfigReloaded(object sender, EventArgs e)
        {
            Plugin.Log.LogInfo("Config has been reloaded!");
            ConfigSetup.ReadConfigAndAssignValues(Plugin.instance.Config, ConfigSetup.defaultManagedBools);
        }

      
    }
}

using BepInEx.Configuration;

namespace OpenLib.ConfigManager
{
    public class ConfigWatch<T>(ConfigEntry<T> entry, bool networking = false)
    {
        public ConfigEntry<T> ConfigItem = entry;
        public bool NetworkingReq = networking;
        public ConfigEntry<bool> networkingConfig = null!;
        public string Name = entry.Definition.Key;
    }


}

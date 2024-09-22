using LobbyCompatibility.Features;
using System;
using System.Reflection;

namespace OpenLib.Compat
{
    public class BMX_LobbyCompat
    {
        internal static void SetCompat(bool isNetworked)
        {
            if (!Plugin.instance.LobbyCompat)
                return;

            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            if (isNetworked)
            {
                PluginHelper.RegisterPlugin(Plugin.PluginInfo.PLUGIN_GUID, version, LobbyCompatibility.Enums.CompatibilityLevel.Everyone, LobbyCompatibility.Enums.VersionStrictness.Patch);
            }
            else
            {
                PluginHelper.RegisterPlugin(Plugin.PluginInfo.PLUGIN_GUID, version, LobbyCompatibility.Enums.CompatibilityLevel.ClientOnly, LobbyCompatibility.Enums.VersionStrictness.Patch);
            }
        }

        public static bool SetBMXCompat(bool isNetworked, Version version) //for public use
        {
            if (!Plugin.instance.LobbyCompat)
            {
                return false;
            }

            if (isNetworked)
            {
                PluginHelper.RegisterPlugin(Plugin.PluginInfo.PLUGIN_GUID, version, LobbyCompatibility.Enums.CompatibilityLevel.Everyone, LobbyCompatibility.Enums.VersionStrictness.Patch);
                return true;
            }
            else
            {
                PluginHelper.RegisterPlugin(Plugin.PluginInfo.PLUGIN_GUID, version, LobbyCompatibility.Enums.CompatibilityLevel.ClientOnly, LobbyCompatibility.Enums.VersionStrictness.Patch);
                return true;
            }
        }
    }
}

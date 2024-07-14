using LobbyCompatibility.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenLib.Compat
{
    internal class BMX_LobbyCompat
    {
        public static void SetCompat(bool isNetworked)
        {
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
    }
}

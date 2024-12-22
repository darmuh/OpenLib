

namespace OpenLib.Compat
{
    public class TerminalStuffMod
    {
        public static void NetSync(TerminalNode node)
        {
            if (node == null)
                return;

            if (!Plugin.instance.TerminalStuff)
                return;

            if (!TerminalStuff.ConfigSettings.NetworkedNodes.Value || !TerminalStuff.ConfigSettings.ModNetworking.Value)
                return;

            Plugin.Spam($"Syncing node with TerminalStuff");
            TerminalStuff.EventSub.TerminalParse.NetSync(node);
        }

        public static void LoadAndSync(TerminalNode node)
        {
            if (node == null) 
                return;

            Plugin.instance.Terminal.LoadNewNode(node);
            Plugin.Spam($"Loading node!");

            if (!Plugin.instance.TerminalStuff)
                return;

            if (!TerminalStuff.ConfigSettings.NetworkedNodes.Value || !TerminalStuff.ConfigSettings.ModNetworking.Value)
                return;

            Plugin.Spam($"Syncing with TerminalStuff!");
            TerminalStuff.EventSub.TerminalParse.NetSync(node);
        }


    }
}

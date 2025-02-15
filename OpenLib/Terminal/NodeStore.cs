using BepInEx.Configuration;
using HarmonyLib;
using OpenLib.Common;
using static OpenLib.CoreMethods.AddingThings;
using static OpenLib.CoreMethods.DynamicBools;

namespace OpenLib.CoreMethods
{
    public class NodeStore(CommandManager cmd, string name, ConfigEntry<int> configInt, int maxStock = 0, bool alwaysInStock = false)
    {
        public bool AlwaysInStock = alwaysInStock;
        public int MaxStock = maxStock;
        public ConfigEntry<int> PriceConfig = configInt;
        public string Name = name;
        public CommandManager Command = cmd;


        public void AddToStore()
        {
            if (CommonTerminal.BuyKeyword == null)
                return;

            if (Command.ConfirmBase == null)
                Command.ConfirmBase.CreateConfirmation();

            TerminalKeyword buy = CommonTerminal.BuyKeyword;

            Command.terminalNode.terminalOptions = [Command.ConfirmBase.Confirm, Command.ConfirmBase.Deny];

            UnlockableItem storeItem = AddUnlockable(Name, Command.terminalNode, AlwaysInStock, MaxStock);
            if (!StartOfRound.Instance.unlockablesList.unlockables.Contains(storeItem))
                StartOfRound.Instance.unlockablesList.unlockables.Add(storeItem);
            int unlockableID = StartOfRound.Instance.unlockablesList.unlockables.IndexOf(storeItem);

            Command.terminalNode.creatureName = Name; //too lazy to define this at the top level
            Command.terminalNode.shipUnlockableID = unlockableID;
            Command.terminalNode.itemCost = PriceConfig.Value;
            Command.ConfirmBase.Confirm.result.shipUnlockableID = unlockableID;
            Command.ConfirmBase.Confirm.result.buyUnlockable = false;
            Command.ConfirmBase.Confirm.result.itemCost = PriceConfig.Value;

            Command.terminalKeywords.Do(x => AddToBuyWord(ref buy, ref x, storeItem));

            Plugin.ShopNodes.Add(Command.terminalNode);
            Plugin.Spam($"Store nodes created for {Name}");
        }
    }
}

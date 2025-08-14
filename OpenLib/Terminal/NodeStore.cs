using BepInEx.Configuration;
using HarmonyLib;
using OpenLib.Common;
using static OpenLib.CoreMethods.AddingThings;

namespace OpenLib.CoreMethods
{
    public class NodeStore(CommandManager cmd)
    {
        public bool AlwaysInStock = false;
        public int MaxStock = 0;
        public ConfigEntry<int> PriceConfig = null!;
        public CommandManager Command = cmd;
        public string Name = cmd.Name;
        public int ManualPrice = 0;

        public void AddToStore()
        {
            if (CommonTerminal.BuyKeyword == null)
                return;

            if (Command.ConfirmBase == null)
                Command.ConfirmBase.CreateConfirmation();

            int price;

            if (PriceConfig == null)
                price = ManualPrice;
            else
                price = PriceConfig.Value;

            TerminalKeyword buy = CommonTerminal.BuyKeyword;

            Command.terminalNode.terminalOptions = [Command.ConfirmBase.Confirm, Command.ConfirmBase.Deny];

            UnlockableItem storeItem = AddUnlockable(Name, Command.terminalNode, AlwaysInStock, MaxStock);
            if (!StartOfRound.Instance.unlockablesList.unlockables.Contains(storeItem))
                StartOfRound.Instance.unlockablesList.unlockables.Add(storeItem);
            int unlockableID = StartOfRound.Instance.unlockablesList.unlockables.IndexOf(storeItem);

            Command.terminalNode.creatureName = Name; //too lazy to define this at the top level
            Command.terminalNode.shipUnlockableID = unlockableID;
            Command.terminalNode.itemCost = price;
            Command.ConfirmBase.Confirm.result.shipUnlockableID = unlockableID;
            Command.ConfirmBase.Confirm.result.buyUnlockable = false;
            Command.ConfirmBase.Confirm.result.itemCost = price;

            Command.terminalKeywords.Do(x => AddToBuyWord(ref buy, ref x, storeItem));

            Plugin.ShopNodes.Add(Command.terminalNode);
            Plugin.Spam($"Store nodes created for {Name}");
        }
    }
}

using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace TerminalStuff
{
    internal class CostCommands
    {
        internal static bool vitalsUpgradeEnabled = false;
        internal static bool enemyScanUpgradeEnabled = false;

        internal static bool CheckUpgradeStatus(ref bool upgrade, string itemName)
        {
            if (upgrade)
            {
                Plugin.MoreLogs("upgrade is already set to true");
                return upgrade;
            }

            if (!GameNetworkManager.Instance.localPlayerController.IsHost)
            {
                NetHandler.Instance.GetItemStatusServerRpc(itemName, upgrade);
                return upgrade;
            }

            foreach (UnlockableItem item in StartOfRound.Instance.unlockablesList.unlockables)
            {
                //Plugin.MoreLogs($"Checking {itemName}");
                if (item.unlockableName == itemName)
                {
                    if (item.alreadyUnlocked || item.hasBeenUnlockedByPlayer || item.spawnPrefab)
                    {
                        upgrade = true;
                        Plugin.MoreLogs($"Upgrade: {itemName} already unlocked. Setting variable to true");
                        return upgrade;
                    }
                }
            }

            Plugin.ERROR($"Unlockable could NOT be found {itemName}");
            return upgrade;
        }

        internal static string BioscanCommand()
        {
            string displayText;
            CheckUpgradeStatus(ref enemyScanUpgradeEnabled, "BioScanner 2.0 Upgrade Patch (bioscanpatch)");

            if (RoundManager.Instance != null)
            {
                int scannedEnemies = RoundManager.Instance.SpawnedEnemies.Count;
                int getCreds = Plugin.instance.Terminal.groupCredits;
                int costCreds = ConfigSettings.enemyScanCost.Value;

                if (ShouldRunBioscan2(getCreds, costCreds)) //upgraded bioscan
                {
                    int newCreds = CalculateNewCredits(getCreds, costCreds, Plugin.instance.Terminal);

                    List<EnemyAI> livingEnemies = GetLivingEnemiesList();
                    string filteredLivingEnemiesString = FilterLivingEnemies(livingEnemies);

                    string bioscanResult = GetBioscanResult(scannedEnemies, costCreds, newCreds, filteredLivingEnemiesString);
                    displayText = bioscanResult;
                    Plugin.MoreLogs($"Living Enemies(filtered): {filteredLivingEnemiesString}");
                    return displayText;
                }
                else if (getCreds >= costCreds) //nonupgraded
                {
                    int newCreds = CalculateNewCredits(getCreds, costCreds, Plugin.instance.Terminal);
                    string bioscanResult = GetBasicBioscanResult(scannedEnemies, costCreds, newCreds);
                    displayText = bioscanResult;
                    Plugin.MoreLogs("v1 scanner utilized, only numbers shown");
                    return displayText;
                }
                else
                {
                    displayText = "Not enough credits to run Biomatter Scanner.\r\n";
                    Plugin.MoreLogs("brokeboy detected");
                    return displayText;
                }
            }
            else
            {
                displayText = "Cannot scan for Biomatter at this time.\r\n";
                return displayText;
            }
        }

        private static bool ShouldRunBioscan2(int getCreds, int costCreds)
        {
            return enemyScanUpgradeEnabled && getCreds >= costCreds;
        }

        private static string GetBasicBioscanResult(int scannedEnemies, int costCreds, int newCreds)
        {
            return $"Biomatter scanner charged {costCreds} credits and has detected [{scannedEnemies}] non-employee organic objects.\r\n\r\nYour new balance is ■{newCreds} Credits.\r\n";
        }

        private static List<EnemyAI> GetLivingEnemiesList()
        {
            return RoundManager.Instance.SpawnedEnemies.Where(enemy => !enemy.isEnemyDead).ToList();
        }

        private static string FilterLivingEnemies(List<EnemyAI> livingEnemies)
        {
            string livingEnemiesString = string.Join(Environment.NewLine, livingEnemies.Select(enemy => enemy.ToString()));
            string pattern = @"\([^)]*\)";
            return Regex.Replace(livingEnemiesString, pattern, string.Empty);
        }
        private static string GetBioscanResult(int scannedEnemies, int costCreds, int newCreds, string filteredLivingEnemiesString)
        {
            string bioscanResult = $"Biomatter scanner charged {costCreds} credits and has detected [{scannedEnemies}] non-employee organic objects.\r\n\r\n";

            if (!string.IsNullOrEmpty(filteredLivingEnemiesString))
            {
                bioscanResult += $"Your new balance is ■{newCreds} Credits.\r\n\r\nDetailed scan has defined these objects as the following in the registry: \r\n{filteredLivingEnemiesString}\r\n";
            }
            else
            {
                bioscanResult += $"Your new balance is ■{newCreds} Credits.\r\n";
                Plugin.MoreLogs("v1 scanner utilized, only numbers shown");
            }

            return bioscanResult;
        }
        internal static string VitalsCommand()
        {
            string displayText;
            CheckUpgradeStatus(ref vitalsUpgradeEnabled, "Vitals Scanner Upgrade (vitalspatch)");
            PlayerControllerB getPlayerInfo = StartOfRound.Instance.mapScreen.targetedPlayer;

            if (getPlayerInfo == null)
            {
                displayText = $"Vitals command malfunctioning...\n\n";
                return displayText;
            }

            int getCreds = Plugin.instance.Terminal.groupCredits;
            int costCreds = GetCostCreds(vitalsUpgradeEnabled);

            string playername = getPlayerInfo.playerUsername;

            Plugin.MoreLogs("playername: " + playername);

            if (ShouldDisplayVitals(getPlayerInfo, getCreds, costCreds))
            {
                int newCreds = CalculateNewCredits(getCreds, costCreds, Plugin.instance.Terminal);

                string vitalsInfo = GetVitalsInfo(getPlayerInfo);
                string creditsInfo = GetCreditsInfo(newCreds);

                if (!vitalsUpgradeEnabled)
                {
                    displayText = $"Charged ■{costCreds} Credits. \n{vitalsInfo}\n{creditsInfo}";
                    return displayText;
                }
                else
                {
                    displayText = $"{vitalsInfo}\n{creditsInfo}";
                    return displayText;
                }
            }
            else
            {
                displayText = $"{ConfigSettings.vitalsPoorString.Value}\n";
                return displayText;
            }
        }
        internal static int GetCostCreds(bool upgradeStatus)
        {
            if (!upgradeStatus)
            {
                return ConfigSettings.vitalsCost.Value;
            }
            else
            {
                return 0;
            }
        }
        internal static string AskBioscanUpgrade()
        {
            if (enemyScanUpgradeEnabled == false)
            {
                string patchASK = $"Purchase the BioScanner 2.0 Upgrade Patch?\nThis software update is available for {ConfigSettings.bioScanUpgradeCost.Value} Credits.\n\n\n\n\n\n\n\n\n\n\nPlease CONFIRM or DENY.\n";
                return patchASK;
            }
            else
            {
                string displayText = $"BioScanner software has already been updated to the latest patch (2.0).\r\n\r\n";
                return displayText;
            }
        }
        internal static string AskVitalsUpgrade()
        {
            if (vitalsUpgradeEnabled == false)
            {
                string patchASK = $"Purchase the Vitals Scanner 2.0 Patch?\nThis software update is available for {ConfigSettings.vitalsUpgradeCost.Value} Credits.\n\n\n\n\n\n\n\n\n\n\nPlease CONFIRM or DENY.\n";
                return patchASK;
            }
            else
            {
                string displayText = $"Vitals Scanner software has already been updated to the latest patch (2.0).\r\n\r\n";
                return displayText;
            }
        }

        internal static string PerformBioscanUpgrade()
        {
            if (enemyScanUpgradeEnabled == false)
            {
                int newCreds = Plugin.instance.Terminal.groupCredits - ConfigSettings.bioScanUpgradeCost.Value;
                string displayText = $"Biomatter Scanner software has been updated to the latest patch (2.0) and now provides more detailed information!\r\n\r\nYour new balance is ■{newCreds} Credits\r\n";
                enemyScanUpgradeEnabled = true;
                return displayText;
            }
            else
            {
                string displayText = $"BioScanner software has already been updated to the latest patch (2.0).\r\n\r\n";
                return displayText;
            }
        }

        internal static string GetRefund()
        {
            string displayText;
            int deliverables = Plugin.instance.Terminal.numberOfItemsInDropship;
            Item[] buyables = Plugin.instance.Terminal.buyableItemsList;
            List<int> items = Plugin.instance.Terminal.orderedItemsFromTerminal;
            List<string> returnlist = [];
            int refund = 0;

            Plugin.MoreLogs($"buyables: {buyables.Length}, deliverables: {deliverables}, items: {items.Count}");

            if (deliverables > 0)
            {
                foreach (int num in items)
                {
                    if (num <= buyables.Length)
                    {
                        refund += buyables[num].creditsWorth;
                        string itemname = buyables[num].itemName;
                        returnlist.Add(itemname + "\n");
                        Plugin.MoreLogs($"Adding {itemname} ${buyables[num].creditsWorth} to refund list");
                    }
                    else
                    {
                        string itemname = buyables[num].itemName;
                        Plugin.MoreLogs($"Unable to add {itemname} ${buyables[num].creditsWorth} to refund list");
                    }

                }
                Plugin.MoreLogs($"old creds: {Plugin.instance.Terminal.groupCredits}");
                int newCreds = Plugin.instance.Terminal.groupCredits + refund;
                Plugin.instance.Terminal.groupCredits = newCreds;
                Plugin.MoreLogs($"new creds: {newCreds}");
                Plugin.instance.Terminal.orderedItemsFromTerminal.Clear();

                NetHandler.Instance.SyncCreditsServerRpc(newCreds, 0);

                string allitems = ListToStringBuild(returnlist);

                Plugin.MoreLogs($"Refund total: ${refund}");
                displayText = $"Cancelling order for: {allitems}\nYou have been refunded ■{refund} Credits!\r\n";
                return displayText;
            }
            else
                displayText = "No ordered items detected on the dropship.\n\n";

            return displayText;
        }

        private static string ListToStringBuild(List<string> list)
        {
            StringBuilder sb = new();

            for (int i = 0; i < list.Count; i++)
            {
                sb.Append(list[i]);
            }

            return sb.ToString();
        }

        internal static string PerformVitalsUpgrade()
        {
            if (vitalsUpgradeEnabled == false)
            {
                int newCreds = Plugin.instance.Terminal.groupCredits - ConfigSettings.vitalsCost.Value;
                vitalsUpgradeEnabled = true;
                string displayText = $"Vitals Scanner software has been updated to the latest patch (2.0) and no longer requires credits to scan.\r\n\r\nYour new balance is ■{newCreds} credits\r\n";
                return displayText;
            }
            else
            {
                string displayText = "Update already purchased.\n";
                return displayText;
            }
        }

        private static bool ShouldDisplayVitals(PlayerControllerB playerInfo, int getCreds, int costCreds)
        {
            return !playerInfo.isPlayerDead && (getCreds >= costCreds || vitalsUpgradeEnabled);
        }

        internal static int CalculateNewCredits(int getCreds, int costCreds, Terminal frompatch)
        {
            int newCreds = getCreds - costCreds;
            frompatch.groupCredits = newCreds;

            NetHandler.Instance.SyncCreditsServerRpc(newCreds, frompatch.numberOfItemsInDropship);
            return newCreds;
        }

        private static string GetVitalsInfo(PlayerControllerB playerInfo)
        {
            int playerHealth = playerInfo.health;
            float playerWeight = playerInfo.carryWeight;
            float playerSanity = playerInfo.insanityLevel;
            bool hasFlash = playerInfo.ItemSlots.Any(item => item is FlashlightItem);
            float realWeight = Mathf.RoundToInt(Mathf.Clamp(playerWeight - 1f, 0f, 100f) * 105f);

            string vitalsInfo = $"{playerInfo.playerUsername} Vitals:\n\n Health: {playerHealth}\n Weight: {realWeight}\n Sanity: {playerSanity}";

            if (hasFlash)
            {
                float flashCharge = Mathf.RoundToInt(playerInfo.pocketedFlashlight.insertedBattery.charge * 100);
                vitalsInfo += $"\n Flashlight Battery Percentage: {flashCharge}%";
            }

            return vitalsInfo;
        }

        private static string GetCreditsInfo(int newCreds)
        {
            return $"Your new balance is ■{newCreds} Credits.\r\n";
        }

    }
}

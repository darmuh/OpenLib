using System.Collections.Generic;
using static TerminalStuff.StringStuff;

namespace TerminalStuff
{
    internal class GetFromConfig
    {
        internal static List<string> alwaysOnKW;
        internal static List<string> camsKW;
        internal static List<string> mapKW;
        internal static List<string> mmKW;
        internal static List<string> mcKW;
        internal static List<string> ovKW;
        internal static List<string> doorKW;
        internal static List<string> lightsKW;
        internal static List<string> modsKW;
        internal static List<string> tpKW;
        internal static List<string> itpKW;
        internal static List<string> quitKW;
        internal static List<string> lolKW;
        internal static List<string> clearKW;
        internal static List<string> dangerKW;
        internal static List<string> healKW;
        internal static List<string> lootKW;
        internal static List<string> suitKW;
        internal static List<string> clockKW;
        internal static List<string> itemsKW;
        internal static List<string> scrapKW;
        internal static List<string> leverKW;
        internal static List<string> mirrorKW;
        internal static List<string> rrKW;

        internal static void CreateKeywordLists()
        {
            alwaysOnKW = GetKeywordsPerConfigItem(ConfigSettings.alwaysOnKeywords.Value);
            camsKW = GetKeywordsPerConfigItem(ConfigSettings.camsKeywords.Value);
            mapKW = GetKeywordsPerConfigItem(ConfigSettings.mapKeywords.Value);
            mmKW = GetKeywordsPerConfigItem(ConfigSettings.minimapKeywords.Value);
            mcKW = GetKeywordsPerConfigItem(ConfigSettings.minicamsKeywords.Value);
            ovKW = GetKeywordsPerConfigItem(ConfigSettings.overlayKeywords.Value);
            doorKW = GetKeywordsPerConfigItem(ConfigSettings.doorKeywords.Value);
            lightsKW = GetKeywordsPerConfigItem(ConfigSettings.lightsKeywords.Value);
            modsKW = GetKeywordsPerConfigItem(ConfigSettings.modsKeywords.Value);
            tpKW = GetKeywordsPerConfigItem(ConfigSettings.tpKeywords.Value);
            itpKW = GetKeywordsPerConfigItem(ConfigSettings.itpKeywords.Value);
            quitKW = GetKeywordsPerConfigItem(ConfigSettings.quitKeywords.Value);
            lolKW = GetKeywordsPerConfigItem(ConfigSettings.videoKeywords.Value);
            clearKW = GetKeywordsPerConfigItem(ConfigSettings.clearKeywords.Value);
            dangerKW = GetKeywordsPerConfigItem(ConfigSettings.dangerKeywords.Value);
            healKW = GetKeywordsPerConfigItem(ConfigSettings.healKeywords.Value);
            lootKW = GetKeywordsPerConfigItem(ConfigSettings.lootKeywords.Value);
            suitKW = GetKeywordsPerConfigItem(ConfigSettings.randomSuitKeywords.Value);
            clockKW = GetKeywordsPerConfigItem(ConfigSettings.clockKeywords.Value);
            itemsKW = GetKeywordsPerConfigItem(ConfigSettings.ListItemsKeywords.Value);
            scrapKW = GetKeywordsPerConfigItem(ConfigSettings.ListScrapKeywords.Value);
            leverKW = GetKeywordsPerConfigItem(ConfigSettings.leverKeywords.Value);
            mirrorKW = GetKeywordsPerConfigItem(ConfigSettings.mirrorKeywords.Value);
            rrKW = GetKeywordsPerConfigItem(ConfigSettings.randomRouteKeywords.Value);
        }
    }
}

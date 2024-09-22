using System;

namespace OpenLib.CoreMethods
{
    public class FauxKeyword
    {
        public TerminalNode MainPage;
        public TerminalNode thisNode = BasicTerminal.CreateNewTerminalNode();
        public string Keyword;
        public Func<string> ResultFunc;
        public Func<string> ConfirmFunc;
        public Func<string> DenyFunc;
        public string ConfirmText;
        public string DenyText;
        public bool GetConfirm;

        public FauxKeyword(string mainWord, string keyword, Func<string> resultFunc)
        {
            if (DynamicBools.TryGetKeyword(mainWord, out TerminalKeyword mainPage))
            {
                this.MainPage = mainPage.specialKeywordResult;
                this.Keyword = keyword;
                this.ResultFunc = resultFunc;
                this.thisNode.clearPreviousText = true;
                this.thisNode.name = keyword;
                Plugin.Spam($"FauxKeyword - {keyword} created!");
                return;
            }
            else
                Plugin.WARNING($"Could not find main page at word - {mainWord}");

            this.MainPage = null;
            this.Keyword = keyword;
        }

        public void AddConfirm(Func<string> confirmFunc, Func<string> denyFunc = null)
        {
            this.GetConfirm = false;
            this.ConfirmFunc = confirmFunc;
            if (denyFunc != null)
                this.DenyFunc = denyFunc;
        }

        public void AddText(string denyText, string confirmText = "")
        {
            this.GetConfirm = false;
            this.ConfirmText = confirmText;
            this.DenyText = denyText;
        }
    }
}

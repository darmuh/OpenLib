using System;


namespace OpenLib.CoreMethods
{
    public class NodeConfirmation(string name, Func<string> confirmAction = null, Func<string> denyAction = null)
    {
        public string Name = name;
        public Func<string> ConfirmFunc = confirmAction;
        public Func<string> DenyFunc = denyAction;
        public string DenyTxt = "DenyFunc";
        public string ConfirmTxt = "ConfirmFunc";

        //nouns
        public CompatibleNoun Confirm = null!;
        public CompatibleNoun Deny = null!;

        public void CreateConfirmation()
        {
            Confirm = BasicTerminal.CreateCompatibleNoun(Name + "_confirm", "confirm", ConfirmTxt);
            Deny = BasicTerminal.CreateCompatibleNoun(Name + "_deny", "deny", DenyTxt);
        }

    }


}

using System;


namespace OpenLib.CoreMethods;

public class NodeConfirmation(CommandManager manager, Func<string> confirmAction = null!, Func<string> denyAction = null!)
{
    public CommandManager Manager = manager;
    public string Name = manager.Name;
    public Func<string> ConfirmFunc { get; set; } = confirmAction;
    public Func<string> DenyFunc { get; set; } = denyAction;
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

    public void SetConfirmAction(Func<string> confirmAction)
    {
        ConfirmFunc = confirmAction;
    }
    public void SetConfirmText(string text)
    {
        ConfirmTxt = text;
    }

    public void SetDenyAction(Func<string> denyAction)
    {
        DenyFunc = denyAction;
    }

    public void SetDenyText(string text)
    {
        DenyTxt = text;
    }

}



namespace OpenLib.CoreMethods
{
    //might replace this with an individual property (class has one singular property at the moment)
    public class NodeSpecial(int specialNum)
    {
        //used by commands that need sync-able identifiers (cams commands in terminalstuff)
        public int SpecialNum = specialNum;

    }
}

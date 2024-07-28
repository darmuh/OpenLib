using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenLib.Common
{
    public class CommonEnums
    {
        public static bool quitTerminalEnum = false; //define as false when needed

        public static IEnumerator TerminalQuitter(Terminal terminal)
        {
            if (quitTerminalEnum)
                yield break;

            quitTerminalEnum = true;

            yield return new WaitForSeconds(0.5f);
            terminal.QuitTerminal();

            quitTerminalEnum = false;
        }

        public static IEnumerator TerminalQuitter(Terminal terminal, bool syncTerminalInUse) //overload for bool use
        {
            if (quitTerminalEnum)
                yield break;

            quitTerminalEnum = true;

            yield return new WaitForSeconds(0.5f);
            terminal.QuitTerminal(syncTerminalInUse);

            quitTerminalEnum = false;
        }
    }
}

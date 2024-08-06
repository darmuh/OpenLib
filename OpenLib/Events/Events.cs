// https://github.com/IAmBatby/LethalLevelLoader/blob/51f9af254c38f926f808f1714bb6dc52bb5f66dc/LethalLevelLoader/General/EventPatches.cs#L287-L307
// https://github.com/AndreyMrovol/LethalMrovLib/blob/main/MrovLib/Events/Events.cs
// used above as template for this version of event creation

using DigitalRuby.ThunderAndLightning;

namespace OpenLib.Events
{
    public class Events
    {
        public class CustomEvent<T>
        {
            public delegate void ParameterEvent(T param);
            private event ParameterEvent OnParameterEvent;
            public bool HasListeners => (Listeners != 0);
            public int Listeners { get; internal set; }

            public void Invoke(T param)
            {
                OnParameterEvent?.Invoke(param);
            }

            public void AddListener(ParameterEvent listener)
            {
                OnParameterEvent += listener;
                Listeners++;
            }

            public void RemoveListener(ParameterEvent listener)
            {
                OnParameterEvent -= listener;
                Listeners--;
            }

        }

        public class TerminalNodeEvent
        {
            public delegate TerminalNode Event(ref TerminalNode original);
            private event Event OnEvent;
            public bool HasListeners => (Listeners != 0);
            public int Listeners { get; internal set; }

            public TerminalNode NodeInvoke(ref TerminalNode original)
            {
                TerminalNode node = OnEvent?.Invoke(ref original);
                return node;
            }
            public void AddListener(Event listener)
            {
                OnEvent += listener;
                Listeners++;
            }

            public void RemoveListener(Event listener)
            {
                OnEvent -= listener;
                Listeners--;
            }
        }

        public class TerminalKeywordEvent
        {
            public delegate TerminalKeyword Event(ref TerminalKeyword original);
            private event Event OnEvent;
            public bool HasListeners => (Listeners != 0);
            public int Listeners { get; internal set; }

            public TerminalKeyword WordInvoke(ref TerminalKeyword original)
            {
                TerminalKeyword word = OnEvent?.Invoke(ref original);
                return word;
            }
            public void AddListener(Event listener)
            {
                OnEvent += listener;
                Listeners++;
            }

            public void RemoveListener(Event listener)
            {
                OnEvent -= listener;
                Listeners--;
            }
        }

        public class CustomEvent
        {
            public delegate void Event();
            private event Event OnEvent;
            public bool HasListeners => (Listeners != 0);
            public int Listeners { get; internal set; }

            public void Invoke()
            {
                OnEvent?.Invoke();
            }

            public void AddListener(Event listener)
            {
                OnEvent += listener;
                Listeners++;
            }

            public void RemoveListener(Event listener)
            {
                OnEvent -= listener;
                Listeners--;
            }
        }
    }
}

using GameNetcodeStuff;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static OpenLib.Plugin;
using Object = UnityEngine.Object;

namespace OpenLib.CoreMethods
{
    public class CodeCollection<T>
    {
        public static List<TerminalCodeObject<T>> terminalCodeObjects = [];
    }

    public class TerminalCodeObject<T> : MonoBehaviour
    {
        public TerminalAccessibleObject TerminalCode;
        public bool DynamicMapIcon = false;
        public RadarTransform RadarTransform = null;
        public UnityEvent<TerminalAccessibleObject, T> OnCodeUsed = new();
        public UnityEvent<TerminalAccessibleObject, T> OnCooldownComplete = new();
        public T obj;

        //isDoorType:
        //sets color of code based on whether it's open or closed
        //use isDoorOpen value to determine whether the game object's associated AnimatedObjectTrigger component is open or closed
        //set isDoorType to false unless you have this component part of your game object!

        public TerminalCodeObject(T itemClass, GameObject gameObj, bool dynamicMapIcon = false, bool isDoorType = false)
        {
            DynamicMapIcon = dynamicMapIcon;
            TerminalCode = AssignCodeToObject(gameObj, isDoorType);
            obj = itemClass;

            CodeCollection<T>.terminalCodeObjects.Add(this);
        }

        public void SetTimers(float codeAccessCooldownTimer, float currentCooldownTimer)
        {
            Plugin.Spam("SetTimers!");
            TerminalCode.codeAccessCooldownTimer = codeAccessCooldownTimer;
            TerminalCode.currentCooldownTimer = currentCooldownTimer;
        }

        public void CodeUsed(PlayerControllerB thisPlayer)
        {
            Plugin.Spam("CodeUsed!");
            OnCodeUsed.Invoke(TerminalCode, obj);
        }

        public void CooldownComplete(PlayerControllerB thisPlayer)
        {
            Plugin.Spam("CooldownComplete!");
            OnCooldownComplete.Invoke(TerminalCode, obj);
        }

        public void OnDestroy()
        {
            Spam("CodeObject is destroyed! Removed from list");
            CodeCollection<T>.terminalCodeObjects.Remove(this);
        }

        public TerminalAccessibleObject AssignCodeToObject(GameObject gameObj, bool isDoorType = false)
        {
            Plugin.Spam("AssignCodeToObject");

            if(AllTerminalCodes.Count == 0)
                AllTerminalCodes = [.. Object.FindObjectsByType<TerminalAccessibleObject>(FindObjectsSortMode.None)];

            if (gameObj == null)
            {
                Plugin.ERROR("NULL GAME OBJECT PROVIDED!!");
                return null;
            }

            TerminalAccessibleObject ObjectCode;

            if (gameObj.GetComponent<TerminalAccessibleObject>() != null)
                ObjectCode = gameObj.GetComponent<TerminalAccessibleObject>(); //re-use existing TAO if it exists
            else
                ObjectCode = gameObj.AddComponent<TerminalAccessibleObject>();

            ObjectCode.isBigDoor = isDoorType;

            ObjectCode.terminalCodeEvent = new();
            ObjectCode.terminalCodeCooldownEvent = new();
            ObjectCode.terminalCodeEvent.AddListener(CodeUsed);
            ObjectCode.terminalCodeCooldownEvent.AddListener(CooldownComplete);

            int codeIndex = 0;
            int loopCount = 0;

            do
            {
                Plugin.Spam($"Object code at index [ {codeIndex} ] in use!");
                codeIndex = GetFreshCode(); //ensure unique code!!!
                loopCount++;
                Plugin.Spam($"New index of [ {codeIndex} ] chosen!");
                
            } while (AllTerminalCodes.Any(x => x.objectCode == RoundManager.Instance.possibleCodesForBigDoors[codeIndex]) && loopCount < 5);
            
            ObjectCode.SetCodeTo(codeIndex);
            ObjectCode.InitializeValues();

            AllTerminalCodes.Add(ObjectCode);

            Plugin.Log.LogInfo($"{gameObj.name} assigned code - {ObjectCode.objectCode}");
            TerminalCode = ObjectCode;

            if (DynamicMapIcon)
            {
                RadarTransform = gameObj.AddComponent<RadarTransform>();
                RadarTransform.Target = gameObj;
                RadarTransform.theCode = TerminalCode;
            }

            return ObjectCode;
        }

        public static int GetFreshCode()
        {
            System.Random random = new();
            return random.Next(RoundManager.Instance.possibleCodesForBigDoors.Length);  
        }   
    }

    public class RadarTransform : MonoBehaviour
    {
        public GameObject Target;
        public RectTransform CodeObj;
        public TerminalAccessibleObject theCode;


        public void UpdateValues()
        {
            Plugin.Spam("RadarTransform - UpdateValues");
            CodeObj = theCode.mapRadarBox.transform.parent.GetComponent<RectTransform>();

            if (CodeObj == null)
                Plugin.ERROR("Unable to get mapcode RectTransform!");
        }

        private void OnDestroy()
        {
            Plugin.Spam("RadarTransform destroyed!!!");
            Plugin.Spam("RadarTransform destroyed!!!");
            Plugin.Spam("RadarTransform destroyed!!!");
        }

        private void Start()
        {
            Plugin.Spam("RadarTransform created!");
            UpdateValues();
        }

        private void Update()
        {
            if (CodeObj != null)
            {
                CodeObj.position = Target.transform.position + Vector3.up * 4.35f;
                CodeObj.position += CodeObj.transform.up * 1.2f - CodeObj.transform.right * 1.2f;
            }
            else
                UpdateValues();

            /*
            
            GameObject gameObject = Object.Instantiate(StartOfRound.Instance.objectCodePrefab, StartOfRound.Instance.mapScreen.mapCameraStationaryUI, worldPositionStays: false);
            RectTransform component = gameObject.GetComponent<RectTransform>();
            component.position = base.transform.position + Vector3.up * 4.35f;
            component.position += component.up * 1.2f - component.right * 1.2f;

             */

        }
    }
}

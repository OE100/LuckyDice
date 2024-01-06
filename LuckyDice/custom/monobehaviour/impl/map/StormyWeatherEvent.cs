using System.Collections;
using GameNetcodeStuff;
using LuckyDice.custom.monobehaviour.def;
using LuckyDice.custom.network;
using LuckyDice.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.monobehaviour.impl.map
{
    public class StormyWeatherEvent : BaseEventBehaviour
    {
        private void Awake()
        {
            IsOneTimeEvent = true;
            NeedsRemoval = false;
            EventManager.Instance.DisplayMessageClientRPC(
                new NetworkObjectReference(),
                "A storm is brewing...",
                "Well, what are you gonna do now Midas?");
            EventManager.Instance.SetStormClientRPC(true);
            StartCoroutine(TurnItemsMetal());
        }

        private IEnumerator TurnItemsMetal()
        {
            while (true)
            {
                foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
                {
                    GrabbableObject heldObjectServer = player.currentlyHeldObjectServer;
                    
                    if (heldObjectServer == null)
                        continue;
                    
                    if (!heldObjectServer.itemProperties.isConductiveMetal)
                    {
                        heldObjectServer.itemProperties.isConductiveMetal = true;
                        Utils.StormyWeather.metalObjects.Add(heldObjectServer);
                    }
                }

                yield return new WaitForSeconds(3);
            }
        } 
    }
}
using System.Collections;
using GameNetcodeStuff;
using LuckyDice.custom.events;
using LuckyDice.custom.monobehaviour.attributes;
using LuckyDice.custom.monobehaviour.def;
using LuckyDice.custom.network;
using LuckyDice.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.monobehaviour.impl.map
{
    [OneTimeEvent]
    public class StormyWeatherEvent : BaseEventBehaviour
    {
        private void Awake()
        {
            EventManager.Instance.DisplayMessageClientRPC(
                new NetworkObjectReference(),
                "A storm is brewing...",
                "Well, what are you gonna do now Midas?");
            EventManager.Instance.SetStormClientRPC(true);
            StartCoroutine(TurnItemsMetal());
        }

        protected virtual IEnumerator TurnItemsMetal()
        {
            while (true)
            {
                foreach (var player in StartOfRound.Instance.allPlayerScripts)
                {
                    var heldObjectServer = player.currentlyHeldObjectServer;
                    
                    if (heldObjectServer == null || EventRegistry.GetPoolFromItem(heldObjectServer.GetType()) != null)
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
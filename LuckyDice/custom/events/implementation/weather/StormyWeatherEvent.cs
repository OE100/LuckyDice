using System.Collections;
using GameNetcodeStuff;
using LuckyDice.custom.events.prototype;
using LuckyDice.custom.network;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.events.implementation.weather
{
    public class StormyWeatherEvent : BaseDiceEvent
    {
        public override bool IsOneTime() => true;
        
        public override void Run()
        {
        }

        public override void AddPlayer(PlayerControllerB player)
        {
            running = true;
            EventManager.Instance.StartCoroutine(EventCoroutine());
        }

        public override IEnumerator EventCoroutine()
        {
            EventManager.Instance.SetStormClientRPC(true);

            yield return new WaitForSeconds(10);
            
            EventManager.Instance.DisplayMessageClientRPC(
                new NetworkObjectReference(),
                "Is that lightning I see?",
                "But it's not even raining...");
            
            while (running)
            {
                if (IsPhaseForbidden())
                {
                    running = false;
                    EventManager.Instance.SetStormClientRPC(false);
                }

                yield return new WaitForSeconds(5);
            }
        }
    }
}
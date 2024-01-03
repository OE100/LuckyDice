using System.Collections;
using GameNetcodeStuff;
using LuckyDice.custom.events.prototype;
using LuckyDice.custom.network;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.events.implementation.map
{
    public class ExplodeLandmines : BaseDiceEvent
    {
        public override void AddPlayer(PlayerControllerB player)
        {
            EventManager.Instance.StartCoroutine(EventCoroutine());
        }

        public override void Run()
        {
        }

        public override IEnumerator EventCoroutine()
        {
            Landmine[] mineObjects = Object.FindObjectsOfType<Landmine>();
            if (mineObjects.Length == 0)
            {
                EventManager.Instance.DisplayMessageClientRPC(
                    new NetworkObjectReference(),
                    "No landmines found!",
                    "You got lucky this time...");
                yield break;
            }
            bool waiting = true;

            int count;
            Plugin.Log.LogDebug("Started waiting until more than half of players are in factory");
            while (waiting)
            {
                if (IsPhaseForbidden())
                {
                    Plugin.Log.LogDebug("Phase is forbidden, stopping event");
                    yield break;
                }
                
                count = 0;
                foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
                    if (player.isInsideFactory)
                        count++;
                
                Plugin.Log.LogDebug($"{count} players are in factory, {StartOfRound.Instance.allPlayerScripts.Length / 2} needed");
                if (count >= StartOfRound.Instance.livingPlayers / 2)
                {
                    Plugin.Log.LogDebug("Enough players are in factory, starting event in 5 seconds");
                    waiting = false;
                }

                yield return new WaitForSeconds(5);
            }
            
            EventManager.Instance.DisplayMessageClientRPC(
                new NetworkObjectReference(),
                "Are those landmines?",
                "Well, a little explosion never hurt anyone...");
            
            yield return new WaitForSeconds(5);
            
            foreach (Landmine mineObject in mineObjects)
                EventManager.Instance.DetonateMineClientRPC(mineObject.GetComponentInParent<NetworkObject>());
        }

        public override bool IsOneTime() => true;
    }
}
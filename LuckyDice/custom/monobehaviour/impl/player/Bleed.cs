using System.Collections.Generic;
using GameNetcodeStuff;
using LuckyDice.custom.monobehaviour.def;
using LuckyDice.custom.network;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.monobehaviour.impl.player
{
    public class Bleed : BasePlayerEvent
    {
        private static float timeToBleed = 2.4f;
        private float timeSinceBleed = 0f;

        private void Awake()
        {
            Plugin.Log.LogDebug("Bleed event Awake!");
            NeedsRemoval = false;
            IsOneTimeEvent = false;
        }

        protected override void Update()
        {
            base.Update();
            
            if (playersToMult.Count == 0)
                return;
            
            if (IsPhaseForbidden())
                playersToMult.Clear();
            
            timeSinceBleed += Time.deltaTime;
            
            if (timeSinceBleed >= timeToBleed)
            {
                timeSinceBleed = 0;

                foreach (KeyValuePair<PlayerControllerB,int> pair in playersToMult)
                {
                    if (pair.Key.isInElevator || pair.Key.isInHangarShipRoom)
                        playersToRemove.Add(pair.Key);
                    else
                        EventManager.Instance.BleedPlayerClientRPC(
                            new NetworkObjectReference(pair.Key.GetComponentInParent<NetworkObject>()),
                            true,
                            pair.Value * 2);
                }
            }
        }

        protected override void OnRemovePlayer(PlayerControllerB player)
        {
            EventManager.Instance.BleedPlayerClientRPC(
                new NetworkObjectReference(player.GetComponentInParent<NetworkObject>()),
                false);
        }

        protected override void OnAddPlayer(PlayerControllerB player)
        {
            MessagePlayer(player);
        }

        protected override void OnAddToPlayer(PlayerControllerB player)
        {
            MessagePlayer(player);
        }

        private void MessagePlayer(PlayerControllerB player)
        {
            EventManager.Instance.DisplayMessageClientRPC(
                new NetworkObjectReference(player.GetComponentInParent<NetworkObject>()), 
                "Unlucky roll!", 
                "Your tongue begins to bleed..."
            );
        }
    }
}
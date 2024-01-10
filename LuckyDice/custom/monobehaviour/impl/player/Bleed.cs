using GameNetcodeStuff;
using LuckyDice.custom.monobehaviour.attributes;
using LuckyDice.custom.monobehaviour.def;
using LuckyDice.custom.network;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.monobehaviour.impl.player
{
    [MountAtRegistry]
    public class Bleed : BasePlayerEvent
    {
        protected const float TimeToBleed = 2.4f; // todo: read from config
        protected float TimeSinceBleed = 0f;
        
        protected override void Update()
        {
            base.Update();
            
            if (playersToMult.Count == 0)
                return;
            
            if (IsPhaseForbidden())
                playersToMult.Clear();
            
            TimeSinceBleed += Time.deltaTime;
            
            if (TimeSinceBleed >= TimeToBleed)
            {
                TimeSinceBleed = 0;

                foreach (var (key, value) in playersToMult)
                {
                    if (key.isInElevator || key.isInHangarShipRoom)
                        playersToRemove.Add(key);
                    else
                        EventManager.Instance.BleedPlayerClientRPC(
                            new NetworkObjectReference(key.GetComponentInParent<NetworkObject>()),
                            true,
                            value * 2);
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
#region

using System.Collections.Generic;
using LuckyDice.custom.network;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;
using Event = LuckyDice.custom.network.Event;

#endregion

namespace LuckyDice.custom.items.dice
{
    public class DiceItem : GrabbableObject
    {
        internal static AudioClip rollSound = null;
        protected List<Event> outcomes = new List<Event>();
        protected AudioSource audioSource;

        public override void Start()
        {
            base.Start();
            audioSource = GetComponent<AudioSource>();
            if (rollSound == null)
            {
                Plugin.Log.LogDebug("Loading dice sound from ab");
                if ((rollSound = Plugin.ab.LoadAsset<AudioClip>("assets/custom/luckydice/sounds/rolling_dice.mp3")) != null)
                    Plugin.Log.LogDebug("Loaded dice sound from ab");
                else 
                    Plugin.Log.LogError("Failed to load dice sound from ab");
            }

            audioSource.clip = rollSound;
        }
        
        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            // if in ship phase or on company moon don't roll
            if (StartOfRound.Instance.inShipPhase || StartOfRound.Instance.currentLevelID == 3)
            {
                return;
            }
            // else activate item and despawn it
            base.ItemActivate(used, buttonDown);
            ItemActivateServerRPC(used, buttonDown);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ItemActivateServerRPC(bool used, bool buttonDown = true)
        {
            // roll dice and despawn it
            if (NetworkManager.Singleton.IsHost)
            {
                Roll();
            }
            ItemActivateClientRPC(used, buttonDown);
        }
        
        [ClientRpc]
        private void ItemActivateClientRPC(bool used, bool buttonDown = true)
        {
            playerHeldBy.DespawnHeldObject();
        }

        [ServerRpc(RequireOwnership = false)]
        private void TriggerEventServerRPC(int side)
        {
            EventManager.Instance.AddPlayerToEventServerRPC(outcomes[side], new NetworkObjectReference(playerHeldBy.GetComponentInParent<NetworkObject>()));
            TriggerEventClientRPC(side);
        }
        
        [ClientRpc]
        private void TriggerEventClientRPC(int side)
        {
            audioSource.Play();
            TriggerEvent(side);
        }
        
        private void TriggerEvent(int side)
        {
            if (side >= outcomes.Count || side < 0)
            {
                return;
            }
            Plugin.Log.LogDebug($"Rolled event: {outcomes[side].ToString()}");
        }
    
        private void Roll()
        {
            int side = Random.Range(0, outcomes.Count);
            TriggerEventServerRPC(side);
        }
    }
}
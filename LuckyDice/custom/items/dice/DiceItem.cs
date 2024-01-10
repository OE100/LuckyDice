using GameNetcodeStuff;
using HarmonyLib;
using LuckyDice.custom.network;
using LuckyDice.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.items.dice
{
    public class DiceItem : GrabbableObject
    {
        [SerializeField]
        protected AudioSource audioSource = null!;

        protected virtual string UseTooltip() => "Roll Dice [LMB]";

        public override void Start()
        {
            base.Start();
            itemProperties.toolTips.AddToArray(UseTooltip());
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            // if in ship phase or on company moon don't roll
            if (itemUsedUp || StartOfRound.Instance.inShipPhase || StartOfRound.Instance.currentLevelID == 3)
                return;
            // else activate item and despawn it
            playerHeldBy.activatingItem = true;
            if (IsOneTimeUse())
                itemUsedUp = true;
            base.ItemActivate(used, buttonDown);
            ItemActivateServerRPC(used, buttonDown);
        }

        protected virtual bool IsOneTimeUse() => true;
        
        protected virtual void OnItemActivateServerRPCEvent() {}
        
        protected virtual void OnItemActivateClientRPCEvent() {}
        
        [ServerRpc(RequireOwnership = false)]
        private void ItemActivateServerRPC(bool used, bool buttonDown = true)
        {
            // roll dice and despawn it
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                if (IsOneTimeUse())
                    itemUsedUp = true;
                
                OnItemActivateServerRPCEvent();
                EventManager.Instance.TriggerEventFromPoolServerRPC(
                    new NetworkObjectReference(GetComponentInParent<NetworkObject>()),
                    new NetworkObjectReference(playerHeldBy.GetComponentInParent<NetworkObject>()),
                    playerHeldBy.currentItemSlot);

                playerHeldBy.activatingItem = true;
                StartCoroutine(Utils.DelayedDestroy(() => playerHeldBy == null, gameObject));
            }
            
            ItemActivateClientRPC(used, buttonDown);
        }
        
        [ClientRpc]
        private void ItemActivateClientRPC(bool used, bool buttonDown = true)
        {
            if (IsOneTimeUse())
                itemUsedUp = true;
            
            OnItemActivateClientRPCEvent();
            
            playerHeldBy.activatingItem = false;
            
            DestroyObjectInHand(playerHeldBy);
        }
    }
}
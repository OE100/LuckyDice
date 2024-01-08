using LuckyDice.custom.network;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.items.dice
{
    public class DiceItem : GrabbableObject
    {
        [SerializeField]
        protected AudioSource audioSource;
        
        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            // if in ship phase or on company moon don't roll
            if (StartOfRound.Instance.inShipPhase || StartOfRound.Instance.currentLevelID == 3)
                return;
            // else activate item and despawn it
            base.ItemActivate(used, buttonDown);
            ItemActivateServerRPC(used, buttonDown);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ItemActivateServerRPC(bool used, bool buttonDown = true)
        {
            // roll dice and despawn it
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
                EventManager.Instance.TriggerEventFromPoolServerRPC(new NetworkObjectReference(GetComponentInParent<NetworkObject>()));
            ItemActivateClientRPC(used, buttonDown);
        }
        
        [ClientRpc]
        private void ItemActivateClientRPC(bool used, bool buttonDown = true)
        {
            DestroyObjectInHand(playerHeldBy);
        }
    }
}
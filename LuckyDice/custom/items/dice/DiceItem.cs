#region

using System.Collections.Generic;
using LuckyDice.custom.network;
using Unity.Netcode;
using Random = UnityEngine.Random;
using Event = LuckyDice.custom.network.Event;

#endregion

namespace LuckyDice.custom.items.dice
{
    public class DiceItem : GrabbableObject
    {
        protected List<Event> outcomes = new List<Event>();
        
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
            TriggerEvent(side);
        }
        
        private void TriggerEvent(int side)
        {
            if (side >= outcomes.Count || side < 0)
            {
                return;
            }
            Plugin.Log.LogMessage($"Rolled event: {outcomes[side].ToString()}");
        }
    
        private void Roll()
        {
            int side = Random.Range(0, outcomes.Count);
            TriggerEventServerRPC(side);
        }
    }
}
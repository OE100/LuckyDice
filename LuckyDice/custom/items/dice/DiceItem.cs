using System.Collections.Generic;
using LuckyDice.custom.network;
using UnityEngine;
using Random = UnityEngine.Random;
using Event = LuckyDice.custom.network.Event;

namespace LuckyDice.custom.items.dice
{
    public class DiceItem : GrabbableObject
    {
        protected List<Event> outcomes = new List<Event>();
    
        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            Roll();
            playerHeldBy.DespawnHeldObject();
        }

        private void TriggerEvent(int side)
        {
            if (side >= outcomes.Count || side < 0)
            {
                return;
            }

            EventManager.Instance.AddPlayerToEventServerRPC(outcomes[side], playerHeldBy.playerSteamId);
        }
    
        private void Roll()
        {
            int side = Random.Range(0, outcomes.Count);
            TriggerEvent(side);
        }
    }
}
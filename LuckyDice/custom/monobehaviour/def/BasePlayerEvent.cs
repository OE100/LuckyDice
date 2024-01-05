using System.Collections.Generic;
using GameNetcodeStuff;

namespace LuckyDice.custom.monobehaviour.def
{
    public class BasePlayerEvent : BaseEventBehaviour 
    {
        protected Dictionary<PlayerControllerB, int> playersToMult = new Dictionary<PlayerControllerB, int>();
        protected List<PlayerControllerB> playersToRemove = new List<PlayerControllerB>();

        public virtual void AddPlayer(PlayerControllerB player)
        {
            if (playersToMult.ContainsKey(player))
            {
                playersToMult[player]++;
                OnAddToPlayer(player);
            }
            else
            {
                playersToMult.Add(player, 1);
                OnAddPlayer(player);
            }
        }

        public virtual void RemovePlayer(PlayerControllerB player)
        {
            if (!playersToMult.ContainsKey(player)) 
                return;
            
            if (playersToMult[player] > 0)
            {
                playersToMult[player]--;
                OnRemoveFromPlayer(player);
            }
            else
            {
                playersToMult.Remove(player);
                OnRemovePlayer(player);
            }
        }

        protected override void Update()
        {
            if (playersToRemove.Count > 0)
                playersToRemove.ForEach(RemovePlayer);
        }

        protected virtual void OnRemovePlayer(PlayerControllerB player)
        {
        }
        
        protected virtual void OnRemoveFromPlayer(PlayerControllerB player)
        {
        }
        
        protected virtual void OnAddPlayer(PlayerControllerB player)
        {
        }
        
        protected virtual void OnAddToPlayer(PlayerControllerB player)
        {
        }
    }
}
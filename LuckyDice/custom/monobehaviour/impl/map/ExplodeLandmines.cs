using LuckyDice.custom.monobehaviour.attributes;
using LuckyDice.custom.monobehaviour.def;
using LuckyDice.custom.network;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.monobehaviour.impl.map
{
    [OneTimeEvent]
    public class ExplodeLandmines : BaseEventBehaviour
    {
        protected float TimeToBlow;
        protected Landmine[] Mines = null!;
        
        private void Awake()
        {
            Plugin.Log.LogDebug($"ExplodeLandmines event Awake!");
            Mines = FindObjectsOfType<Landmine>();
            if (Mines.Length == 0)
                EventManager.Instance.DisplayMessageClientRPC(
                    new NetworkObjectReference(),
                    "No landmines found!",
                    "You got lucky this time...");
            else 
                EventManager.Instance.DisplayMessageClientRPC(
                    new NetworkObjectReference(),
                    "Are those landmines?",
                    "Well, a little explosion never hurt anyone...");
            TimeToBlow = 3f;
        }

        protected override void Update()
        {
            if (TimeToBlow < 0f)
            {
                Plugin.Log.LogDebug("ExplodeLandmines event time reached 0!");
                ExplodeMines();
                Destroy(this);
            }
            
            TimeToBlow -= Time.deltaTime;
            
        }
        
        private void ExplodeMines()
        {
            if (Mines.Length == 0)
            {
                Plugin.Log.LogDebug($"No landmines found!");
                return;
            }
            Plugin.Log.LogDebug($"Starting to explode landmines!");
            foreach (var mine in Mines)
                mine.ExplodeMineServerRpc();
        }
    }
}
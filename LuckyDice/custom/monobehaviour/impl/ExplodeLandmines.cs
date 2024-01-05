using LuckyDice.custom.monobehaviour.def;
using LuckyDice.custom.network;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.monobehaviour.impl
{
    public class ExplodeLandmines : BaseEventBehaviour
    {
        private float time;
        private Landmine[] mines;
        
        private void Awake()
        {
            Plugin.Log.LogDebug($"ExplodeLandmines event Awake!");
            IsOneTimeEvent = true;
            NeedsRemoval = false;
            mines = FindObjectsOfType<Landmine>();
            if (mines.Length == 0)
                EventManager.Instance.DisplayMessageClientRPC(
                    new NetworkObjectReference(),
                    "No landmines found!",
                    "You got lucky this time...");
            else 
                EventManager.Instance.DisplayMessageClientRPC(
                    new NetworkObjectReference(),
                    "Are those landmines?",
                    "Well, a little explosion never hurt anyone...");
            time = 3f;
        }

        protected override void Update()
        {
            if (time < 0f)
            {
                Plugin.Log.LogDebug($"ExplodeLandmines event time reached 0!");
                ExplodeMines();
                Destroy(this);
            }
            
            time -= Time.deltaTime;
            
        }
        
        private void ExplodeMines()
        {
            if (mines.Length == 0)
            {
                Plugin.Log.LogDebug($"No landmines found!");
                return;
            }
            Plugin.Log.LogDebug($"Starting to explode landmines!");
            foreach (Landmine mine in mines)
                mine.ExplodeMineServerRpc();
        }
    }
}
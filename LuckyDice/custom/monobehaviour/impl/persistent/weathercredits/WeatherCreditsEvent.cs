using LuckyDice.custom.monobehaviour.def;
using LuckyDice.custom.network;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.monobehaviour.impl.persistent.weathercredits
{
    public class WeatherCreditsEvent : BaseEventBehaviour
    {
        public static int Credits = 0;

        protected float TimeUntilTrigger = 2f;
        
        public int CreditsPerTrigger = 1;
        
        private void LateUpdate()
        {
            // delayed trigger
            if (TimeUntilTrigger > 0)
            {
                TimeUntilTrigger -= Time.deltaTime;
                return;
            }
            Credits += CreditsPerTrigger;
            EventManager.Instance.DisplayMessageClientRPC(new NetworkObjectReference(),
                "Jewish Space Laser Online!",
                $"You get {CreditsPerTrigger} credits for disabling weather effects!");
            Destroy(this);
        }
    }
}
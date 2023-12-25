using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using LuckyDice.custom.network;

namespace LuckyDice.custom.events.prototype
{
    public abstract class MultiplierDiceEvent<TMultiplier> : IDiceEvent
    {
        protected bool running = false;
        protected Dictionary<PlayerControllerB, TMultiplier> playersToMult = new Dictionary<PlayerControllerB, TMultiplier>();
        
        public abstract void AddPlayer(PlayerControllerB player);

        public abstract void RemovePlayer(PlayerControllerB player);

        public void Run()
        {
            running = true;
            EventManager.Instance.StartCoroutine(EventCoroutine());
        }

        public void Stop()
        {
            running = false;
        }

        public abstract IEnumerator EventCoroutine();
        
        protected static bool IsPhaseForbidden()
        {
            return StartOfRound.Instance.inShipPhase || StartOfRound.Instance.currentLevelID == 3;
        }
    }
}
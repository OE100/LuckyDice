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

        public virtual void Run()
        {
            if (!RoundManager.Instance.IsHost && !RoundManager.Instance.IsServer)
                return;
            running = true;
            EventManager.Instance.StartCoroutine(EventCoroutine());
        }

        public virtual void Stop()
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
using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using LuckyDice.custom.network;

namespace LuckyDice.custom.events.prototype
{
    public abstract class BaseDiceEvent : IDiceEvent
    {
        protected bool running = false;
        protected List<PlayerControllerB> players = new List<PlayerControllerB>();
        
        public virtual void AddPlayer(PlayerControllerB player)
        {
            players.Add(player);
        }

        public virtual void RemovePlayer(PlayerControllerB player)
        {
            players.Remove(player);
        }

        public virtual void Run()
        {
            if (RoundManager.Instance.IsHost || RoundManager.Instance.IsServer)
            {
                running = true;
                EventManager.Instance.StartCoroutine(EventCoroutine());
            }
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
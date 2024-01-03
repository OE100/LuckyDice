#region

using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using LuckyDice.custom.network;
using Unity.Netcode;

#endregion

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
            Plugin.Log.LogDebug($"IsHost: {NetworkManager.Singleton.IsHost}, IsServer: {NetworkManager.Singleton.IsServer}, condition: {!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer}");
            if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer)
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

        public abstract bool IsOneTime();
    }
}
using System.Collections.Generic;
using GameNetcodeStuff;
using LuckyDice.custom.monobehaviour.def;
using LuckyDice.custom.network;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.monobehaviour.impl.player
{
    public class TroubleInTerroristTown : BasePlayerEvent
    {
        private float timeRemaining;
        private bool started;
        private bool warningPlayed;
        
        private void Awake()
        {
            Plugin.Log.LogDebug("TroubleInTerroristTown Awake!");
            NeedsRemoval = false;
            IsOneTimeEvent = true;
        }

        public override void AddPlayer(PlayerControllerB player)
        {
            List<PlayerControllerB> players = new List<PlayerControllerB>(StartOfRound.Instance.allPlayerScripts);
            players.RemoveAll(p => p.isPlayerDead);
            if (players.Count <= 1)
            {
                Plugin.Log.LogDebug("One player alive, cancelling trouble in terrorist town.");
                Destroy(this);
            }
            // select non-terrorist
            players.RemoveAt(Random.Range(0, players.Count));
            // select terrorist
            int tInd = Random.Range(0, players.Count);
            playersToMult[players[tInd]] = 1;
            players.RemoveAt(tInd);
            // randomly roll the rest
            while (players.Count > 0)
            {
                // todo: replace with terrorist chance from config
                if (Random.Range(0f, 1f) > 0.5f)
                    playersToMult[players[0]] = 1;
                players.RemoveAt(0);
            }

            started = true;
            timeRemaining = 20f; // todo: replace with random in range from config
            EventManager.Instance.DisplayMessageClientRPC(
                new NetworkObjectReference(),
                "Trouble in terrorist town!",
                "You may or may not have bombs on you."
            );
        }

        protected override void Update()
        {
            if (IsPhaseForbidden())
                Destroy(this);

            if (!started)
                return;
            
            if (playersToMult.Count > 0)
            {
                if (timeRemaining <= 15 && !warningPlayed)
                {
                    warningPlayed = true;
                    foreach (KeyValuePair<PlayerControllerB,int> pair in playersToMult)
                    {
                        if (pair.Value < 1)
                            continue;

                        pair.Key.voiceMuffledByEnemy = true;
                        
                        EventManager.Instance.DisplayMessageClientRPC(
                            new NetworkObjectReference(pair.Key.GetComponentInParent<NetworkObject>()),
                            "You are a terrorist!",
                            "Try to blow everyone up."
                        );
                    }
                }
                if (timeRemaining <= 0f)
                {
                    foreach (KeyValuePair<PlayerControllerB,int> pair in playersToMult)
                        if (pair.Value > 0)
                            Kaboom(pair.Key);
                    
                    Destroy(this);
                }
            }
            
            timeRemaining -= Time.deltaTime;
        }
        
        private void Kaboom(PlayerControllerB player)
        {
            player.voiceMuffledByEnemy = false;
            EventManager.Instance.SpawnExplosionOnPlayerClientRPC(
                new NetworkObjectReference(player.GetComponentInParent<NetworkObject>()));
        }
    }
}
﻿using System.Collections.Generic;
using GameNetcodeStuff;
using LuckyDice.custom.monobehaviour.def;
using LuckyDice.custom.network;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.monobehaviour.impl.player
{
    public class TroubleInTerroristTown : BaseEventBehaviour
    {
        private float timeRemaining;
        private bool started;
        private bool warningPlayed;
        private List<PlayerControllerB> terrorists;
        
        private void Awake()
        {
            Plugin.Log.LogDebug("TroubleInTerroristTown Awake!");
            NeedsRemoval = false;
            IsOneTimeEvent = true;
            Plugin.Log.LogDebug("TroubleInTerroristTown AddPlayer!");
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
            terrorists = new List<PlayerControllerB>();
            int tInd = Random.Range(0, players.Count);
            terrorists.Add(players[tInd]);
            players.RemoveAt(tInd);
            // randomly roll the rest
            while (players.Count > 0)
            {
                // todo: replace with terrorist chance from config
                if (Random.Range(0f, 1f) > 0.5f)
                    terrorists.Add(players[0]);
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
            
            if (terrorists.Count > 0)
            {
                if (timeRemaining <= 15 && !warningPlayed)
                {
                    warningPlayed = true;
                    foreach (PlayerControllerB player in terrorists)
                    {
                        player.voiceMuffledByEnemy = true;
                        
                        EventManager.Instance.DisplayMessageClientRPC(
                            new NetworkObjectReference(player.GetComponentInParent<NetworkObject>()),
                            "You are a terrorist!",
                            "Try to blow everyone up."
                        );
                    }
                }
                if (timeRemaining <= 0f)
                {
                    foreach (PlayerControllerB player in terrorists)
                        Kaboom(player);
                    
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
            // todo: add value for kills and save ship inventory in case of everyones death by explosions
        }
    }
}
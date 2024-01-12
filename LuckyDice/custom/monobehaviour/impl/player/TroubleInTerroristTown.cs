using GameNetcodeStuff;
using LuckyDice.custom.monobehaviour.attributes;
using LuckyDice.custom.monobehaviour.def;
using LuckyDice.custom.network;
using LuckyDice.Utilities;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LuckyDice.custom.monobehaviour.impl.player
{
    [OneTimeEvent]
    public class TroubleInTerroristTown : BaseEventBehaviour
    {
        protected float TimeRemaining;
        protected bool Started;
        protected bool WarningPlayed;
        protected List<PlayerControllerB> Terrorists;
        
        private void Awake()
        {
            Plugin.Log.LogDebug("TroubleInTerroristTown Awake!");
            // roll terrorists
            var players = Utils.GetAllLivingPlayers();
            if (players.Count <= 1)
            {
                Plugin.Log.LogDebug("One player alive, cancelling trouble in terrorist town.");
                Started = false;
                return;
            }

            Started = true;
            WarningPlayed = false;
            // select non-terrorist
            var nInd = Random.Range(0, players.Count);
            Plugin.Log.LogDebug($"Chosen {players[nInd].playerUsername} as non-terrorist");
            players.RemoveAt(nInd);
            // select terrorist
            Terrorists = [];
            var tInd = Random.Range(0, players.Count);
            Plugin.Log.LogDebug($"Chosen {players[tInd].playerUsername} as terrorist");
            Terrorists.Add(players[tInd]);
            players.RemoveAt(tInd);
            // randomly roll the rest
            while (players.Count > 0)
            {
                if (Random.Range(0f, 1f) < ModConfig.TTTTerroristChance.Value)
                {
                    Plugin.Log.LogDebug($"Chosen {players[0].playerUsername} as terrorist");
                    Terrorists.Add(players[0]);
                }
                else
                    Plugin.Log.LogDebug($"Chosen {players[0].playerUsername} as non-terrorist");
                players.RemoveAt(0);
            }

            Started = true;
            TimeRemaining = Random.Range(ModConfig.TTTMinTimeToBlow.Value, ModConfig.TTTMaxTimeToBlow.Value);
            EventManager.Instance.DisplayMessageClientRPC(
                new NetworkObjectReference(),
                "Trouble in terrorist town!",
                "You may or may not have bombs on you."
            );
        }
        
        protected override void Update()
        {
            Plugin.Log.LogDebug($"Checking for forbidden phase: {IsPhaseForbidden()}");
            if (IsPhaseForbidden())
            {
                Destroy(this);
                return;
            }

            Plugin.Log.LogDebug($"Checking for not starting the event: {!Started}");
            if (!Started)
            {
                Destroy(this);
                return;
            }
            
            Plugin.Log.LogDebug($"Checking for terrorists count: {Terrorists.Count}");
            if (Terrorists.Count > 0)
            {
                if (TimeRemaining <= 15 && !WarningPlayed)
                {
                    WarningPlayed = true;
                    foreach (PlayerControllerB player in Terrorists)
                    {
                        player.voiceMuffledByEnemy = true;
                        
                        EventManager.Instance.DisplayMessageClientRPC(
                            new NetworkObjectReference(player.GetComponentInParent<NetworkObject>()),
                            "You are a terrorist!",
                            "Try to blow everyone up."
                        );
                    }
                }
                if (TimeRemaining <= 0f)
                {
                    foreach (var player in Terrorists)
                        Kaboom(player);
                    
                    Destroy(this);
                }
            }
            else
            {
                EventManager.Instance.DisplayMessageClientRPC(
                    new NetworkObjectReference(),
                    "Congratulations!",
                    "All the terrorists have died, you are safe now, well at least safer..."
                    );
                Destroy(this);
            }
            
            TimeRemaining -= Time.deltaTime;
        }

        protected virtual void Kaboom(PlayerControllerB player)
        {
            player.voiceMuffledByEnemy = false;
            EventManager.Instance.SpawnExplosionOnPlayerClientRPC(
                new NetworkObjectReference(player.GetComponentInParent<NetworkObject>()));
            // todo: add value for kills and save ship inventory in case of everyones death by explosions
        }
    }
}
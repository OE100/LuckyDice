using System.Collections;
using GameNetcodeStuff;
using LuckyDice.custom.monobehaviour.def;
using LuckyDice.custom.network;
using LuckyDice.Patches;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.monobehaviour.impl.tweak
{
    public class MaskedChaos : BaseEventBehaviour
    {
        private void Awake()
        {
            Plugin.Log.LogDebug("MaskedChaos Awake");
            
            NeedsRemoval = false;
            IsOneTimeEvent = true;
            
            EventManager.Instance.SetMaskedEnemyChangesClientRPC(true);

            StartCoroutine(EventStartup());
        }

        protected override void Update()
        {
            if (IsPhaseForbidden())
            {
                EventManager.Instance.SetMaskedEnemyChangesClientRPC(false);
                Destroy(this);
            }
        }

        private IEnumerator EventStartup()
        {
            int spawnIndex = RoundManager.Instance.currentLevel.Enemies
                .FindIndex(x => x.enemyType.name == Enemies.MaskedPlayerEnemy);

            if (spawnIndex < 0)
            {
                Plugin.Log.LogError($"Couldn't find {Enemies.MaskedPlayerEnemy} in level enemy list, event will not be triggered!");
                yield break;
            }
            
            EventManager.Instance.DisplayMessageClientRPC(
                new NetworkObjectReference(),
                "Masked Chaos",
                "I suggest you grab the masks before... well before they grab you."
            );
            
            foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
            {
                if (player.isPlayerDead)
                    continue;

                int count = 2;
                while (count > 0)
                {
                    Vector3 randomPos = Utilities.Utils.GetRandomLocationAroundPosition(
                        player.transform.position,
                        radius: 5,
                        randomHeight: false);

                    bool found = Utilities.Utils.ReturnClosestNavMeshPoint(
                        randomPos,
                        out var closestPoint);

                    if (!found)
                    {
                        if (player.isPlayerDead)
                            count = 0;
                        continue;
                    }
                    
                    int mask = Random.Range(0, 2); // 0 = Tragedy, 1 = Comedy
                    EventManager.Instance.SpawnItemAroundPositionServerRPC(
                        closestPoint,
                        itemId: mask + 65,
                        stackValue: 35 - mask * 10);

                    count--;
                }
            }
            
            Plugin.Log.LogDebug("Masked Chaos, waiting for 7 seconds...");
            yield return new WaitForSeconds(7);
            
            Plugin.Log.LogDebug("Spawning masked enemies around players");
            // spawn 4 masked enemies around each player alive
            foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
            {
                if (player.isPlayerDead)
                    continue;

                int count = 4;
                while (count > 0)
                {
                    Vector3 position = Utilities.Utils.GetRandomLocationAroundPosition(
                        origin: player.transform.position,
                        radius: 20,
                        randomHeight: false);

                    bool found = Utilities.Utils.ReturnClosestNavMeshPoint(
                        origin: position,
                        closestPoint:out var closestPoint);

                    if (!found)
                        continue;
                    
                    RoundManager.Instance.SpawnEnemyOnServer(
                        closestPoint, 
                        player.transform.rotation.y, 
                        spawnIndex);

                    count--;
                }
            }
        }
    }
}
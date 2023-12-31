using System.Collections;
using GameNetcodeStuff;
using LuckyDice.custom.events.prototype;
using LuckyDice.custom.network;
using LuckyDice.Patches;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.events.implementation.spawn
{
    public class MaskedChaos : BaseDiceEvent
    {
        internal static bool triggered = false;

        public override void Run()
        {
            base.Run();
            Plugin.Log.LogDebug("Running Masked Chaos event.");
        }

        public override IEnumerator EventCoroutine()
        {
            while (running)
            {
                if (IsPhaseForbidden())
                {
                    if (triggered)
                        StopEvent();
                    yield return new WaitForSeconds(10);
                }

                if (!triggered && players.Count > 0)
                {
                    Plugin.Log.LogDebug("Triggering Masked Chaos event");
                    triggered = true;
                    EventManager.Instance.StartCoroutine(StartEvent());
                }

                yield return new WaitForSeconds(2);
            }
        }

        private IEnumerator StartEvent()
        {
            int spawnIndex = RoundManager.Instance.currentLevel.Enemies
                .FindIndex(x => x.enemyType.name == Enemies.MaskedPlayerEnemy);
            if (spawnIndex == -1)
            {
                Plugin.Log.LogError(
                    $"Couldn't find {Enemies.MaskedPlayerEnemy} in level enemy list, event will not be triggered!");
                yield break;
            }
            EventManager.Instance.DisplayMessageClientRPC(
                new NetworkObjectReference(),
                "Masked Chaos",
                "I suggest you grab the masks before... well before they grab you."
            );
            
            Plugin.Log.LogDebug("Spawning masks beneath players");
            // spawn 2 masks beneath each player alive
            foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
            {
                if (player.isPlayerDead)
                    continue;

                int count = 2;
                while (count > 0)
                {
                    Vector3 randomPos = Utilities.Utilities.GetRandomLocationAroundPosition(
                        player.transform.position,
                        radius: 5,
                        randomHeight: false);

                    bool found = Utilities.Utilities.ReturnClosestNavMeshPoint(
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
                    Vector3 position = Utilities.Utilities.GetRandomLocationAroundPosition(
                        origin: player.transform.position,
                        radius: 20,
                        randomHeight: false);

                    bool found = Utilities.Utilities.ReturnClosestNavMeshPoint(
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
        
        private void StopEvent()
        {
            triggered = false;
            players.Clear();
        }
    }
}
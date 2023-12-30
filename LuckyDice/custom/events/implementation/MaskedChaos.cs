using System.Collections;
using GameNetcodeStuff;
using LuckyDice.custom.events.prototype;
using LuckyDice.custom.network;
using LuckyDice.Patches;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace LuckyDice.custom.events.implementation
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

                for (int i = 0; i < 2; i++)
                {
                    int mask = Random.Range(0, 2); // 0 = Tragedy, 1 = Comedy
                    EventManager.Instance.SpawnScrapOnPlayerServerRPC(
                        new NetworkObjectReference(player.GetComponentInParent<NetworkObject>()),
                        1, 35 - mask * 10, mask + 65
                    );
                }
            }

            yield return new WaitForSeconds(7);
            
            Plugin.Log.LogDebug("Spawning masked enemies around players");
            // spawn 4 masked enemies around each player alive
            foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
            {
                if (player.isPlayerDead)
                    continue;
                
                for (int i = 0; i < 4; i ++)
                {
                    Vector2 randVect2 = Random.insideUnitCircle * 45;
                    Vector3 spawnPos = player.transform.position +
                                       new Vector3(randVect2.x, 0, randVect2.y);
                    // check for closest navmesh point
                    NavMesh.SamplePosition(spawnPos, out NavMeshHit navHit, Mathf.Infinity, NavMesh.AllAreas);
                    if (navHit.hit)
                        spawnPos = navHit.position;
                    float spawnRot = player.transform.rotation.y;
                    RoundManager.Instance.SpawnEnemyOnServer(spawnPos, spawnRot, spawnIndex);
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
using System.Collections;
using GameNetcodeStuff;
using LuckyDice.custom.monobehaviour.attributes;
using LuckyDice.custom.monobehaviour.def;
using LuckyDice.custom.network;
using LuckyDice.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.monobehaviour.impl.tweak
{
    [OneTimeEvent]
    public class MaskedChaos : BaseEventBehaviour
    {
        private void Awake()
        {
            Plugin.Log.LogDebug("MaskedChaos Awake");
            
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
                    Vector3 randomPos = Utils.GetRandomLocationAroundPosition(
                        player.transform.position,
                        radius: 5,
                        randomHeight: false);

                    bool found = Utils.ClosestNavMeshToPosition(
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
                    Vector3 position = Utils.GetRandomLocationAroundPosition(
                        position: player.transform.position,
                        radius: 20,
                        randomHeight: false);

                    bool found = Utils.ClosestNavMeshToPosition(
                        position: position,
                        closestPoint:out var closestPoint);

                    if (!found)
                        continue;
                    
                    GameObject enemy = Instantiate(EnemiesRegistry.GetEnemyPrefab<MaskedPlayerEnemy>(), closestPoint, Random.rotation);
                    enemy.GetComponent<NetworkObject>().Spawn(destroyWithScene:true);
                    RoundManager.Instance.SpawnedEnemies.Add(enemy.GetComponent<EnemyAI>());

                    count--;
                }
            }
        }
    }
}
using System.Collections;
using GameNetcodeStuff;
using LuckyDice.custom.monobehaviour.attributes;
using LuckyDice.custom.network;
using LuckyDice.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.monobehaviour.def
{
    [MountAtRegistry]
    public class BaseSpawnEnemyEvent<TEnemy> : BasePlayerEvent where TEnemy : EnemyAI
    {
        protected virtual GameObject EnemyPrefab() => EnemiesRegistry.GetEnemyPrefab<TEnemy>();
        protected virtual int AmountPerStack() => 1;
        protected virtual string EventMessageHeader() => "The air begins to shift";
        protected virtual string EventMessageBody() => "The molecules around you start to shift and rearrange...";

        protected override void Update()
        {
        }

        public override void AddPlayer(PlayerControllerB player)
        {
            EventManager.Instance.DisplayMessageClientRPC(
                new NetworkObjectReference(player.GetComponentInParent<NetworkObject>()),
                EventMessageHeader(),
                EventMessageBody());
            StartCoroutine(SpawnEnemiesAroundPlayer(player));
        }

        private IEnumerator SpawnEnemiesAroundPlayer(PlayerControllerB player)
        {
            // wait 5 seconds and spawn monsters
            yield return new WaitForSeconds(5f);
            
            int count = AmountPerStack();
            while (count > 0)
            {
                if (IsPhaseForbidden())
                    break;
                
                bool found = Utils.ClosestNavMeshToPosition(
                    Utils.GetRandomLocationAroundPosition(
                        player.transform.position),
                    out var position);
                if (found)
                {
                    EnemyAI enemyAI = Utils.SpawnEnemyPrefab(EnemyPrefab(),
                        position, Random.rotation);
                    enemyAI.StartCoroutine(Utils.DelayedSetOutside(enemyAI, !player.isInsideFactory));
                    count--;
                }
                else
                    Plugin.Log.LogDebug($"Didn't find NavMesh position around player {player.playerUsername}, searching again...");
            }
        }
    }
}
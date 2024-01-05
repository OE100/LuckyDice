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
    public abstract class BaseSpawnEnemyEvent : BasePlayerEvent
    {
        protected int SpawnIndex = -1;
        protected abstract string Name();
        protected abstract int AmountPerStack();

        protected override void Update()
        {
            if (IsPhaseForbidden())
            {
                SpawnIndex = -1;
                return;
            }
            if (SpawnIndex == -1)
            {
                // find spawn index
                SpawnIndex = RoundManager.Instance.currentLevel.Enemies
                    .FindIndex(x => x.enemyType.name == Name());
            }
        }

        public override void AddPlayer(PlayerControllerB player)
        {
            if (SpawnIndex != -1)
            {
                EventManager.Instance.DisplayMessageClientRPC(
                    new NetworkObjectReference(player.GetComponentInParent<NetworkObject>()),
                    "The air begins to shift",
                    "The molecules around you start to shift and rearrange...");
                StartCoroutine(SpawnEnemiesAroundPlayer(player));
            }
        }

        private IEnumerator SpawnEnemiesAroundPlayer(PlayerControllerB player)
        {
            // wait 5 seconds and spawn monsters
            yield return new WaitForSeconds(5f);
            
            int count = AmountPerStack();
            while (count > 0)
            {
                bool found = Utils.ReturnClosestNavMeshPoint(
                    Utils.GetRandomLocationAroundPosition(
                        player.transform.position),
                    out var position);
                if (found)
                {
                    RoundManager.Instance.SpawnEnemyOnServer(
                        position,
                        player.transform.rotation.y,
                        SpawnIndex);
                                            
                    count--;
                }
                else
                    Plugin.Log.LogDebug($"Didn't find NavMesh position around player {player.playerUsername}, searching again...");
            }
        }
    }
}
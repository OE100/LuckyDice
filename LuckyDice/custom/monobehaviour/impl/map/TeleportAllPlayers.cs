using System.Collections;
using GameNetcodeStuff;
using LuckyDice.custom.monobehaviour.attributes;
using LuckyDice.custom.monobehaviour.def;
using LuckyDice.custom.network;
using LuckyDice.Utilities;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LuckyDice.custom.monobehaviour.impl.map
{
    [NeedsRemoval]
    public class TeleportAllPlayers : BaseEventBehaviour
    {
        protected List<PlayerControllerB> Players = null!;
        protected List<EnemyAI> Enemies = null!;
        protected const float TimeBetweenChecks = 5f;
        protected float TimeToNextCheck;
        protected bool Triggered;

        protected override void Update()
        {
            if (!Triggered && TimeToNextCheck <= 0f)
            {
                Players = Utils.GetAllLivingPlayers();
                Enemies = new List<EnemyAI>(RoundManager.Instance.SpawnedEnemies);
                Enemies.RemoveAll(enemy => enemy.isOutside || enemy.isEnemyDead || !enemy.agent.isOnNavMesh);
                
                if (Players.Count <= Enemies.Count)
                {
                    Triggered = true;
                    StartCoroutine(SearchForReplacements());
                }
                
                TimeToNextCheck = TimeBetweenChecks;
            }
            TimeToNextCheck -= Time.deltaTime;
        }

        protected IEnumerator SearchForReplacements()
        {
            while (Players.Count > 0)
            {
                PlayerControllerB player = Players[0];
                int enemyIndex = Random.Range(0, Enemies.Count);
                        
                Vector3 position = player.transform.position;
                        
                EnemyAI enemy = Enemies[enemyIndex];
                        
                if (player.isInsideFactory == enemy.isOutside)
                    enemy.StartCoroutine(Utils.DelayedSetOutside(enemyAI: enemy,
                        isOutside: !player.isInsideFactory));
                        
                EventManager.Instance.TeleportPlayerClientRPC(
                    new NetworkObjectReference(player.NetworkObject),
                    enemy.transform.position);
                        
                EventManager.Instance.TeleportEntityClientRPC(
                    new NetworkObjectReference(enemy.NetworkObject),
                    position);
            }

            Destroy(this);
            yield break;
        }
    }
}
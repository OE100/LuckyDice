using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using LuckyDice.custom.monobehaviour.attributes;
using LuckyDice.custom.monobehaviour.def;
using LuckyDice.custom.network;
using LuckyDice.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.monobehaviour.impl.map
{
    [NeedsRemoval]
    public class TeleportAllPlayers : BaseEventBehaviour
    {
        protected List<PlayerControllerB> players;
        protected List<EnemyAI> enemies;
        protected float timeBetweenChecks = 5f;
        protected float timeToNextCheck;
        protected bool triggered;

        protected override void Update()
        {
            if (!triggered && timeToNextCheck <= 0f)
            {
                players = Utils.GetAllLivingPlayers();
                enemies = new List<EnemyAI>(RoundManager.Instance.SpawnedEnemies);
                enemies.RemoveAll(enemy => enemy.isOutside || enemy.isEnemyDead || !enemy.agent.isOnNavMesh);
                
                if (players.Count <= enemies.Count)
                {
                    triggered = true;
                    StartCoroutine(SearchForReplacements());
                }
                
                timeToNextCheck = timeBetweenChecks;
            }
            timeToNextCheck -= Time.deltaTime;
        }

        protected IEnumerator SearchForReplacements()
        {
            while (players.Count > 0)
            {
                PlayerControllerB player = players[0];
                int enemyIndex = Random.Range(0, enemies.Count);
                        
                Vector3 position = player.transform.position;
                        
                EnemyAI enemy = enemies[enemyIndex];
                        
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
using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using LuckyDice.Patches.custom.network;
using UnityEngine;

namespace LuckyDice.Patches.custom.events
{
    public class BleedEvent : DiceEvent
    {
        private List<PlayerControllerB> playersAffected = new List<PlayerControllerB>();
        private bool running = false;
        
        public void AddPlayer(PlayerControllerB player)
        {
            playersAffected.Add(player);
            player.bleedingHeavily = true;
        }

        public void RemovePlayer(PlayerControllerB player)
        {
            playersAffected.Remove(player);
            player.bleedingHeavily = false;
        }

        public void Run()
        {
            running = true;
            EventManager.Instance.StartCoroutine(EventCoroutine());
        }

        public void Stop()
        {
            running = false;
        }

        public IEnumerator EventCoroutine()
        {
            while (running)
            {
                foreach (PlayerControllerB player in playersAffected)
                {
                    player.DamagePlayer(damageNumber: 1, hasDamageSFX: false);
                }
                yield return new WaitForSeconds(2.4f);
            }
        }
    }
}
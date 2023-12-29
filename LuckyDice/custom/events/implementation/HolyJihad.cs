using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using LuckyDice.custom.events.prototype;
using LuckyDice.custom.network;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.events.implementation
{
    public class HolyJihad : MultiplierDiceEvent<IEnumerator>
    {
        public override void AddPlayer(PlayerControllerB player)
        {
            if (playersToMult.ContainsKey(player))
                return;
            player.StartCoroutine(JihadCoroutine(player, Random.Range(5f, 10f)));
        }

        public override void RemovePlayer(PlayerControllerB player)
        {
            playersToMult.Remove(player);
        }

        public override IEnumerator EventCoroutine()
        {
            while (running)
            {
                if (IsPhaseForbidden())
                {
                    if (playersToMult.Count > 0)
                    {
                        foreach (KeyValuePair<PlayerControllerB, IEnumerator> pair in playersToMult)
                            pair.Key.StopCoroutine(pair.Value);
                        playersToMult.Clear();
                    }
                }

                yield return new WaitForSeconds(2);
            }
        }

        private IEnumerator JihadCoroutine(PlayerControllerB player, float time)
        {
            yield return new WaitForSeconds(time);
            EventManager.Instance.SpawnExplosionOnPlayerClientRPC(new NetworkObjectReference(player.GetComponentInParent<NetworkObject>()));
        }
    }
}
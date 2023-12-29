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
        internal static AudioClip jihadClip = null;
        internal static AudioClip beepClip = null;
        
        public override void Run()
        {
            base.Run();
            if (jihadClip == null)
                jihadClip = Plugin.ab.LoadAsset<AudioClip>("Assets/custom/luckydice/sounds/jihad_explode.ogg");
            if (beepClip == null)
                beepClip = Plugin.ab.LoadAsset<AudioClip>("Assets/custom/luckydice/sounds/jihad_beep.mp3");
        }

        public override void AddPlayer(PlayerControllerB player)
        {
            if (playersToMult.ContainsKey(player))
                return;
            player.StartCoroutine(JihadCoroutine(player, Random.Range(20f, 25f)));
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
            yield return new WaitForSeconds(time - 10);
            EventManager.Instance.DisplayMessageClientRPC(
                new NetworkObjectReference(player.GetComponentInParent<NetworkObject>()),
                "Alahu Akbar!",
                "Prepare for holy jihad!"
                );
            yield return new WaitForSeconds(10);
            EventManager.Instance.SpawnExplosionOnPlayerClientRPC(new NetworkObjectReference(player.GetComponentInParent<NetworkObject>()));
        }
    }
}
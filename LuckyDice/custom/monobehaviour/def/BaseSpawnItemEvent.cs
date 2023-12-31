﻿using System.Collections;
using GameNetcodeStuff;
using LuckyDice.custom.monobehaviour.attributes;
using LuckyDice.custom.network;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.monobehaviour.def
{
    [MountAtRegistry]
    public abstract class BaseSpawnItemEvent : BasePlayerEvent
    {
        protected abstract int AmountPerStack();
        protected abstract int ItemId();
        protected abstract int ItemValue();
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
            StartCoroutine(SpawnItemsAroundPlayer(player));
        }

        private IEnumerator SpawnItemsAroundPlayer(PlayerControllerB player)
        {
            yield return new WaitForSeconds(5);

            int count = AmountPerStack();
            
            while (count > 0)
            {
                if (IsPhaseForbidden())
                    break;
                
                Vector3 randomPos = Utilities.Utils.GetRandomLocationAroundPosition(
                    player.transform.position,
                    radius: 5,
                    randomHeight: true);

                bool found = Utilities.Utils.ClosestNavMeshToPosition(
                    randomPos,
                    out var closestPoint);

                if (!found)
                    continue;
                                
                EventManager.Instance.SpawnItemAroundPositionServerRPC(
                    position: closestPoint,
                    itemId: ItemId(),
                    stackValue: ItemValue());
                                
                count--;
            }

        }
    }
}
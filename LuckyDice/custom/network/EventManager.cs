using System.Linq;
using GameNetcodeStuff;
using LuckyDice.custom.events;
using LuckyDice.custom.events.prototype;
using Unity.Netcode;
using UnityEngine;
using Event = LuckyDice.custom.network.Event;

namespace LuckyDice.custom.network
{
    public class EventManager : NetworkBehaviour
    {
        public static EventManager Instance { get; private set; }
        private readonly IDiceEvent[] Events = {
            new Bleed(),
            new SpawnCoilhead()
        };
        
        public override void OnNetworkSpawn()
        {
            // initialize singleton
            base.OnNetworkSpawn();
            Instance = this;
            
            // todo: find coilhead enemy for event
            
            // start all events
            foreach (IDiceEvent e in Events)
            {
                e.Run();
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void StartEventServerRPC(Event e)
        {
            StartEventClientRPC(e);
        }
        
        [ClientRpc]
        public void StartEventClientRPC(Event e)
        {
            Events[(int)e].Run();
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void StopEventServerRPC(Event e)
        {
            StopEventClientRPC(e);
        }
        
        [ClientRpc]
        public void StopEventClientRPC(Event e)
        {
            Events[(int)e].Stop();
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void AddPlayerToEventServerRPC(Event e, ulong steamId)
        {
            AddPlayerToEventClientRPC(e, steamId);
        }
        
        [ClientRpc]
        public void AddPlayerToEventClientRPC(Event e, ulong steamId)
        {
            PlayerControllerB player = StartOfRound.Instance.allPlayerScripts.First(p => p.playerSteamId == steamId);
            Events[(int)e].AddPlayer(player);
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void RemovePlayerFromEventServerRPC(Event e, ulong steamId)
        {
            RemovePlayerFromEventClientRPC(e, steamId);
        }
        
        [ClientRpc]
        public void RemovePlayerFromEventClientRPC(Event e, ulong steamId)
        {
            PlayerControllerB player = StartOfRound.Instance.allPlayerScripts.First(p => p.playerSteamId == steamId);
            Events[(int)e].RemovePlayer(player);
        }
    }
}
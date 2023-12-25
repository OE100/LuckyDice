using System.Linq;
using GameNetcodeStuff;
using LuckyDice.Patches.custom.events;
using Unity.Netcode;

namespace LuckyDice.Patches.custom.network
{
    public class EventManager : NetworkBehaviour
    {
        public static EventManager Instance { get; private set; }
        public readonly DiceEvent[] Events = {
            new BleedEvent()
        };
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Instance = this;
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
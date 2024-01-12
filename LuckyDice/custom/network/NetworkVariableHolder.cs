using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.network;

public class NetworkVariableHolder : NetworkBehaviour
{
    public static NetworkVariableHolder Instance { get; private set; }

    public NetworkVariable<int> WeatherCredits = new();
    public NetworkVariable<float> TimeUntilNextWarning = new();

    public override void OnNetworkSpawn()
    {
        Instance = this;
        
        WeatherCredits.Value = 0;
        TimeUntilNextWarning.Value = 0f;
    }

    private void Update()
    {
        if (!(NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost))
            return;
        
        if (TimeUntilNextWarning.Value > 0)
            TimeUntilNextWarning.Value -= Time.deltaTime;
    }
}
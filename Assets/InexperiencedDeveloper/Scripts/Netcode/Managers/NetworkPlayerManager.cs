using InexperiencedDeveloper.Network.Core;
using InexperiencedDeveloper.Utils.Log;
using System.Collections.Generic;
using Unity.Netcode;

public class NetworkPlayerManager : NetworkSingleton<NetworkPlayerManager>
{
    private List<ulong> playersInGame = new List<ulong>();

    public List<ulong> PlayersInGame => playersInGame;

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    //private void OnDisable()
    //{
    //    NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    //    NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    //}

    private void OnClientConnected(ulong id)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            playersInGame.Add(id);
            DebugLogger.Log($"Player {id} just connected.");
        }
    }

    private void OnClientDisconnected(ulong id)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            playersInGame.Remove(id);
            DebugLogger.Log($"Player {id} just disconnected.");
        }
    }
}

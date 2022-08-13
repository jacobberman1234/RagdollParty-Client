using InexperiencedDeveloper.Network.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using System.Linq;
using UnityEngine;

public class ServerReconciliation : NetworkSingleton<ServerReconciliation>
{
    private List<ulong> playersInGame = new List<ulong>();
    private Dictionary<ulong, Dictionary<int, Vector3>> positionBuffer;
    public static int BUFFER_SIZE = 1024;
    private int currTick = 0;

    private void Update()
    {
        if (IsServer)
        {
            if (playersInGame != NetworkPlayerManager.Instance.PlayersInGame)
            {
                playersInGame = NetworkPlayerManager.Instance.PlayersInGame;
            }
            foreach (var player in playersInGame)
            {
                var client = NetworkManager.Singleton.ConnectedClients[player];
                Dictionary<int, Vector3> currPos = new Dictionary<int, Vector3>();
                if (positionBuffer.ContainsKey(player))
                {
                    if (positionBuffer[player].ContainsKey(currTick))
                    {
                        positionBuffer[player].Add(currTick, client.PlayerObject.transform.position);
                    }
                }
                else
                {
                    Dictionary<int, Vector3> dict = new Dictionary<int, Vector3>();
                    dict.Add(currTick, client.PlayerObject.transform.position);
                    positionBuffer.Add(player, dict);
                }
            }
        }

    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            currTick++;
            if (currTick >= BUFFER_SIZE)
                currTick = 0;
        }

    }

    public bool ValidatePosition(ulong playerId, int tick, Vector3 pos, out Vector3 realPos)
    {
        if (positionBuffer.ContainsKey(playerId))
        {
            if (positionBuffer[playerId].ContainsKey(tick))
            {
                if (positionBuffer[playerId][tick] == pos)
                {
                    realPos = Vector3.zero;
                    return true;
                }
                else
                {
                    realPos = positionBuffer[playerId][tick];
                    return false;
                }
            }
            else
            {
                var lastPos = positionBuffer[playerId].Values.Last();
                realPos = lastPos;
                return false;
            }
        }
        else
        {
            realPos = Vector3.zero;
            return false;
        }

    }
}

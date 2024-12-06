using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.Experimental.RestService;
using UnityEngine;

public class NetworkedPlayerData : NetworkBehaviour, IDisposable
{
    public NetworkList<PlayerInfoData> connectedPlayers;
    private int playerCount = 0;
    private ulong serverLocalId;

    private Color[] playerColorDefaults =
    {
        Color.blue, Color.magenta, Color.cyan, Color.yellow, Color.white
    };

    private void Awake()
    {
        // Initialized here to avoid memory leaks
        connectedPlayers = new NetworkList<PlayerInfoData>(readPerm: NetworkVariableReadPermission.Everyone);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer) return;

        NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvents;
        serverLocalId = NetworkManager.ServerClientId;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        
        NetworkManager.Singleton.OnConnectionEvent -= OnConnectionEvents;

        base.OnNetworkDespawn();
    }

    private void OnConnectionEvents(NetworkManager networkManager, ConnectionEventData eventData)
    {
        if (eventData.EventType == ConnectionEvent.ClientConnected)
        {
            CreatePlayerData(eventData.ClientId);
        }
        
        if (eventData.EventType == ConnectionEvent.ClientDisconnected)
        {
            RemovePlayerData(connectedPlayers[FindPlayerInfoDataIndex(eventData.ClientId)]);
        }
    }

    private void CreatePlayerData(ulong clientId)
    {
        playerCount++;
        PlayerInfoData playerInfoData = new PlayerInfoData(clientId);

        // If host, assume ready
        playerInfoData._isPlayerReady = serverLocalId == clientId;
        playerInfoData._name = $"Player {playerCount}";
        playerInfoData._color = playerColorDefaults[(playerCount - 1) % playerColorDefaults.Length];

        connectedPlayers.Add(playerInfoData);
    }

    private void RemovePlayerData(PlayerInfoData playerData)
    {
       bool foundPlayer = connectedPlayers.Remove(playerData);

       if (foundPlayer) playerCount--;
    }

    private int FindPlayerInfoDataIndex(ulong clientId)
    {
        for (int i = 0; i < connectedPlayers.Count; i++)
        {
            if (connectedPlayers[i]._clientId == clientId) return i;
        }

        return -1;
    }

    public void UpdateClientStatus(ulong clientId, bool isReady)
    {
        int playerIndex = FindPlayerInfoDataIndex(clientId);

        if (playerIndex == -1) return;

        PlayerInfoData foundPlayer = connectedPlayers[playerIndex];

        foundPlayer._isPlayerReady = isReady;

        connectedPlayers[playerIndex] = foundPlayer;
    }

    public string getPlayerName(ulong playerId)
    {
        int playerIndex = FindPlayerInfoDataIndex(playerId);
        
        if (playerIndex == -1) return "Unknown";

        return connectedPlayers[playerIndex]._name.ToString();
    }

    public void Dispose()
    {
        connectedPlayers?.Dispose();
    }

    public override void OnDestroy()
    {
        connectedPlayers?.Dispose();
        base.OnDestroy();
    }
}

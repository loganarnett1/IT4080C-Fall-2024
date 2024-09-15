using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;

public class SpawnController : NetworkBehaviour
{
    [SerializeField]
    private NetworkObject _playerPrefab;

    [SerializeField]
    private Transform[] _spawnPoints;

    [SerializeField]
    private TMP_Text _countTxt;

    private NetworkVariable<int> _playerCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer) {
            NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvent;
        }
        _playerCount.OnValueChanged += PlayerCountChanged;

    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (IsServer) {
            NetworkManager.Singleton.OnConnectionEvent -= OnConnectionEvent;
        }
        _playerCount.OnValueChanged -= PlayerCountChanged;
    }

    private void OnConnectionEvent(NetworkManager netManager, ConnectionEventData eventData)
    {
        if (eventData.EventType == ConnectionEvent.ClientConnected)
        {
            _playerCount.Value++;
        }
    }

    private void PlayerCountChanged(int prevVal, int newVal) => UpdateCountTextRPC(newVal);

    [Rpc(SendTo.Everyone)]
    private void UpdateCountTextRPC(int newValue) => _countTxt.text = $"Players: {newValue}";

    public void SpawnAllPlayers()
    {
        if (!IsServer) return;

        int spawnIndex = 0;
        foreach(ulong clientId in NetworkManager.ConnectedClientsIds)
        {
            NetworkObject spawnedPlayerNO = NetworkManager.Instantiate(
                _playerPrefab,
                _spawnPoints[spawnIndex].position,
                _spawnPoints[spawnIndex].rotation
            );

            spawnedPlayerNO.SpawnAsPlayerObject(clientId);

            spawnIndex = (spawnIndex + 1) % _spawnPoints.Length;
        }
    }
}

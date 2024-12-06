using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private Button startButton, leaveButton, readyButton;
    [SerializeField] private GameObject socialPanel;
    [SerializeField] private GameObject playerRowPrefab;
    [SerializeField] private GameObject playerListWrapper;
    [SerializeField] private TMP_Text statusText, readyButtonText;
    [SerializeField] private NetworkedPlayerData networkPlayers;

    private List<GameObject> playerInfoRows = new List<GameObject>();
    private ulong serverId;
    private bool isReady = false;

    private void Start()
    {
        networkPlayers.connectedPlayers.OnListChanged += NetPlayersChanged;
        readyButton.onClick.AddListener(ClientReadyClick);
        leaveButton.onClick.AddListener(ClientLeaveClick);
    }

    // Initialization to be called after client/server is connected
    public void Init()
    {
        serverId = NetworkManager.ServerClientId;

        if (IsServer)
        {
            statusText.text = "Waiting for Players";
            readyButton.gameObject.SetActive(false);
            startButton.gameObject.SetActive(true);
        }
        else
        {
            statusText.text = "Not Ready";
            readyButton.gameObject.SetActive(true);
            startButton.gameObject.SetActive(false);
        }
    }

    private void NetPlayersChanged(NetworkListEvent<PlayerInfoData> changeEvent)
    {
        PopulatePlayerPanel();
    }

    private void ClientReadyClick()
    {
        if (IsServer) return;

        isReady = !isReady;
        if (isReady)
        {
            statusText.text = "Ready";
            readyButtonText.text = "Un-Ready";
        }
        else
        {
            statusText.text = "Not Ready";
            readyButtonText.text = "Ready";
        }
        
        ClientIsReadyRpc(isReady);
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void ClientIsReadyRpc(bool isReady, RpcParams rpcParams = default)
    {
        networkPlayers.UpdateClientStatus(rpcParams.Receive.SenderClientId, isReady);
    }

    private void ClientLeaveClick()
    {
        if (!IsServer)
        {
            QuitLobbyServerRpc();
        }
        else
        {
            foreach (PlayerInfoData playerData in networkPlayers.connectedPlayers)
            {
                if (playerData._clientId != serverId)
                {
                    KickUser(playerData._clientId);
                }
            }

            NetworkManager.Shutdown();
            SceneManager.LoadScene(0);
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void QuitLobbyServerRpc(RpcParams rpcParams = default)
    {
        KickUser(rpcParams.Receive.SenderClientId);
    }

    private void KickUser(ulong kickTarget)
    {
        if (!IsServer || !IsHost) return;

        foreach (PlayerInfoData playerData in networkPlayers.connectedPlayers)
        {
            if (playerData._clientId == kickTarget)
            {
                KickClientRpc(RpcTarget.Single(kickTarget, RpcTargetUse.Temp));

                NetworkManager.Singleton.DisconnectClient(kickTarget);
            }
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void KickClientRpc(RpcParams rpcParams)
    {
        SceneManager.LoadScene(0);
    }

    [ContextMenu("Generate Player List")]
    private void PopulatePlayerPanel()
    {
        ClearPlayerPanel();

        bool allReady = true;

        foreach (PlayerInfoData playerData in networkPlayers.connectedPlayers)
        {
            GameObject newPlayerRow = Instantiate(playerRowPrefab, playerListWrapper.transform);
            PlayerLabel playerLabel = newPlayerRow.GetComponent<PlayerLabel>();

            playerLabel.onKick += KickUser;

            if (IsServer && playerData._clientId != serverId)
            {
                playerLabel.SetKickActive(true);
            }
            else
            {
                // Clients don't have access to kick
                playerLabel.SetKickActive(false);
            }

            playerLabel.SetPlayerLabelName(playerData._clientId, playerData._name.ToString());
            playerLabel.SetPlayerStatus(playerData._isPlayerReady);
            playerLabel.SetPlayerColor(playerData._color);
            
            playerInfoRows.Add(newPlayerRow);

            if (!playerData._isPlayerReady) allReady = false;
        }

        if (IsServer)
        {
            if (allReady)
            {
                if (networkPlayers.connectedPlayers.Count > 1)
                {
                    statusText.text = "Ready to start";
                    startButton.gameObject.SetActive(true);
                }
                else
                {
                    statusText.text = "Empty Lobby";
                }
            }
            else
            {
                statusText.text = "Waiting for ready players";
                startButton.gameObject.SetActive(false);
            }
        }
    }

    private void ClearPlayerPanel()
    {
        foreach(GameObject playerRow in playerInfoRows)
        {
            Destroy(playerRow);
        }

        playerInfoRows.Clear();
    }
}

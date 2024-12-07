using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using App.Scripts.GameLogic;
using System;

namespace App.Scripts.UI
{
    public class UI_NetManager : NetworkBehaviour
    {
        [SerializeField]
        private Button _serverBtn, _hostBtn, _clientBtn, _startBtn;

        [SerializeField]
        private GameObject _connectionBtnGroup, _socialPanel;

        [SerializeField]
        private SpawnController _spawnController;

        [SerializeField]
        private LobbyManager _lobbyManager;

        [SerializeField]
        private ChatNetworkManager chatNetworkManager;

        void Start()
        {
            _serverBtn?.onClick.AddListener(ServerClick);
            _hostBtn?.onClick.AddListener(HostClick);
            _clientBtn?.onClick.AddListener(ClientClick);
            _startBtn?.onClick.AddListener(StartClick);
            _socialPanel?.gameObject.SetActive(false);
        }

        private void ServerClick()
        {
            NetworkManager.Singleton.StartServer();
            _lobbyManager.Init();
            _connectionBtnGroup.SetActive(false);
            _socialPanel?.gameObject.SetActive(true);
            chatNetworkManager.RefreshMessagesPanel();
        }

        private void HostClick()
        {
            NetworkManager.Singleton.StartHost();
            _lobbyManager.Init();
            _connectionBtnGroup.SetActive(false);
            _socialPanel?.gameObject.SetActive(true);
            chatNetworkManager.RefreshMessagesPanel();
        }

        private void ClientClick()
        {
            NetworkManager.Singleton.StartClient();
            _lobbyManager.Init();
            _connectionBtnGroup.SetActive(false);
            _socialPanel?.gameObject.SetActive(true);
            chatNetworkManager.RefreshMessagesPanel();
        }

        private void StartClick()
        {
            _spawnController.SpawnAllPlayers();
            HideGuiRpc();
        }

        [Rpc(SendTo.Everyone)]
        private void HideGuiRpc()
        {
            _socialPanel.SetActive(false);
        }
    }
}

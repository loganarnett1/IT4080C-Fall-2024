using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using App.Scripts.GameLogic;

namespace App.Scripts.UI
{
    public class UI_NetManager : NetworkBehaviour
    {
        [SerializeField]
        private Button _serverBtn, _hostBtn, _clientBtn, _startBtn;

        [SerializeField]
        private GameObject _connectionBtnGroup;

        [SerializeField]
        private SpawnController _spawnController;

        void Start()
        {
            _startBtn?.gameObject.SetActive(false);
            _serverBtn?.onClick.AddListener(ServerClick);
            _hostBtn?.onClick.AddListener(HostClick);
            _clientBtn?.onClick.AddListener(ClientClick);
            _startBtn?.onClick.AddListener(StartClick);
        }

        private void ServerClick()
        {
            NetworkManager.Singleton.StartServer();
            _connectionBtnGroup.SetActive(false);
            _startBtn?.gameObject.SetActive(true);
        }

        private void HostClick()
        {
            NetworkManager.Singleton.StartHost();
            _connectionBtnGroup.SetActive(false);
            _startBtn?.gameObject.SetActive(true);
        }

        private void ClientClick()
        {
            NetworkManager.Singleton.StartClient();
            _connectionBtnGroup.SetActive(false);
        }

        private void StartClick()
        {
            _spawnController.SpawnAllPlayers();
            _startBtn?.gameObject.SetActive(false);
        }
    }
}

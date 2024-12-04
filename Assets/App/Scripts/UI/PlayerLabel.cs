using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLabel : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerLabel;
    [SerializeField] private Button _kickButton;
    [SerializeField] private Image _playerStatusImage, _playerColorImage;

    public event Action<ulong> onKick;
    private ulong _clientId;


    private void OnEnable()
    {
        _kickButton.onClick.AddListener(OnKickButtonClicked);
    }

    public void SetPlayerLabelName(ulong playerName)
    {
        _clientId = playerName;
        _playerLabel.text = $"Player {playerName.ToString()}";
    }

    public void OnKickButtonClicked()
    {
        onKick?.Invoke(_clientId);
    }

    public void SetKickActive(bool isOn)
    {
        _kickButton.gameObject.SetActive(isOn);
    }

    public void SetPlayerStatus(bool ready)
    {
        _playerStatusImage.color = ready ? Color.green : Color.red;
    }

    public void SetPlayerColor(Color color)
    {
        _playerColorImage.color = color;
    }
}

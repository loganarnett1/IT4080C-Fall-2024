using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChatNetworkManager : NetworkBehaviour, IDisposable
{
    public NetworkList<Message> messages;
    [SerializeField] private TMP_Text chatText;
    [SerializeField] private NetworkedPlayerData networkPlayerData;
    [SerializeField] private Button sendMessageButton;
    [SerializeField] private TMP_InputField messageInputField;

    private void Awake()
    {
        messages = new NetworkList<Message>(readPerm: NetworkVariableReadPermission.Everyone);
        messages.OnListChanged += OnMessagesUpdated;
    } 

    private void OnMessagesUpdated(NetworkListEvent<Message> changeEvent)
    {
        RefreshMessagesPanel();
    }

    void Start()
    {
        sendMessageButton.onClick.AddListener(OnSendMessageClick);
    }

    private void OnSendMessageClick()
    {
        Debug.Log($"in here {messageInputField.text}");

        if (messageInputField.text == "") return;

        SendMessageRpc(NetworkManager.LocalClientId, messageInputField.text);

        messageInputField.text = "";
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void SendMessageRpc(ulong clientId, string message)
    {
        if (!IsServer) return;
        Message createdMessage = new Message(clientId, message);

        messages.Add(createdMessage);
    }

    private void ClearMessages(PlayerInfoData playerData) => messages.Clear();

    public void RefreshMessagesPanel()
    {
        string messageString = "";

        foreach(Message message in messages)
        {
            messageString = $"{networkPlayerData.getPlayerName(message.clientId)}: {message.message}\n{messageString}";
        }

        chatText.text = messageString;
    }

    public void Dispose()
    {
        messages?.Dispose();
    }

    public override void OnDestroy()
    {
        messages?.Dispose();
        base.OnDestroy();
    }
}
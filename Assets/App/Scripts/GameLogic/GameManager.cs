using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : NetworkBehaviour
{
    private List<ulong> currentPlayers;
    public TMP_Text gameEndingText;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        currentPlayers = new List<ulong>();

        gameEndingText.gameObject.SetActive(false);
    }

    [Rpc(SendTo.Server)]
    public void AddPlayerRpc(ulong playerId)
    {
        currentPlayers.Add(playerId);
    }

    [Rpc(SendTo.Server)]
    public void RemovePlayerRpc(ulong playerId)
    {
        currentPlayers.Remove(playerId);
    }

    [Rpc(SendTo.Server)]
    public void PlayerDeathRpc(RpcParams rpcParams = default)
    {
        currentPlayers.Remove(rpcParams.Receive.SenderClientId);

        YouLoseRpc(RpcTarget.Single(rpcParams.Receive.SenderClientId, RpcTargetUse.Temp));

        if (currentPlayers.Count == 1)
        {
            YouWinRpc(RpcTarget.Single(currentPlayers[0], RpcTargetUse.Temp));
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void YouLoseRpc(RpcParams rpcParams = default)
    {
        gameEndingText.text = "You Lost";
        gameEndingText.gameObject.SetActive(true);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void YouWinRpc(RpcParams rpcParams)
    {
        gameEndingText.text = "You Win";
        gameEndingText.gameObject.SetActive(true);
    }
}

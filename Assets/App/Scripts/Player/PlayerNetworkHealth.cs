using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNetworkHealth : NetworkBehaviour
{
    [SerializeField] private float initialHealth = 100f;
    [SerializeField] private float cooldownTimeSeconds = .5f;
    [SerializeField] private Image healthBar;

    private bool canTakeDamage = true;
    private GameManager gameManager;
    private NetworkVariable<float> health = new NetworkVariable<float>(100);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        health.Value = initialHealth;

        health.OnValueChanged += UpdateHealth;
        gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
    }

    private void UpdateHealth(float previousValue, float newValue)
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = newValue / initialHealth;
        }

        if (IsOwner && newValue < 0f)
        {
            gameManager.PlayerDeathRpc();
            HasDiedRpc();
        }
    }

    [Rpc(SendTo.Server)]
    private void HasDiedRpc()
    {
        NetworkObject.Despawn();
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void DamagePlayerRpc(float damage)
    {
        if (!canTakeDamage) return;
        health.Value -= damage;
        StartCoroutine(nameof(DamageCooldown));
    }

    private IEnumerator DamageCooldown()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(cooldownTimeSeconds);
        canTakeDamage = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<int> ammo = new NetworkVariable<int>(4);
    [SerializeField] private Transform startingPoint;
    [SerializeField] private NetworkObject projectilePrefab;

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void FireProjectileRpc(ulong senderClientId)
    {
        if (ammo.Value > 0)
        {
            NetworkObject newProjectile = NetworkManager.Instantiate(projectilePrefab, startingPoint.position, startingPoint.rotation);

            Debug.Log($"Owner of thing: {senderClientId}");

            newProjectile.SpawnWithOwnership(senderClientId);

            ammo.Value--;
        }
    }

    public void AddAmmo(int amount)
    {
        ammo.Value++;
    }
}

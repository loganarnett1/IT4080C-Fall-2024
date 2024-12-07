using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

public class AmmoSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkObject ammo;

    [SerializeField] private float tickTime = 6f;
    [SerializeField] private float currentTime = 6f;

    public void FixedUpdate()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            SpawnAmmoRpc();

            currentTime = Random.Range(tickTime, tickTime + 5f);
        }

    }

    [Rpc(SendTo.Server)]
    public void SpawnAmmoRpc()
    {
        NetworkObject ammoSpawned = NetworkManager.Instantiate(ammo, transform.position, transform.rotation);
        ammoSpawned.Spawn(true);      
    }
}

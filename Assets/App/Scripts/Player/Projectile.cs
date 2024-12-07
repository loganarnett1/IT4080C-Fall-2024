using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SerializeField] float speed = 40f;
    [SerializeField] private float damage = 10;
    [SerializeField] private float projectileLifetimeSeconds = 5f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        GetComponent<Rigidbody>().velocity = this.transform.forward * speed;
        StartCoroutine(AutoDestruct());
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.Equals("Player") && other.gameObject.GetComponent<NetworkObject>().OwnerClientId != this.OwnerClientId)
        {
            other.gameObject.GetComponent<PlayerNetworkHealth>().DamagePlayerRpc(damage);
            this.NetworkObject.Despawn();
        }
    }

    private IEnumerator AutoDestruct()
    {
        yield return new WaitForSeconds(projectileLifetimeSeconds);
        this.NetworkObject.Despawn();
    }
}

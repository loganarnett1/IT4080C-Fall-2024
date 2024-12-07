using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Ammo : NetworkBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            BulletSpawner spawner = other.gameObject.GetComponent<BulletSpawner>();

            if (spawner == null)
            {
                spawner = other.gameObject.GetComponentInParent<BulletSpawner>();
            }

            spawner.AddAmmo(1);
            
            this.NetworkObject.Despawn();
        }
    }
}

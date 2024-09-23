using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace App.Scripts.Player
{
    public class PlayerMovement : NetworkBehaviour
    {
        [SerializeField]
        float moveSpeed = 3f;

        void Update()
        {
            if(!IsOwner) return;

            Vector3 moveDirection = new Vector3(0, 0, 0);
            if (Input.GetKey(KeyCode.W)) moveDirection.z = +1f;
            if (Input.GetKey(KeyCode.S)) moveDirection.z = -1f;
            if (Input.GetKey(KeyCode.A)) moveDirection.x = -1f;
            if (Input.GetKey(KeyCode.D)) moveDirection.x = +1f;

            transform.position += moveDirection * (moveSpeed * Time.deltaTime);
        }
    }
}

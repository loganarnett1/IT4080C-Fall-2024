using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace App.Scripts.Player
{
    public class PlayerMovement : NetworkBehaviour
    {
        [SerializeField] float moveSpeed = 3f;
        [SerializeField] float moveSpeedSprint = 6f;
        [SerializeField] private Animator playerAnimator;
        [SerializeField] private OwnerNetworkAnimator networkAnimator;


        private void Awake()
        {
            if (playerAnimator == null)
            {
                playerAnimator = gameObject.GetComponent<Animator>();
            }

            if (networkAnimator == null)
            {
                networkAnimator = gameObject.GetComponent<OwnerNetworkAnimator>();
            }
        }

        void Update()
        {
            if(!IsOwner) return;

            Vector3 moveDirection = new Vector3(0, 0, 0);
            if (Input.GetKey(KeyCode.W)) moveDirection.z = +1f;
            if (Input.GetKey(KeyCode.S)) moveDirection.z = -1f;
            if (Input.GetKey(KeyCode.A)) moveDirection.x = -1f;
            if (Input.GetKey(KeyCode.D)) moveDirection.x = +1f;

            moveDirection.Normalize();

            bool isWalking = moveDirection.x != 0 || moveDirection.y != 0;
            bool isSprinting = Input.GetKey(KeyCode.LeftShift) && isWalking;

            playerAnimator.SetBool("IsWalking", isWalking);
            playerAnimator.SetBool("IsSprinting", isSprinting);
            if (Input.GetKey(KeyCode.Space)) networkAnimator.SetTrigger("JumpTrigger");
            if (Input.GetKey(KeyCode.Z)) networkAnimator.SetTrigger("PunchTrigger");

            transform.position += moveDirection * ((isSprinting ? moveSpeedSprint : moveSpeed) * Time.deltaTime);
            if (isWalking) transform.forward = moveDirection;
        }
    }
}

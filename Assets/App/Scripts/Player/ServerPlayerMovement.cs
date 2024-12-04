using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEditor.ShortcutManagement;
using UnityEngine.EventSystems;
using System.Linq;
using Unity.Netcode.Components;
using TMPro;

namespace App.Scripts.Player
{
    public class ServerPlayerMovement : NetworkBehaviour
    {
        [SerializeField] private float movementSpeed;
        [SerializeField] private float movementSpeedRunning;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private NetworkAnimator networkAnimator;
        [SerializeField] private Animator playerAnimator;

        public CharacterController characterController;

        private PlayerInputActions playerInput;

        void Start()
        {
            playerInput = new PlayerInputActions();
            playerInput.Enable();
        }

        void Update()
        {
            if (!IsOwner) return;
            
            Vector2 moveInput = playerInput.Player.Movement.ReadValue<Vector2>();

            bool isJumping = playerInput.Player.Jump.triggered;
            bool isPunching = playerInput.Player.Punch.triggered;
            bool isSprinting = playerInput.Player.Sprint.ReadValue<float>() > 0;
            
            if (IsServer)
            {
                Move(moveInput, Time.deltaTime, isJumping, isPunching, isSprinting);
            }
            else if (IsClient) 
            {
                MoveServerRPC(moveInput, Time.deltaTime, isJumping, isPunching, isSprinting);
            }
        }

        private void Move(Vector2 input, float deltaTime, bool isJumping, bool isPunching, bool isSprinting)
        {
            Vector3 movementDirection = input.x * playerTransform.right + input.y * playerTransform.forward;

            bool isWalking = input.x != 0 || input.y != 0;

            playerAnimator.SetBool("IsWalking", isWalking);
            playerAnimator.SetBool("IsSprinting", isSprinting && isWalking);
            if (isJumping) networkAnimator.SetTrigger("JumpTrigger");
            if (isPunching) networkAnimator.SetTrigger("PunchTrigger");

            characterController.Move(movementDirection * deltaTime * (isSprinting ? movementSpeedRunning : movementSpeed));
        }

        [Rpc(target:SendTo.Server)]
        private void MoveServerRPC(Vector2 input, float deltaTime, bool isJumping, bool isPunching, bool isSprinting) =>
            Move(input, deltaTime, isJumping, isPunching, isSprinting);
    }
}

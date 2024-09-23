using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEditor.ShortcutManagement;
using UnityEngine.EventSystems;

namespace App.Scripts.Player
{
    public class ServerPlayerMovement : NetworkBehaviour
    {
        [SerializeField] private float movementSpeed;
        [SerializeField] private Transform playerTransform;

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
            
            if (IsServer)
            {
                Move(moveInput, Time.deltaTime);
            }
            else if (IsClient) 
            {
                MoveServerRPC(moveInput, Time.deltaTime);
            }
        }

        private void Move(Vector2 input, float deltaTime)
        {
            Vector3 movementDirection = input.x * playerTransform.right + input.y * playerTransform.forward;

            characterController.Move(movementDirection * movementSpeed * deltaTime);
        }

        [Rpc(target:SendTo.Server)]
        private void MoveServerRPC(Vector2 input, float deltaTime) => Move(input, deltaTime);
    }
}

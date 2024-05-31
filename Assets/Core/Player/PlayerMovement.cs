//Copyright takiido. All Rights Reserved.

using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public float fwdSpeed = 10.0f;
        public float laneChangeSpeed = 10.0f;
        public float jumpForce = 10.0f;
        public float slideDuration = 1.0f;

        private Rigidbody _rb;
        private bool _isJumping = false;
        private bool _isSliding = false;
        private float _sliderTimer;

        private int _curLane = 1;
        private Vector3 _targetPos;

        private InputAction _moveActions;

        private void Awake()
        {
            InputActionAsset inputAction = GetComponent<PlayerInput>().actions;

            _moveActions = inputAction.FindAction("Move");
        }

        private void OnEnable()
        {
            _moveActions.Enable();
        }

        private void OnDisable()
        {
            _moveActions.Disable();
        }

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _targetPos = transform.position;
        }
    
        private void Update()
        {
            //if (GameManager.Instance.isGameOver) return;
            HandleInput();
            MoveFwd();
        }

        private void HandleInput()
        {
            Vector2 moveInput = _moveActions.ReadValue<Vector2>();

            if (moveInput.x < 0 && _curLane > 0)
                _curLane--;
            else if (moveInput.x > 0 && _curLane < 2)
                _curLane++;
        }

        private void MoveFwd()
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _rb.linearVelocity.y, fwdSpeed);
        }
    }
}

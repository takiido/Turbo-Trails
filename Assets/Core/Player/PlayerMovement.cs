//Copyright takiido. All Rights Reserved.

using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public float laneWidth;
        
        public float fwdSpeed = 10.0f;
        public float laneChangeSpeed = 10.0f;
        public float jumpForce = 10.0f;
        public float slideDuration = 1.0f;
        public float laneChangeCooldown = 0.5f;
        
        private Rigidbody _rb;
        private bool _isJumping = false;
        private bool _isSliding = false;
        private float _sliderTimer;
        private float _laneChangeTimer;

        private int _curLane = 1;
        private Vector3 _targetPos;

        private InputAction _moveActions;
        private InputAction _jumpAction;

        private void Awake()
        {
            InputActionAsset inputAction = GetComponent<PlayerInput>().actions;

            _moveActions = inputAction.FindAction("Move");
            _jumpAction = inputAction.FindAction("Jump");
        }

        private void OnEnable()
        {
            _moveActions.Enable();
            _jumpAction.Enable();
        }

        private void OnDisable()
        {
            _moveActions.Disable();
            _jumpAction.Disable();
        }

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _targetPos = transform.position;
        }
    
        private void Update()
        {
            //if (GameManager.Instance.isGameOver) return;
            //MoveFwd();
            HandleInput();
            HandleJump();
            HandleLaneChange();
            
            if (_laneChangeTimer > 0)
            {
                _laneChangeTimer -= Time.deltaTime;
            }
        }
        
        private void MoveFwd()
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _rb.linearVelocity.y, fwdSpeed);
        }

        private void HandleInput()
        {
            Vector2 moveInput = _moveActions.ReadValue<Vector2>();

            if (_laneChangeTimer <= 0)
            {
                if (moveInput.x < 0 && _curLane > 0)
                {
                    _curLane--;
                    _laneChangeTimer = laneChangeCooldown;
                }
                else if (moveInput.x > 0 && _curLane < 2)
                {
                    _curLane++;
                    _laneChangeTimer = laneChangeCooldown;
                }
            }
            
            if (_jumpAction.triggered && !_isJumping)
                Jump();
        }

        private void HandleJump()
        {
            if (_isJumping && _rb.linearVelocity.y <= 0.1f)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f))
                    _isJumping = false;
            }
        }

        private void Jump()
        {
            _isJumping = true;
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        
        private void HandleLaneChange()
        {
            _targetPos = new Vector3((_curLane - 1) * laneWidth, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * laneChangeSpeed);
        }
    }
}

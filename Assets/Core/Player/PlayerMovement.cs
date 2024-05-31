//Copyright takiido. All Rights Reserved.

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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

        public CapsuleCollider pCollider;
        
        private Rigidbody _rb;
        private SpringRaycast _springRaycast;
        private bool _isJumping;
        private bool _isSliding;
        private float _sliderTimer;
        private float _laneChangeTimer;

        private int _curLane = 1;
        private Vector3 _targetPos;

        private InputAction _moveActions;
        private InputAction _jumpAction;
        private InputAction _slideAction;

        private void Awake()
        {
            InputActionAsset inputAction = GetComponent<PlayerInput>().actions;

            _moveActions = inputAction.FindAction("Move");
            _jumpAction = inputAction.FindAction("Jump");
            _slideAction = inputAction.FindAction("Slide");
        }

        private void OnEnable()
        {
            _moveActions.Enable();
            _jumpAction.Enable();
            _slideAction.Enable();
        }

        private void OnDisable()
        {
            _moveActions.Disable();
            _jumpAction.Disable();
            _slideAction.Disable();
        }

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _springRaycast = GetComponent<SpringRaycast>();
            _targetPos = transform.position;

            _isJumping = false;
            _isSliding = false;
        }
    
        private void Update()
        {
            //if (GameManager.Instance.isGameOver) return;
            //MoveFwd();
            HandleInput();
            HandleJump();
            HandleSlide();
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

            if (_slideAction.triggered && !_isSliding)
                Slide();
        }

        private void HandleJump()
        {
            if (_isJumping && _rb.linearVelocity.y <= 0.1f)
            {
                if (Physics.Raycast(transform.position, Vector3.down, out _, 1.1f))
                    _isJumping = false;
            }
        }

        private void Jump()
        {
            _isJumping = true;
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        private void HandleSlide()
        {
            if (_isSliding)
            {
                _sliderTimer += Time.deltaTime;
                if (_sliderTimer >= slideDuration)
                    EndSlide();
            }
        }

        private void Slide()
        {
            _springRaycast.enabled = false;
            _isSliding = true;
            _sliderTimer = 0.0f;

            pCollider.center = new Vector3(pCollider.center.x, pCollider.center.y / 2, pCollider.center.z);
            pCollider.height /= 2;
        }

        private void EndSlide()
        {
            _springRaycast.enabled = true;
            _isSliding = false;
            
            pCollider.center = new Vector3(pCollider.center.x, pCollider.center.y * 2, pCollider.center.z);
            pCollider.height *= 2;
        }
        
        private void HandleLaneChange()
        {
            _targetPos = new Vector3((_curLane - 1) * laneWidth, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * laneChangeSpeed);
        }
    }
}

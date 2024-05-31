//Copyright takiido. All Rights Reserved.

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

        public Animator bodyAnimator;
        public Animator legsAnimator;
        private static readonly int IsJumping = Animator.StringToHash("isJumping");
        private static readonly int IsSliding = Animator.StringToHash("isSliding");

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
            _springRaycast = SpringRaycast.Instance;
            _targetPos = transform.position;

            _isJumping = false;
            _isSliding = false;
        }
    
        private void Update()
        {
            //if (GameManager.Instance.isGameOver) return;
            //MoveFwd();
            HandleInput();
            HandleLaneChange();
            
            if (_laneChangeTimer > 0)
            {
                _laneChangeTimer -= Time.deltaTime;
            }
        }

        private void FixedUpdate()
        {
            HandleJump();
            HandleSlide();
        }

        private void MoveFwd()
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _rb.linearVelocity.y, fwdSpeed);
        }

        private void HandleInput()
        {
            Vector2 moveInput = _moveActions.ReadValue<Vector2>();

            if (_laneChangeTimer <= 0 && !_isSliding)
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
            {
                Jump();
            }

            if (_slideAction.triggered && !_isSliding)
            {
                Slide();
            }
        }
        
        private void Jump()
        {
            if (!_isJumping)
            {
                _isJumping = true;
                bodyAnimator.SetBool(IsJumping, true);
                legsAnimator.SetBool(IsJumping, true);
                _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
        
        private void Slide()
        {
            _springRaycast.SetSlide(true);
            _isSliding = true;
            bodyAnimator.SetBool(IsSliding, true);
            legsAnimator.SetBool(IsSliding, true);
            _sliderTimer = 0.0f;

            pCollider.center = new Vector3(pCollider.center.x, pCollider.center.y / 2, pCollider.center.z);
            pCollider.height /= 2;
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
        
        private void HandleJump()
        {
            if (_isJumping && _rb.linearVelocity.y <= 0.1f)
            {
                if (Physics.Raycast(transform.position, Vector3.down, out _, 1.8f, LayerMask.GetMask("Ground")))
                {
                    _isJumping = false;
                    bodyAnimator.SetBool(IsJumping, false);
                    legsAnimator.SetBool(IsJumping, false);
                }
            }
        }
        
        private void EndSlide()
        {
            _springRaycast.SetSlide(false);
            _isSliding = false;
            bodyAnimator.SetBool(IsSliding, false);
            legsAnimator.SetBool(IsSliding, false);
            
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

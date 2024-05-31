//Copyright takiido. All Rights Reserved.

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

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _targetPos = transform.position;
        }
    
        private void Update()
        {
            //if (GameManager.Instance.isGameOver) return;
        }

        private void HandleInput()
        {
        }
    }
}

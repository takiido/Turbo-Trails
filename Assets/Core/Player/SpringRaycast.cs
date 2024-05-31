// Copyright takiido. All Rights Reserved.

using System;
using UnityEngine;

namespace Core.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class SpringRaycast : MonoBehaviour
    {
        public static SpringRaycast Instance { get; private set; }
        
        [Header("Debugging")]
        [Tooltip("Enable to visualize the raycast in the Scene view.")]
        public bool debug;

        [Header("Spring Settings")]
        [Tooltip("Layer mask for detecting ground.")]
        public LayerMask groundLayer;

        [Tooltip("Spring constant (stiffness). Lower values for a softer spring.")]
        [SerializeField] private float springConstant = 10.0f;

        [Tooltip("Damping constant to control the spring's response.")]
        [SerializeField] private float dampingConstant = 1.0f;

        [Tooltip("Rest length of the spring when no force is applied.")]
        [SerializeField] private float restLength = 1.5f;

        [Header("Legs Settings")]
        [Tooltip("Reference to the legs game object.")]
        public Transform legs;

        [Tooltip("Max distance to keep legs on the ground")] 
        [SerializeField] private float maxDistanceToGround;
        
        [Tooltip("Distance between legs and body if jump")] 
        [SerializeField] private float legsDistance;
        
        private Rigidbody _rb;
        private float _defaultSpringConstant;

        private void Awake()
        {
            if (Instance == null) 
                Instance = this; 
            else Destroy(gameObject);
        }

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _defaultSpringConstant = springConstant;
        }

        private void Update()
        {
            ApplySpringEffect();
        }

        private void ApplySpringEffect()
        {
            Vector3 rayDirection = Vector3.down;
            Vector3 rayOrigin = transform.position;
            RaycastHit hit;

            if (Physics.Raycast(rayOrigin, rayDirection, out hit, Mathf.Infinity, groundLayer))
            {
                Vector3 hitPoint = hit.point;
                
                float distanceToGround = hit.distance;
                float displacement = restLength - distanceToGround;
                float springForce = springConstant * displacement;
                float dampingForce = dampingConstant * _rb.linearVelocity.y;
                float totalForce = springForce - dampingForce;

                if (distanceToGround <= maxDistanceToGround)
                    legs.position = hitPoint;
                else 
                    legs.position = transform.position - Vector3.up * legsDistance;

                _rb.AddForce(Vector3.up * totalForce);
                
                if (debug)
                {
                    Debug.Log($"Distance to Ground: {distanceToGround}");
                    Debug.Log($"Displacement: {displacement}");
                    Debug.Log($"Spring Force: {springForce}");
                    Debug.Log($"Damping Force: {dampingForce}");
                    Debug.Log($"Total Force: {totalForce}");
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (debug)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 10.0f);
            }
        }

        public void SetSlide(bool isSliding)
        {
            springConstant = isSliding ? 0.0f : _defaultSpringConstant;
        }
    }
}
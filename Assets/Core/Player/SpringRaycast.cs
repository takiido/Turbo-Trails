using UnityEngine;

namespace Core.Player
{
    public class SpringRaycast : MonoBehaviour
    {
        public bool debug = false;
        
        public LayerMask groundLayer;

        private float _springConstant = 10.0f;
        private float _dampingConstant = 1.0f;
        private float _restLength = 1.5f;

        private Rigidbody _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            SpringEffect();
        }
    
        private void SpringEffect()
        {
            Vector3 rayDirection = Vector3.down;
            Vector3 rayOrigin = transform.position;
            RaycastHit hit;

            // Cast the ray
            if (Physics.Raycast(rayOrigin, rayDirection, out hit, Mathf.Infinity, groundLayer))
            {
                // Calculate the distance to the ground
                float distanceToGround = hit.distance;

                // Calculate the displacement from the rest length
                float displacement = _restLength - distanceToGround;

                // Calculate the spring force using Hooke's law
                float springForce = _springConstant * displacement;

                // Calculate the damping force
                float dampingForce = _dampingConstant * _rb.velocity.y;

                // Calculate the total force
                float totalForce = springForce - dampingForce;

                // Apply the force to the Rigidbody
                _rb.AddForce(Vector3.up * totalForce);
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
    }
}

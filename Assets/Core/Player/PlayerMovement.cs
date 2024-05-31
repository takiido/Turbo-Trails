//Copyright takiido. All Rights Reserved.

using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Player
{
    public class PlayerMovement : MonoBehaviour
    {

    
        private void Update()
        {
        }
    
        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed)
                Debug.Log("Move");
        }
    }
}

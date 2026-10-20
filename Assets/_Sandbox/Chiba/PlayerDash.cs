using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;

namespace Sandbox.Chiba
{
    public class PlayerDash : MonoBehaviour
    {
        [SerializeField] InputAction dashAction;
        [SerializeField] /*public */float speedAfterDash;
        [SerializeField] public float dashSpeed;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }
        private void OnEnable()
        {
            dashAction.Enable();
        }
        private void OnDisable()
        {
            dashAction.Disable();
        }
        // Update is called once per frame
        void Update()
        {
            if(dashAction.IsPressed())
            {
                dashSpeed = speedAfterDash;
            }
            else
            {
                dashSpeed = 1;
            }
        }
    }
}
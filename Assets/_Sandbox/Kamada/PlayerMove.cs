using UnityEngine;
using UnityEngine.InputSystem;

namespace Sandbox.Kamada
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] InputAction moveAction;

        private void OnEnable()
        {
            moveAction.Enable();
        }

        private void OnDisable()
        {
            moveAction.Disable();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Vector2 input = moveAction.ReadValue<Vector2>();

            transform.position +=
                new Vector3(input.x * Time.deltaTime, 0.0f, input.y * Time.deltaTime);
        }
    }
}

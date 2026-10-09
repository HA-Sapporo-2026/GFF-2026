using UnityEngine;
using UnityEngine.InputSystem;

namespace Sandbox.Niwa
{
    public class PlayerMove : MonoBehaviour
    {
        Vector2 movecontext;
        [SerializeField] float speed = 1.0f;
        void Start()
        {

        }

        void Update()
        {
            var move = new Vector3(movecontext.x, 0, movecontext.y) * speed * Time.deltaTime;
            transform.Translate(move);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            movecontext = context.ReadValue<Vector2>();
        }
    }
}
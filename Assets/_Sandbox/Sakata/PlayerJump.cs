using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static UnityEditor.Progress;

public class PlayerJump : MonoBehaviour
{
    public Rigidbody rigidbody;
    public Collider collider;
    public InputAction jumpAction;
    GameObject floorObject;

    bool switchFloor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        jumpAction.Enable();
    }

    void OnDisable()
    {
        jumpAction.Disable();
    }

    void Update()
    {
        if (jumpAction.WasPressedThisFrame() && !switchFloor)
            rigidbody.linearVelocity = new Vector3(rigidbody.linearVelocity.x, 5.0f, rigidbody.linearVelocity.y);
    }

    private void OnCollisionStay(Collision collision)
    {
        switchFloor = false;
    }

    private void OnCollisionExit(Collision collision)
    {
        switchFloor = true;
    }
}

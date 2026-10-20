using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
{
    [SerializeField] InputAction dashAction;

    public float dashSpeed = 2.0f;
    public bool dashFlag = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        dashAction.Enable();
    }

    void OnDisable()
    { 
        dashAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (dashAction.IsPressed())
            dashFlag = true;
        else
            dashFlag = false;
    }
}

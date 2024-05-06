using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private GameObject chatScreen;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform camera;
    [SerializeField] private float accel;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float camSens;
    [SerializeField] private float maxCamSpeed;
    
    private Vector2 moveInput;
    private float mouseX;
    
    private void OnToggleChat()
    {
        chatScreen.SetActive(!chatScreen.activeSelf);
        Cursor.lockState = chatScreen.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        rb.AddForce((transform.forward * moveInput.y + transform.right * moveInput.x) * (accel * Time.fixedDeltaTime));
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
    }

    private void Update()
    {
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, mouseX * Time.deltaTime * camSens, 0);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxCamSpeed * Time.deltaTime);
    }

    private void OnMove(InputValue input)
    {
        if (chatScreen.activeSelf) return;
        
        moveInput = input.Get<Vector2>();
    }

    private void OnLook(InputValue input)
    {
        if (chatScreen.activeSelf) return;

        mouseX = input.Get<Vector2>().x;
    }
}
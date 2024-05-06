using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private GameObject chatScreen;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float accel;
    [SerializeField] private float maxSpeed;

    private Vector2 moveInput;
    
    private void OnToggleChat()
    {
        chatScreen.SetActive(!chatScreen.activeSelf);
    }

    private void FixedUpdate()
    {
        rb.AddForce(new Vector3(moveInput.x, 0, moveInput.y) * (accel * Time.deltaTime));
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
    }

    private void OnMove(InputValue input)
    {
        if (chatScreen.activeSelf) return;
        
        moveInput = input.Get<Vector2>();
    }
}
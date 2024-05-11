using Network;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Players
{
    public class PlayerController : Spawnable
    {
        [SerializeField] private GameObject chatScreen;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Transform camera;
        [SerializeField] private Transform camPos;
        [SerializeField] private float accel;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float camSens;
        [SerializeField] private float maxCamSpeed;
        
        private Vector2 moveInput;
        private float mouseX;

        private void OnEnable()
        {
            if (!NetworkManager.Instance.isServer)
                Client.Connected += OnConnectedHandler;
        }
    
        private void OnDisable()
        {
            if (!NetworkManager.Instance.isServer)
                Client.Connected -= OnConnectedHandler;
        }

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
    
        private static void OnConnectedHandler()
        {
            NetSpawnRequest spawnRequest = new();
        
            NetworkManager.Instance.SendToServer(spawnRequest.Serialize());
        }

        public override void Spawn(int id)
        {
            this.id = id;
        
            PlayerController instance = Instantiate(this, Vector3.zero, Quaternion.identity);
            
            Camera cam = Camera.main;

            if (!cam) return;
            
            cam.transform.parent = instance.camPos;
            cam.transform.localPosition = Vector3.zero;
            cam.transform.localRotation = Quaternion.identity;
        }
    }
}
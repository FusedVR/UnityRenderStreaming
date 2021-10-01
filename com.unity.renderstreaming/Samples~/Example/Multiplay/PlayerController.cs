using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace Unity.RenderStreaming.Samples
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] GameObject player;
        [SerializeField] GameObject cameraPivot;
        [SerializeField] PlayerInput playerInput;
        [SerializeField] InputSystemChannelReceiver receiver;
        [SerializeField] TextMesh label;

        [SerializeField] float moveSpeed = 100f;
        [SerializeField] float rotateSpeed = 10f;

        Vector2 inputMovement;
        Vector2 inputLook;

        Vector3 initialPosition;

        private void Awake()
        {
            playerInput.neverAutoSwitchControlSchemes = true;
            receiver.onDeviceChange += OnDeviceChange;

            initialPosition = transform.position;
        }

        void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (!playerInput.user.valid)
                return;
            var user = playerInput.user;

            switch (change)
            {
                case InputDeviceChange.Added:
                {
                    InputUser.PerformPairingWithDevice(device, user);
                    return;
                }
                case InputDeviceChange.Removed:
                {
                    user.UnpairDevice(device);
                    return;
                }
            }
        }

        private void Update()
        {
            var moveForward = Quaternion.Euler(0, cameraPivot.transform.eulerAngles.y, 0) * new Vector3(inputMovement.x, 0, inputMovement.y);
            //player.transform.position += moveForward * Time.deltaTime * moveSpeed;
            player.GetComponent<Rigidbody>().AddForce(moveForward * Time.deltaTime * moveSpeed);

            var moveAngles = new Vector3(-inputLook.y, inputLook.x);
            var newAngles = cameraPivot.transform.localEulerAngles + moveAngles * Time.deltaTime * rotateSpeed;
            cameraPivot.transform.localEulerAngles = new Vector3(Mathf.Clamp(newAngles.x, 0, 45), newAngles.y, 0); ;

            // reset if the ball fall down from the floor
            if (player.transform.position.y < -5)
            {
                player.transform.position = initialPosition;
                player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }

        public void SetLabel(string text)
        {
            label.text = text;
        }

        public void OnControlsChanged()
        {
            Debug.Log($"{label.text} OnControlsChanged");
        }

        public void OnDeviceLost()
        {
            Debug.Log($"{label.text} OnDeviceLost");
        }

        public void OnDeviceRegained()
        {
            Debug.Log($"{label.text} OnDeviceRegained");
        }

        public void OnMovement(InputAction.CallbackContext value)
        {
            inputMovement = value.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext value)
        {
            inputLook = value.ReadValue<Vector2>();
        }
    }
}

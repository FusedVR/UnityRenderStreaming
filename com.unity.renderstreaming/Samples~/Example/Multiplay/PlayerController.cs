using System.Collections.Generic;
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

        List<InputDevice> devices = new List<InputDevice>();

        private void Awake()
        {
            Debug.Log("PlayerController " + gameObject.name);
            //playerInput.neverAutoSwitchControlSchemes = true;
            receiver.onDeviceChange += OnDeviceChange;

            initialPosition = transform.position;
        }

        void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            //if (!playerInput.user.valid)
            //{
            //    playerInput.enabled = true;
            //}
            Debug.Log("OnDeviceChange " + device.name + " change:" + change + " id:" + playerInput.user.id + " valid:" + playerInput.user.valid);

            //var user = playerInput.user;

            switch (change)
            {
                case InputDeviceChange.Added:
                {
                    Debug.Log("OnDeviceChange2 " +device.name + " remote" + device.remote);
                    //InputUser.PerformPairingWithDevice(device, user);
                    devices.Add(device);
                    playerInput.SwitchCurrentControlScheme(devices.ToArray());
                    return;
                }
                case InputDeviceChange.Removed:
                {
                    //user.UnpairDevice(device);
                    devices.Remove(device);
                    playerInput.SwitchCurrentControlScheme(devices.ToArray());
                    return;
                }
            }
        }

        private void Update()
        {
            var forwardDirection = Quaternion.Euler(0, cameraPivot.transform.eulerAngles.y, 0);
            var moveForward = forwardDirection * new Vector3(inputMovement.x, 0, inputMovement.y);
            player.GetComponent<Rigidbody>().AddForce(moveForward * Time.deltaTime * moveSpeed);

            var moveAngles = new Vector3(-inputLook.y, inputLook.x);
            var newAngles = cameraPivot.transform.localEulerAngles + moveAngles * Time.deltaTime * rotateSpeed;
            cameraPivot.transform.localEulerAngles = new Vector3(Mathf.Clamp(newAngles.x, 0, 45), newAngles.y, 0);

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
        }

        public void OnDeviceLost()
        {
        }

        public void OnDeviceRegained()
        {
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

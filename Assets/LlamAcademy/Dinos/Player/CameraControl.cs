using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LlamAcademy.Dinos.Player
{
    [RequireComponent(typeof(CinemachineCamera))]
    public class CameraControl : MonoBehaviour
    {
        [SerializeField]
        private AnimationCurve SpeedRamp = new() { keys = new Keyframe[] { new Keyframe(0, 0), new Keyframe(0, 1) } };

        [SerializeField] private bool EnableMousePan;
        [SerializeField]
        private float EdgeScrollWidth = 100;

        [SerializeField] private BoxCollider WorldBounds;
        [SerializeField]
        [Range(0.01f, 50)]
        private float KeyboardSpeed = 5f;

        private CinemachineCamera CinemachineCamera;
        private float MouseScrollStartTime;
        private bool IsMouseScrolling;

        private void Awake()
        {
            CinemachineCamera = GetComponent<CinemachineCamera>();
        }

        private void Update()
        {
            HandleKeyboardInput();
            if (EnableMousePan)
            {
                HandleMouseInput();
            }

            ClampToWorld();
        }

        private void HandleMouseInput()
        {
            Vector3 moveDirection = Vector3.zero;
            Vector3 screenPosition = Mouse.current.position.ReadValue();

            float scrollRightPosition = Screen.width - EdgeScrollWidth;
            float scrollUpPosition = Screen.height - EdgeScrollWidth;

            if (screenPosition.x < EdgeScrollWidth)
            {
                moveDirection += Vector3.right;
            }
            else if (screenPosition.x > scrollRightPosition)
            {
                moveDirection += Vector3.left;
            }

            if (screenPosition.y < EdgeScrollWidth)
            {
                moveDirection += Vector3.forward;
            }
            else if (screenPosition.y > scrollUpPosition)
            {
                moveDirection += Vector3.back;
            }

            if (moveDirection != Vector3.zero)
            {
                if (!IsMouseScrolling)
                {
                    MouseScrollStartTime = Time.time;
                }
                IsMouseScrolling = true;
            }
            else
            {
                IsMouseScrolling = false;
            }

            CinemachineCamera.Follow.position += SpeedRamp.Evaluate(Time.time - MouseScrollStartTime) * Time.deltaTime * moveDirection;
        }

        private void HandleKeyboardInput()
        {
            Vector3 moveDirection = Vector3.zero;
            if (Keyboard.current.upArrowKey.isPressed)
            {
                moveDirection += Vector3.back;
            }
            if (Keyboard.current.downArrowKey.isPressed)
            {
                moveDirection += Vector3.forward;
            }
            if (Keyboard.current.leftArrowKey.isPressed)
            {
                moveDirection += Vector3.right;
            }
            if (Keyboard.current.rightArrowKey.isPressed)
            {
                moveDirection += Vector3.left;
            }

            CinemachineCamera.Follow.position += KeyboardSpeed * Time.deltaTime * moveDirection;
        }

        private void ClampToWorld()
        {
            if (CinemachineCamera.Follow.position.x >= WorldBounds.bounds.max.x)
            {
                CinemachineCamera.Follow.position = new Vector3(WorldBounds.bounds.max.x, CinemachineCamera.Follow.position.y, CinemachineCamera.Follow.position.z);
            }
            else if (CinemachineCamera.Follow.position.x <= WorldBounds.bounds.min.x)
            {
                CinemachineCamera.Follow.position = new Vector3(WorldBounds.bounds.min.x, CinemachineCamera.Follow.position.y, CinemachineCamera.Follow.position.z);
            }
            if (CinemachineCamera.Follow.position.z >= WorldBounds.bounds.max.z)
            {
                CinemachineCamera.Follow.position = new Vector3(CinemachineCamera.Follow.position.x, CinemachineCamera.Follow.position.y, WorldBounds.bounds.max.z);
            }
            else if (CinemachineCamera.Follow.position.z <= WorldBounds.bounds.min.z)
            {
                CinemachineCamera.Follow.position = new Vector3(CinemachineCamera.Follow.position.x, CinemachineCamera.Follow.position.y, WorldBounds.bounds.min.z);
            }
        }

    }

}

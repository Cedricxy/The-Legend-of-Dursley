using UnityEngine;
using Unity.Cinemachine;

namespace Camera
{
    public class CameraMovement : MonoBehaviour
    {
        public static CinemachineCamera CinemachineCamera;
        [SerializeField] private float activeFieldOfView = 3.85f;
        [SerializeField] private float fieldOfViewMin = 1.85f;
        [SerializeField] private float fieldOfViewMax = 5.85f;
        [SerializeField] private float zoomSpeed = 5f;
        
        private void Awake()
        {
            CinemachineCamera = Object.FindFirstObjectByType<CinemachineCamera>();
        }

        private void Update()
        {
            HandleZoom();
        }
        
        private void HandleZoom()
        {
            if (Input.mouseScrollDelta.y < 0 || Input.GetKeyDown(KeyCode.KeypadMinus))
                activeFieldOfView += 1f;
            if (Input.mouseScrollDelta.y > 0 || Input.GetKeyDown(KeyCode.KeypadPlus))
                activeFieldOfView -= 1f;

            activeFieldOfView = Mathf.Clamp(activeFieldOfView, fieldOfViewMin, fieldOfViewMax);

            if (CinemachineCamera)
            {
                CinemachineCamera.Lens.OrthographicSize = Mathf.Lerp(
                    CinemachineCamera.Lens.OrthographicSize,
                    activeFieldOfView,
                    Time.deltaTime * zoomSpeed
                );
            }
        }
    }
}
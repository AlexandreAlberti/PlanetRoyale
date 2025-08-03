using Cinemachine;
using Game.Camera;
using UnityEngine;

namespace Game.UI
{
    public class LookAtCamera : MonoBehaviour
    {
        private void LateUpdate()
        {
            UpdateDirection();
        }

        private void UpdateDirection()
        {
            CinemachineVirtualCamera virtualCamera = CameraManager.Instance.Camera();
            Vector3 lookDirection = transform.position - virtualCamera.transform.position;
            transform.rotation = Quaternion.LookRotation(lookDirection, virtualCamera.transform.up);
        }
    }
}
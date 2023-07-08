using System;
using UnityEngine;
public class LookAtCamera : MonoBehaviour
{
   [SerializeField] private Mode _mode;
   private enum Mode
   {
      LookAt,
      LookAtInverted,
      CameraForward,
      CameraForwardInverted
   }

   private Camera _camera;
   private void Awake()
   {
      _camera = Camera.main;
   }

   private void LateUpdate()
   {
      switch (_mode)
      {
         case Mode.LookAt:
            transform.LookAt(_camera.transform);
            break;
         case Mode.LookAtInverted:
            var dirFromCamera = transform.position - _camera.transform.position;
            transform.LookAt(transform.position + dirFromCamera);
            break;
         case Mode.CameraForward:
            transform.forward = _camera.transform.forward;
            break;
         case Mode.CameraForwardInverted:
            transform.forward = -_camera.transform.forward;
            break;
         default:
            throw new ArgumentOutOfRangeException();
      }
    
   }
}

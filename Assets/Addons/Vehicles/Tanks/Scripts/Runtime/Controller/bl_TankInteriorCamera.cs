using UnityEngine;
using System.Collections;

namespace MFPS.Runtime.Vehicles
{
    public class bl_TankInteriorCamera : MonoBehaviour
    {
        public float pitchSensitivity = 0.4f;
        public float yawSensitivity = 0.7f;
        public Vector2 pitchClamp = new Vector2(10, 40);
        public AnimationCurve pitchClampAngleModifier;
        public Camera cameraReference;

        private bl_TankTurret tankTurret;
        private Vector3 turretRotation;
        private Vector3 cannonRotation;
        private float defaultFov;

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            tankTurret = GetComponentInParent<bl_TankTurret>();
            defaultFov = cameraReference.fieldOfView;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnEnable()
        {
            bl_VehicleCamera.Instance.CurrentVehicleCamera = cameraReference;
            cannonRotation = tankTurret.cannonPivot.localEulerAngles;
            turretRotation = tankTurret.turretPivot.localEulerAngles;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDisable()
        {
            if (bl_VehicleCamera.Instance != null) bl_VehicleCamera.Instance.CurrentVehicleCamera = null;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            Rotation();
        }

        /// <summary>
        /// 
        /// </summary>
        void Rotation()
        {
            float x = bl_GameInput.MouseX * yawSensitivity;
            float y = -bl_GameInput.MouseY * pitchSensitivity;

            if(tankTurret.upAxis == bl_TankTurret.UpAxis.ZIsUp)
            {
                turretRotation.z += x;
            }
            else
            {
                turretRotation.y += x;
            }

            cannonRotation.x += y;

            float minVertical = pitchClamp.y;
            float extraPitch = pitchClampAngleModifier.Evaluate(tankTurret.turretPivot.localEulerAngles.z);
            minVertical -= extraPitch;

            cannonRotation.x = Mathf.Clamp(bl_MathUtility.WrapAngle(cannonRotation.x), pitchClamp.x, minVertical);

            tankTurret.turretPivot.localRotation = Quaternion.Slerp(tankTurret.turretPivot.localRotation, Quaternion.Euler(turretRotation), 1);
            tankTurret.cannonPivot.localRotation = Quaternion.Slerp(tankTurret.cannonPivot.localRotation, Quaternion.Euler(cannonRotation), 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newZoom"></param>
        public void SetZoom(bl_VehicleCamera.ZoomTransition zoom)
        {
            StopCoroutine(nameof(DoZoomTransition));
            StartCoroutine(nameof(DoZoomTransition), zoom);
        }

        /// <summary>
        /// Set the camera field of view to it is original value
        /// </summary>
        public void ResetZoom(float duration = 0.5f)
        {
            SetZoom(new bl_VehicleCamera.ZoomTransition()
            {
                TargetZoom = defaultFov,
                TransitionDuration = duration,
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1)
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newZoom"></param>
        /// <returns></returns>
        IEnumerator DoZoomTransition(bl_VehicleCamera.ZoomTransition zoom)
        {
            float origin = cameraReference.fieldOfView;

            float d = 0;
            float c;
            while (d <= 1)
            {
                d += Time.deltaTime / zoom.TransitionDuration;
                c = zoom.Curve.Evaluate(d);
                cameraReference.fieldOfView = Mathf.Lerp(origin, zoom.TargetZoom, c);

                yield return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="active"></param>
        public void SetActive(bool active) => gameObject.SetActive(active);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsActive() => gameObject.activeInHierarchy;
    }
}
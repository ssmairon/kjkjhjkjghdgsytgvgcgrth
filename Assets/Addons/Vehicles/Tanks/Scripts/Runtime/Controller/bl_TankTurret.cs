using UnityEngine;
using MFPS.Internal.Interfaces;
using UnityEngine.Serialization;

namespace MFPS.Runtime.Vehicles
{
    public class bl_TankTurret : bl_MonoBehaviour, IVehicleUpdate
    {
        #region Public members
        [Header("Settings")]
        [LovattoToogle] public bool smoothMovement = true;
        public float smoothness = 12;
        public float turnRate = 30;
        public float turretRotationSpeedQualifier = 0.7f;

        [Header("Movement")]
        public UpAxis upAxis = UpAxis.YIsUp;
        [LovattoToogle] public bool invertOrientation = false;
        [LovattoToogle] public bool limitHorizontalRotation = false;
        public Vector2 horizontalAngles = new Vector2(-30f, 30f);
        public float cannonSpeed = 50f;
        public Vector2 verticalClamp = new Vector2(-60, 15);

        [Header("Aim")]
        public float aimZoom = 30;
        public float transitionDuration = 0.6f;
        public AnimationCurve aimTransitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("References")]
        public bl_TankInteriorCamera interiorCamera;
        [FormerlySerializedAs("baseTurret")]
        public Transform turretPivot;
        [FormerlySerializedAs("barrelTurret")]
        public Transform cannonPivot;
        public Transform barrelLookAt;
        public Transform firePoint;
        #endregion

        #region Private region
        private float angleYDelta;
        private float angleXDelta;
        private Vector3 turretLocalEulerAngles, cannonLocalEulerAngles;
        private bl_VehicleManager vehicleManager;
        #endregion

        protected virtual float TurretAxisControl
        {
            get
            {
                angleYDelta = bl_MathUtility.AngleSigned(upAxis == UpAxis.YIsUp ? turretPivot.forward : turretPivot.up, AimingDirection(turretPivot.position), upAxis == UpAxis.YIsUp ? turretPivot.up : turretPivot.forward);
                return Mathf.Clamp(angleYDelta, -1f, 1f);
            }
        }

        protected virtual float CannonAxisControl
        {
            get
            {
                angleXDelta = bl_MathUtility.AngleSigned(upAxis == UpAxis.YIsUp ? cannonPivot.forward : cannonPivot.up, AimingDirection(firePoint.position), upAxis == UpAxis.YIsUp ? cannonPivot.right : -cannonPivot.right);
                return Mathf.Clamp(angleXDelta, -1f, 1f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            TryGetComponent(out vehicleManager);
            if (vehicleManager != null) vehicleManager.AddUpdateComponent(this);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            turretLocalEulerAngles = turretPivot.localEulerAngles;
            cannonLocalEulerAngles = cannonPivot.localEulerAngles;
            if (interiorCamera != null) interiorCamera.SetActive(false);
        }

        /// <summary>
        /// 
        /// </summary>
        void IVehicleUpdate.WhenInsideUpdateVehicle()
        {
            if (vehicleManager.localVehicleState == Internal.Vehicles.LocalVehicleState.PlayerIn && vehicleManager.isMine)
            {
                TurretRotation();
                CannonRotation();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void IVehicleUpdate.WhenInsideFixedUpdateVehicle()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public void SwitchCameraView()
        {
            if (interiorCamera == null) return;

            bool usingInside = interiorCamera.IsActive();
            if (usingInside)
            {
                interiorCamera.SetActive(false);
                bl_VehicleCamera.Instance.SetActive(true);
                bl_VehicleCamera.Instance.SetCameraSettings(GetComponentInChildren<bl_VehicleCameraRig>().customCameraSettings);
                bl_TankStateUI.Instance?.SetupTank(bl_VehicleCamera.Instance.cameraTransform, transform);
                bl_TankCrosshair.Instance.SetActive(bl_TankCrosshair.CrosshairType.Dynamic, true);
            }
            else
            {
                interiorCamera.SetActive(true);
                bl_VehicleCamera.Instance.SetActive(false);
                bl_TankStateUI.Instance?.SetupTank(interiorCamera.transform, transform);
                bl_TankCrosshair.Instance.SetActive(bl_TankCrosshair.CrosshairType.Dynamic, false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void TurretRotation()
        {
            if (interiorCamera != null && interiorCamera.IsActive()) return;
            if (turretPivot == null || bl_MathUtility.Approximately(TurretAxisControl, 0f)) return;

            float turnDir = turnRate * turretRotationSpeedQualifier * Time.deltaTime;
            turnDir = Mathf.Clamp(angleYDelta, 0f - turnDir, turnDir);

            if (upAxis == UpAxis.YIsUp) turretLocalEulerAngles.y += turnDir;
            else turretLocalEulerAngles.z += turnDir;

            if (limitHorizontalRotation)
            {
                if (upAxis == UpAxis.YIsUp) turretLocalEulerAngles.y = Mathf.Clamp(turretLocalEulerAngles.y, horizontalAngles.x, horizontalAngles.y);
                else turretLocalEulerAngles.z = Mathf.Clamp(turretLocalEulerAngles.z, horizontalAngles.x, horizontalAngles.y);

            }
            if (smoothMovement)
            {
                if (!invertOrientation)
                    turretPivot.localRotation = Quaternion.Slerp(turretPivot.localRotation, Quaternion.Euler(turretLocalEulerAngles), Time.deltaTime * smoothness);
                else turretPivot.localRotation = Quaternion.Slerp(turretPivot.localRotation, Quaternion.Euler(-turretLocalEulerAngles), Time.deltaTime * smoothness);
            }
            else
            {
                if (!invertOrientation)
                    turretPivot.localEulerAngles = turretLocalEulerAngles;
                else turretPivot.localEulerAngles = -turretLocalEulerAngles;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void CannonRotation()
        {
            if (interiorCamera != null && interiorCamera.IsActive()) return;

            float verticalInput = CannonAxisControl;
            if (Mathf.Abs(verticalInput) >= 1f)
            {
                verticalInput = Mathf.Clamp(verticalInput * cannonSpeed * Time.deltaTime, -Mathf.Abs(angleXDelta), Mathf.Abs(angleXDelta));
            }
            cannonLocalEulerAngles.x += verticalInput;
            if (cannonLocalEulerAngles.x > 180f)
            {
                cannonLocalEulerAngles.x -= 360f;
            }
            cannonLocalEulerAngles.x = Mathf.Clamp(cannonLocalEulerAngles.x, verticalClamp.x, verticalClamp.y);

            if (smoothMovement)
            {
                cannonPivot.localRotation = Quaternion.Slerp(cannonPivot.localRotation, Quaternion.Euler(cannonLocalEulerAngles), Time.deltaTime * smoothness);
            }
            else cannonPivot.localEulerAngles = cannonLocalEulerAngles;

            bl_TankCrosshair.Instance?.UpdatePosition(barrelLookAt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private Vector3 AimingDirection(Vector3 position)
        {
            return bl_VehicleCamera.Instance.headLook.position - position;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle"></param>
        public void SetBaseAngle(Vector3 angle)
        {
            turretPivot.localRotation = Quaternion.Slerp(turretPivot.localRotation, Quaternion.Euler(angle), Time.fixedDeltaTime * smoothness);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle"></param>
        public void SetCannonAngle(Vector3 angle)
        {
            cannonPivot.localRotation = Quaternion.Slerp(cannonPivot.localRotation, Quaternion.Euler(angle), Time.fixedDeltaTime * smoothness);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector3 GetBaseTurretAngle() =>  turretPivot.localEulerAngles;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCannonAngle() => cannonPivot.localEulerAngles;

        [System.Serializable]
        public enum UpAxis
        {
            YIsUp,
            ZIsUp
        }
    }
}
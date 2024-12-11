using UnityEngine;

namespace MFPS.Runtime.Vehicles
{
    public class bl_TankCrosshair : MonoBehaviour
    {
        public enum CrosshairType
        {
            Dynamic,
            Fixed,
            Both
        }

        [SerializeField] private RectTransform fixedCrosshairRect = null;
        [SerializeField] private RectTransform crosshairRect = null;

        private Vector3 followPosition;

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            SetActive(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aimTarget"></param>
        public void UpdatePosition(Transform aimTarget)
        {
            if (aimTarget == null || bl_VehicleCamera.Instance == null) return;

            followPosition = bl_VehicleCamera.Instance.cameraRef.WorldToScreenPoint(aimTarget.position);
            followPosition.z = 0;
            crosshairRect.position = Vector3.Lerp(crosshairRect.position, followPosition, Time.smoothDeltaTime * 14);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="active"></param>
        public void SetActive(bool active)
        {
            if (fixedCrosshairRect != null) fixedCrosshairRect.gameObject.SetActive(active);
            if (crosshairRect != null) crosshairRect.gameObject.SetActive(active);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="crosshairType"></param>
        /// <param name="active"></param>
        public void SetActive(CrosshairType crosshairType, bool active)
        {
            if(crosshairType == CrosshairType.Dynamic || crosshairType == CrosshairType.Both)
            {
                if (crosshairRect != null) crosshairRect.gameObject.SetActive(active);
            }

            if (crosshairType == CrosshairType.Fixed || crosshairType == CrosshairType.Both)
            {
                if (fixedCrosshairRect != null) fixedCrosshairRect.gameObject.SetActive(active);
            }
        }

        private static bl_TankCrosshair _instance;
        public static bl_TankCrosshair Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<bl_TankCrosshair>();
                return _instance;
            }
        }
    }
}
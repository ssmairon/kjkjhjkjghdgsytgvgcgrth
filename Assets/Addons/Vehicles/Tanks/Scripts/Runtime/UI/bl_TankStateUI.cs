using UnityEngine;
using MFPS.Internal.Interfaces;

namespace MFPS.Runtime.Vehicles
{
    public class bl_TankStateUI : MonoBehaviour, IVehicleUpdate
    {
        [SerializeField] private GameObject content = null;
        [SerializeField] private RectTransform tankBaseIconRect = null;

        private Transform m_tankView, m_tankBase = null;
        private Vector3 viewForward, baseForward;

        /// <summary>
        /// 
        /// </summary>
        void Start()
        {
            SetActive(false);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetActive(bool active)
        {
            if (content != null) content.SetActive(active);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewTransform"></param>
        /// <param name="tankBase"></param>
        public void SetupTank(Transform viewTransform, Transform tankBase)
        {
            m_tankView = viewTransform;
            m_tankBase = tankBase;
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateUI()
        {
            if (m_tankView != null && m_tankBase != null)
            {
                viewForward = m_tankView.forward;
                baseForward = m_tankBase.forward;
                viewForward.y = 0;
                baseForward.y = 0;

                var angle = Vector3.SignedAngle(viewForward, baseForward, Vector3.up);
                var rot = tankBaseIconRect.eulerAngles;
                rot.z = -angle;
                tankBaseIconRect.rotation = Quaternion.Slerp(tankBaseIconRect.rotation, Quaternion.Euler(rot), 1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void WhenInsideFixedUpdateVehicle()
        {
           
        }

        /// <summary>
        /// 
        /// </summary>
        public void WhenInsideUpdateVehicle()
        {
            UpdateUI();
        }

        private static bl_TankStateUI _instance;
        public static bl_TankStateUI Instance
        {
            get
            {
                if (_instance == null) { _instance = FindObjectOfType<bl_TankStateUI>(); }
                return _instance;
            }
        }
    }
}
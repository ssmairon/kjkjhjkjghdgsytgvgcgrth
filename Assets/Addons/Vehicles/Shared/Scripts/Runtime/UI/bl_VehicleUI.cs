using UnityEngine;
using UnityEngine.UI;

namespace MFPS.Runtime.Vehicles
{
    public class bl_VehicleUI : MonoBehaviour
    {
        public GameObject vehicleStatsUI;
        public GameObject exitButton;
        public Text vehicleHealth;

        private bl_VehicleManager activeVehicleTrigger;
        private bl_VehicleManager activeLocalVehicle;
        private int currentSeatID = 0;
        private int activeSeatID = 0;

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            ShowEnterUI(false);
            vehicleStatsUI.SetActive(false);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnEnable()
        {
            bl_VehicleEvents.onLocalEnterInVehicle += OnLocalEnterInVehicle;
            bl_VehicleEvents.onLocalExitInVehicle += OnLocalExitVehicle;
            bl_EventHandler.onLocalPlayerDeath += OnLocalPlayerDeath;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDisable()
        {
            bl_VehicleEvents.onLocalEnterInVehicle -= OnLocalEnterInVehicle;
            bl_VehicleEvents.onLocalExitInVehicle -= OnLocalExitVehicle;
            bl_EventHandler.onLocalPlayerDeath -= OnLocalPlayerDeath;
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnEnterVehicleClick()
        {
            if (activeVehicleTrigger == null) return;

            if (currentSeatID < 0)
            {
                activeVehicleTrigger.IntentToEnterInVehicle();
            }
            else
            {
                activeSeatID = currentSeatID;
                activeVehicleTrigger.VehicleSeats.IntentToEnterInSeat();               
            }        
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnExitVehicleClick()
        {
            if (activeLocalVehicle == null)
            {
                Debug.LogWarning("Intent to exit a vehicle when is not registered as inside a vehicle.");
                return;
            }
            
            if (activeSeatID < 0)
                activeLocalVehicle.IntentToExitVehicle();
            else
            {
                activeLocalVehicle.VehicleSeats?.IntentToExitSeat();
                activeSeatID = -1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void OnLocalEnterInVehicle(bl_VehicleManager vehicle)
        {
            ShowEnterUI(false);
            vehicleStatsUI.SetActive(true);
            activeLocalVehicle = vehicle;
            if (exitButton != null) exitButton.SetActive(bl_UtilityHelper.isMobile);
#if MFPSM
            var mobileLayers = Mobile.bl_MobileButtonLayers.Instance;
            if (mobileLayers != null)
            {
                mobileLayers.SetActiveButtonGroup(vehicle.vehicle.mobileButtonLayer);
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        void OnLocalExitVehicle(bl_VehicleManager vehicle)
        {
            vehicleStatsUI.SetActive(false);
            activeLocalVehicle = null;
            currentSeatID = 0;
#if MFPSM
            var mobileLayers = MFPS.Mobile.bl_MobileButtonLayers.Instance;
            if (mobileLayers != null)
            {
                mobileLayers.SetActiveButtonGroup(0);
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        void OnLocalPlayerDeath()
        {
            ShowEnterUI(false);
            vehicleStatsUI.SetActive(false);
            activeLocalVehicle = null;
            currentSeatID = -1;
            activeSeatID = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vehicle"></param>
        public void UpdateStats(bl_VehicleManager vehicle)
        {
            if (vehicleHealth != null && vehicle.VehicleHealth != null) vehicleHealth.text = ((int)vehicle.VehicleHealth.CurrentHealth).ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ShowEnterUI(bool active, bl_VehicleManager vehicle = null, int seatID = -1)
        {
            activeVehicleTrigger = active ? vehicle : null;
            currentSeatID = seatID;
            if (active)
            {
                string inputName = bl_UtilityHelper.isMobile ? "TOUCH" : bl_Input.GetButtonName("Interact");
                bl_InputInteractionIndicator.ShowIndication(inputName, "TO ENTER IN VEHICLE", () =>
                {
                    OnEnterVehicleClick();
                });
            }
            else
                bl_InputInteractionIndicator.SetActive(false);
        }

        private static bl_VehicleUI _instance;
        public static bl_VehicleUI Instance
        {
            get
            {
                if (_instance == null) { _instance = FindObjectOfType<bl_VehicleUI>(); }
                return _instance;
            }
        }
    }
}
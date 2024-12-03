using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFPS.Mobile
{
    public class bl_MobileButtonLayers : MonoBehaviour
    {
        public GameObject movementJoystick;
        public GameObject weaponSelector;
        public GameObject kitsButton;
        public GameObject chatButton;
        public GameObject voiceButton;
        public List<GameObject> weaponButtons;
        public List<GameObject> movementButtons;
        public List<GameObject> buttonGroups;

        /// <summary>
        /// 
        /// </summary>
        public static void ShowLayers(MobileButtonsLayers layers)
        {
            if (Instance == null) return;

            Instance.SetActiveLayers(true, layers);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void HideLayers(MobileButtonsLayers layers)
        {
            if (Instance == null) return;

            Instance.SetActiveLayers(false, layers);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetActiveLayers(bool active, MobileButtonsLayers layers)
        {
            if (layers.IsEnumFlagPresent(MobileButtonsLayers.MovementJoystick)) movementJoystick.SetActive(active);
            if (layers.IsEnumFlagPresent(MobileButtonsLayers.Chat) && chatButton != null) chatButton.SetActive(active);
            if (layers.IsEnumFlagPresent(MobileButtonsLayers.Voice) && voiceButton != null) voiceButton.SetActive(active);
            if (layers.IsEnumFlagPresent(MobileButtonsLayers.Kits) && kitsButton != null) kitsButton.SetActive(active);
            if (layers.IsEnumFlagPresent(MobileButtonsLayers.WeaponSelector) && weaponSelector != null) weaponSelector.SetActive(active);
            if (layers.IsEnumFlagPresent(MobileButtonsLayers.MovementButtons)) SetActiveInArray(movementButtons.ToArray(), active);
            if (layers.IsEnumFlagPresent(MobileButtonsLayers.WeaponButtons)) SetActiveInArray(weaponButtons.ToArray(), active);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetActiveInArray(GameObject[] array, bool active)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == null) continue;
                array[i].SetActive(active);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetActiveButtonGroup(int groupID)
        {
            if (buttonGroups == null) return;
            foreach (var group in buttonGroups)
            {
                if (group != null) group.SetActive(false);
            }
            buttonGroups[groupID].SetActive(true);
        }

        private static bl_MobileButtonLayers _instance;
        public static bl_MobileButtonLayers Instance
        {
            get
            {
                if (_instance == null) { _instance = FindObjectOfType<bl_MobileButtonLayers>(); }
                return _instance;
            }
        }
    }
}
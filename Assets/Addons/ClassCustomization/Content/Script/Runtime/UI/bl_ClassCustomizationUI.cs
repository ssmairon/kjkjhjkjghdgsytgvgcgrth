using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace MFPS.Addon.ClassCustomization
{
    public class bl_ClassCustomizationUI : MonoBehaviour
    {
        [Serializable]
        public class MenuWindow
        {
            public string Name;
            public GameObject Window;
            public Button OpenButton;
            [LovattoToogle] public bool Active = true;
            public bl_EventHandler.UEvent onOpen;

            public void SetActive(bool active)
            {
                if (Window != null) Window.SetActive(active);
                if (OpenButton != null) OpenButton.interactable = !active;
                onOpen?.Invoke();
            }
        }

        [Header("Windows")]
        public string firstWindow = "loadout";
        public List<MenuWindow> windows;

        [Header("References")]
        public List<bl_SlotCardUI> slotsUI;
        public RectTransform[] ClassButtons;
        public TextMeshProUGUI ClassText = null;
        public GameObject SaveButton;
        public GameObject loadingUI;
        public bl_WeaponSelector weaponSelector;

        public int CurrentWindow { get; set; } = -1;

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            SaveButton?.SetActive(false);
            OpenWindow(firstWindow);
        }

        /// <summary>
        /// 
        /// </summary>
        public void OpenWeaponList(int loadoutId)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowName"></param>
        public void OpenWindow(string windowName)
        {
            var id = windows.FindIndex(x => x.Name == windowName);
            if (id == -1)
            {
                Debug.LogWarning($"Pause window {windowName} doesn't exist.");
                return;
            }
            OpenWindow(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowName"></param>
        public void OpenWindow(int windowIndex)
        {
            if (windowIndex == CurrentWindow) return;

            CurrentWindow = windowIndex;

            windows.ForEach(x => x.SetActive(false));

            var window = windows[CurrentWindow];
            window.SetActive(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bl_SlotCardUI GetSlot(int index)
        {
            return slotsUI[index];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bl_SlotCardUI GetSlot(string key)
        {
            foreach (var item in slotsUI)
            {
                if (item.key == key) return item;
            }
            return null;
        }

        private static bl_ClassCustomizationUI _instance;
        public static bl_ClassCustomizationUI Instance
        {
            get
            {
                if (_instance == null) { _instance = FindObjectOfType<bl_ClassCustomizationUI>(); }
                return _instance;
            }
        }
    }
}
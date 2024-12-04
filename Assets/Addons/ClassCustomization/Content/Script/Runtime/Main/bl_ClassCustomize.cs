using System.Collections.Generic;
using UnityEngine;

namespace MFPS.Addon.ClassCustomization
{
    public class bl_ClassCustomize : MonoBehaviour
    {
        public PlayerClass m_Class { get; set; } = PlayerClass.Assault;
        [Header("Info")]
        public string SceneToReturn = "";
        [LovattoToogle] public bool returnToMenuAfterSave = false;

        [Header("Weapons Info")]
        public ClassWeapons assaultWeapons;
        public ClassWeapons engineerWeapons;
        public ClassWeapons supportWeapons;
        public ClassWeapons reconWeapons;

        [Header("Slots Rules")]
        public ClassAllowedWeaponsType PrimaryAllowedWeapons;
        public ClassAllowedWeaponsType SecondaryAllowedWeapons;
        public ClassAllowedWeaponsType KnifeAllowedWeapons;
        public ClassAllowedWeaponsType GrenadesAllowedWeapons;

        private bl_ClassManager ClassManager;
        private bl_ClassCustomizationUI UI;

        /// <summary>
        /// 
        /// </summary>
        void Awake()
        {
            UI = FindObjectOfType<bl_ClassCustomizationUI>();
            ClassManager = bl_ClassManager.Instance;
            ClassManager.Init();
            bl_Input.Initialize();
            bl_Input.CheckGamePadRequired();
        }

        /// <summary>
        /// 
        /// </summary>
        void Start()
        {
            TakeCurrentClass(bl_ClassManager.Instance.playerClass);
            SelectClassButton();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slotId"></param>
        /// <returns></returns>
        public List<WeaponItemData> GetSupportedWeaponsForSlot(int slotId)
        {
            var list = new List<WeaponItemData>();

            var weaponClass = GetLoadoutClass(bl_ClassManager.Instance.playerClass);
            for (int i = 0; i < weaponClass.AllWeapons.Count; i++)
            {
                var weaponData = weaponClass.AllWeapons[i];
                if (isAllowedWeapon(weaponData.Info, slotId))
                {
                    if (!weaponClass.AllWeapons[i].isEnabled) continue;

                    list.Add(weaponData);
                }
            }

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ChangeSlotClass(WeaponItemData weaponData, int slotId)
        {
            int gunId = weaponData.GunID;
            var loadout = GetWeaponLoadoutList(bl_ClassManager.Instance.playerClass);
            switch (slotId)
            {
                case 0:
                    loadout.Primary = gunId;
                    break;
                case 1:
                    loadout.Secondary = gunId;
                    break;
                case 2:
                    loadout.Perks = gunId;
                    break;
                case 3:
                    loadout.Letal = gunId;
                    break;
            }

            if (bl_SoldierPreview.Instance != null) bl_SoldierPreview.Instance.ShowWeapon(gunId);

            UI.SaveButton.SetActive(true);
        }

        /// <summary>
        /// Update the weapons information in the slots UI
        /// </summary>
        void UpdateClassUI(int id, int slot)
        {
            var weaponList = GetLoadoutClass(bl_ClassManager.Instance.playerClass);
            UI.GetSlot(slot)?.SetupWeapon(weaponList.AllWeapons[id]);
        }

        /// <summary>
        /// 
        /// </summary>
        private bool isAllowedWeapon(bl_GunInfo info, int slot)
        {
            ClassAllowedWeaponsType rules = PrimaryAllowedWeapons;
            if (slot == 1) { rules = SecondaryAllowedWeapons; }
            else if (slot == 2) { rules = KnifeAllowedWeapons; }
            else if (slot == 3) { rules = GrenadesAllowedWeapons; }

            if ((rules.AllowMachineGuns && (info.Type == GunType.Machinegun || info.Type == GunType.Burst)) ||
                (rules.AllowPistols && info.Type == GunType.Pistol) ||
                (rules.AllowShotguns && info.Type == GunType.Shotgun) ||
                (rules.AllowKnifes && info.Type == GunType.Knife) ||
                (rules.AllowGrenades && (info.Type == GunType.Grenade || info.Type == GunType.Grenade)) ||
                (rules.AllowSnipers && info.Type == GunType.Sniper) ||
                (rules.AllowLaunchers && info.Type == GunType.Launcher))
            {
                return true;
            }
            return false;
        }

        public void ChangeKit(int kit)
        {
            bl_ClassManager.Instance.ClassKit = kit;
            UI.SaveButton.SetActive(true);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ChangeClass(PlayerClass newclass)
        {
            if (m_Class == newclass && bl_ClassManager.Instance.playerClass == newclass)
                return;

            m_Class = newclass;
            bl_ClassManager.Instance.playerClass = newclass;
            newclass.SavePlayerClass();
            UI.ClassText.text = (newclass.ToString() + " Class").ToUpper();

            var loadout = GetWeaponLoadoutList(newclass);
            UpdateClassUI(GetListId(newclass, loadout.Primary), 0);
            UpdateClassUI(GetListId(newclass, loadout.Secondary), 1);
            UpdateClassUI(GetListId(newclass, loadout.Perks), 2);
            UpdateClassUI(GetListId(newclass, loadout.Letal), 3);

            PlayerPrefs.SetInt(ClassKey.ClassType, (int)newclass);
            SelectClassButton();
        }

        Vector2 defaulClassSize = Vector2.zero;
        void SelectClassButton()
        {
            if (defaulClassSize == Vector2.zero) { defaulClassSize = UI.ClassButtons[0].sizeDelta; }
            foreach (var r in UI.ClassButtons)
            {
                r.sizeDelta = defaulClassSize;
            }
            Vector2 v = defaulClassSize;
            v.y += 10;
            UI.ClassButtons[(int)bl_ClassManager.Instance.playerClass].sizeDelta = v;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SaveClass()
        {
            UI.loadingUI.SetActive(true);
            bl_ClassManager.Instance.SaveClass(() => { this.InvokeAfter(1, () => { UI.loadingUI.SetActive(false); }); });
            UI.SaveButton.SetActive(false);
            if (returnToMenuAfterSave)
            {
                ReturnToScene();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ReturnToScene()
        {
            bl_UtilityHelper.LoadLevel(SceneToReturn);
        }

        /// <summary>
        /// 
        /// </summary>
        private int GetListId(PlayerClass clas, int id)
        {
            var weaponList = GetLoadoutClass(clas);
            for (int i = 0; i < weaponList.AllWeapons.Count; i++)
            {
                if (weaponList.AllWeapons[i].GunID == id)
                {
                    return i;
                }
            }
            return 0;
        }

        /// <summary>
        /// Take the current class
        /// </summary>
        void TakeCurrentClass(PlayerClass mclass)
        {
            var weaponList = GetLoadoutClass(mclass);
            var classLoaout = GetWeaponLoadoutList(mclass);
            if (bl_SoldierPreview.Instance != null) bl_SoldierPreview.Instance.ShowWeapon(classLoaout.Primary);

            int index = weaponList.AllWeapons.FindIndex(x => x.GunID == classLoaout.Primary);
            UI.GetSlot(0)?.SetupWeapon(weaponList.AllWeapons[index]);

            index = weaponList.AllWeapons.FindIndex(x => x.GunID == classLoaout.Secondary);
            UI.GetSlot(1)?.SetupWeapon(weaponList.AllWeapons[index]);

            index = weaponList.AllWeapons.FindIndex(x => x.GunID == classLoaout.Perks);
            UI.GetSlot(2)?.SetupWeapon(weaponList.AllWeapons[index]);

            index = weaponList.AllWeapons.FindIndex(x => x.GunID == classLoaout.Letal);
            UI.GetSlot(3)?.SetupWeapon(weaponList.AllWeapons[index]);
        }

        /// <summary>
        /// 
        /// </summary>
        public bl_PlayerClassLoadout GetWeaponLoadoutList(PlayerClass playerClass)
        {
            var loadout = ClassManager.AssaultClass;
            switch (playerClass)
            {
                case PlayerClass.Engineer:
                    loadout = ClassManager.EngineerClass;
                    break;
                case PlayerClass.Support:
                    loadout = ClassManager.SupportClass;
                    break;
                case PlayerClass.Recon:
                    loadout = ClassManager.ReconClass;
                    break;
            }
            return loadout;
        }

        /// <summary>
        /// 
        /// </summary>
        public ClassWeapons GetLoadoutClass(PlayerClass playerClass)
        {
            var loadout = assaultWeapons;
            switch (playerClass)
            {
                case PlayerClass.Engineer:
                    loadout = engineerWeapons;
                    break;
                case PlayerClass.Support:
                    loadout = supportWeapons;
                    break;
                case PlayerClass.Recon:
                    loadout = reconWeapons;
                    break;
            }
            return loadout;
        }

        private static bl_ClassCustomize _instance;
        public static bl_ClassCustomize Instance
        {
            get
            {
                if (_instance == null) { _instance = FindObjectOfType<bl_ClassCustomize>(); }
                return _instance;
            }
        }

#if UNITY_EDITOR
        public void RefreshLists()
        {
            if (!gameObject.activeInHierarchy) return;
            assaultWeapons.UpdateList(this);
            engineerWeapons.UpdateList(this);
            supportWeapons.UpdateList(this);
            reconWeapons.UpdateList(this);
        }

        private void OnValidate()
        {
            if (!gameObject.activeInHierarchy) return;
            RefreshLists();
        }
#endif
    }
}
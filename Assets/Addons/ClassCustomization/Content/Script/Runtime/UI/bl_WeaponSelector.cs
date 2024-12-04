using MFPS.Internal;
using MFPS.Internal.Structures;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace MFPS.Addon.ClassCustomization
{
    public class bl_WeaponSelector : MonoBehaviour
    {
        #region Public members
        [SerializeField] private UIListHandler categoriesListHandler = null;
        [SerializeField] private UIListHandler weaponsListHandler = null;
        [SerializeField] private TextMeshProUGUI titleText = null;
        public bl_WeaponsDetailsWindow weaponDetails;

        public class Category
        {
            public string Name;
            public GunType WeaponType;
        }

        public List<WeaponItemData> CurrentWeaponList
        {
            get;
            set;
        }

        public List<WeaponItemData> CurrentWeaponCategoryList
        {
            get;
            set;
        }

        public bl_SlotCardUI CurrentSlotUI
        {
            get;
            set;
        }

        private List<Category> categories;
        #endregion

        private Dictionary<int, bl_CategoryButton> catButtonsCache;

        /// <summary>
        /// 
        /// </summary>
        public void SelectWeaponInCategory(WeaponItemData weapon)
        {
            CurrentSlotUI.SetupWeapon(weapon);
            bl_ClassCustomize.Instance.ChangeSlotClass(weapon, CurrentSlotUI.slotId);
            bl_ClassCustomizationUI.Instance.OpenWindow("loadout");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="weaponList"></param>
        public void PrepareList(List<WeaponItemData> weaponList, bl_SlotCardUI slot)
        {
            CurrentWeaponList = weaponList;
            CurrentSlotUI = slot;

            PrepareCategories();
            ShowCategory(0);

            if(titleText != null)
            {
                titleText.text = $"{slot.key.ToUpper()} WEAPON SELECT";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void PrepareCategories()
        {
            categories = new List<Category>();
            foreach (var item in CurrentWeaponList)
            {
                if (categories.Exists(x => x.Name == item.Type.ToString())) continue;

                categories.Add(new Category()
                {
                    Name = item.Type.ToString(),
                    WeaponType = item.Type,
                });
            }

            categoriesListHandler.Clear();

            catButtonsCache = new Dictionary<int, bl_CategoryButton>();
            for (int i = 0; i < categories.Count; i++)
            {
                var script = categoriesListHandler.InstatiateAndGet<bl_CategoryButton>();
                script.Setup(categories[i], i, this);
                catButtonsCache.Add(i, script);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="catId"></param>
        public void ShowCategory(int catId)
        {
            var cat = categories[catId];
            var list = CurrentWeaponList.Where(x => x.Type == cat.WeaponType).ToList();

            ShowWeaponList(list);

            foreach (var item in catButtonsCache)
            {
                item.Value.Selected(false);
            }
            catButtonsCache[catId].Selected(true);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ShowWeaponList(List<WeaponItemData> weapons)
        {
            CurrentWeaponCategoryList = weapons;
            weaponsListHandler.Clear();
            
            for (int i = 0; i < weapons.Count; i++)
            {
                if (weapons[i].Info.Unlockability.UnlockMethod == MFPSItemUnlockability.UnlockabilityMethod.Hidden) continue;
                
                var script = weaponsListHandler.InstatiateAndGet<bl_ClassInfoUI>();
                script.Setup(weapons[i], CurrentSlotUI, i);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="weapon1"></param>
        /// <param name="weapon2"></param>
        public void CompareWeapons(WeaponItemData weapon1, WeaponItemData weapon2)
        {
            if(weaponDetails != null)
            {
                weaponDetails.CompareWeapons(weapon1, weapon2);
            }
        }
    }
}
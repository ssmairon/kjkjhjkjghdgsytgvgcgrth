using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MFPS.Addon.ClassCustomization
{
    public class bl_CategoryButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;

        private int catId;
        private bl_WeaponSelector weaponSelector;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cat"></param>
        /// <param name="id"></param>
        public void Setup(bl_WeaponSelector.Category cat, int id, bl_WeaponSelector selector)
        {
            nameText.text = bl_StringUtility.NicifyVariableName(cat.Name).ToUpper();
            catId = id;
            weaponSelector = selector;
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnClick()
        {
            weaponSelector.ShowCategory(catId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selected"></param>
        public void Selected(bool selected)
        {
            GetComponent<Button>().interactable = !selected;
        }
    }
}
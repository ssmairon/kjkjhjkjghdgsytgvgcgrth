using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace MFPS.Addon.ClassCustomization
{
    public class bl_SlotCardUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public string key;
        public int slotId;
        public TextMeshProUGUI slotNameText = null;
        [SerializeField] private TextMeshProUGUI itemNameText = null;
        [SerializeField] private TextMeshProUGUI itemTypeText = null;
        [SerializeField] private Image iconImage = null;
        [SerializeField] private GameObject highlightUI = null;

        private CanvasGroup m_alpha;
        private CanvasGroup Alpha
        {
            get
            {
                if (m_alpha == null)
                {
                    m_alpha = GetComponent<CanvasGroup>();
                }
                return m_alpha;
            }
        }
        
        public WeaponItemData CurrentWeapon
        {
            get;
            set;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="weapon"></param>
        public void SetupWeapon(WeaponItemData weaponData)
        {
            var weapon = weaponData.Info;
            CurrentWeapon = weaponData;
            itemNameText.text = weapon.Name;
            itemTypeText.text = bl_StringUtility.NicifyVariableName(weapon.Type.ToString());
            iconImage.sprite = weapon.GunIcon;
            Alpha.alpha = 0.7f;
            if(highlightUI != null) { highlightUI.SetActive(false); }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnClick()
        {
            bl_ClassCustomizationUI.Instance.OpenWindow("weapon-selector");

            var weaponList = bl_ClassCustomize.Instance.GetSupportedWeaponsForSlot(slotId);
            bl_ClassCustomizationUI.Instance.weaponSelector.PrepareList(weaponList, this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wait"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        IEnumerator Fade(float wait, float origin, float target, float speed = 2)
        {
            if (wait > 0) yield return new WaitForSeconds(wait);

            float d = 0;
            while (d < 1)
            {
                d += Time.deltaTime * speed;
                Alpha.alpha = Mathf.Lerp(origin, target, d);
                yield return null;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (highlightUI != null) { highlightUI.SetActive(true); }
            StopAllCoroutines();
            StartCoroutine(Fade(0, Alpha.alpha, 1, 4));
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            if (highlightUI != null) { highlightUI.SetActive(false); }
            StopAllCoroutines();
            StartCoroutine(Fade(0, Alpha.alpha, 0.7f));
        }
    }
}
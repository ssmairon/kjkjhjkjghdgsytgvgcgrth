using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;

namespace MFPS.Addon.ClassCustomization
{
    public class bl_ClassInfoUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public float idleAlpha = 0.5f;
        public Image Icon;
        public TextMeshProUGUI NameText;
        [SerializeField] private GameObject equippedUI = null;
        [SerializeField] private GameObject LevelLock = null;
        [SerializeField] private CanvasGroup Alpha = null;

        private bl_SlotCardUI slot;
        private WeaponItemData weaponData;
        private bool interactable = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="listId"></param>
        public void Setup(WeaponItemData info, bl_SlotCardUI slotInfo, int index)
        {
            slot = slotInfo;
            weaponData = info;

            Icon.sprite = info.Info.GunIcon;
            NameText.text = info.Info.Name.ToUpper();
            interactable = !bl_ClassManager.Instance.isEquiped(info.GunID, bl_ClassManager.Instance.playerClass);

            int gunID = bl_GameData.Instance.GetWeaponID(info.Info.Name);
            if (info.Info.Unlockability.IsUnlocked(gunID))
            {
                LevelLock.SetActive(false);
                interactable = true;
            }
            else
            {
                LevelLock.SetActive(true);
                interactable = false;
            }
            StopAllCoroutines();
            StartCoroutine(Fade(index * 0.04f, 0, idleAlpha));

            if (equippedUI != null)
            {
                equippedUI.SetActive(info.GunID == slotInfo.CurrentWeapon.GunID);
            }

            if(index == 0)
            {
                bl_ClassCustomizationUI.Instance.weaponSelector.CompareWeapons(slot.CurrentWeapon, weaponData);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ChangeSlot()
        {
            bl_ClassCustomizationUI.Instance.weaponSelector.SelectWeaponInCategory(weaponData);
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
            if (!interactable) return;

            ChangeSlot();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            StopAllCoroutines();
            StartCoroutine(Fade(0, Alpha.alpha, 1, 4));

            bl_ClassCustomizationUI.Instance.weaponSelector.CompareWeapons(slot.CurrentWeapon, weaponData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopAllCoroutines();
            StartCoroutine(Fade(0, Alpha.alpha, idleAlpha));
        }
    }
}
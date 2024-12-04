using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MFPS.Addon.ClassCustomization
{
    public class bl_WeaponsDetailsWindow : MonoBehaviour
    {
        [SerializeField] private bl_CompareSlider[] statSliders = null;
        [SerializeField] private TextMeshProUGUI compareWithText = null;

        private Dictionary<string, float> maxStats;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="weapon1"></param>
        /// <param name="weapon2"></param>
        public void CompareWeapons(WeaponItemData weapon1, WeaponItemData weapon2)
        {
            CalculateMaximums();

            statSliders[0].Compare((float)weapon1.Info.Accuracy, (float)weapon2.Info.Accuracy, maxStats["accuracy"], true);
            statSliders[1].Compare((float)weapon1.Info.Damage, (float)weapon2.Info.Damage, maxStats["damage"], true);
            statSliders[2].Compare((float)weapon1.Info.Range, (float)weapon2.Info.Range, maxStats["range"], true);
            statSliders[3].Compare((float)weapon1.Info.FireRate, (float)weapon2.Info.FireRate, maxStats["rate"], false);
            statSliders[4].Compare((float)weapon1.Info.Weight, (float)weapon2.Info.Weight, maxStats["mobility"], false);

            if(compareWithText != null) { compareWithText.text = $"Stats compared to {weapon1.Name}"; }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CalculateMaximums()
        {
            if (maxStats != null) return;

            maxStats = new Dictionary<string, float>()
            {
                { "accuracy", 0 },  { "damage", 0 }, { "range", 0 }, { "rate", 0 }, { "mobility", 0 }
            };

            var all = bl_GameData.Instance.AllWeapons;
            foreach (var weapon in all)
            {
                maxStats["accuracy"] = Mathf.Max(maxStats["accuracy"], (float)weapon.Accuracy);
                maxStats["damage"] = Mathf.Max(maxStats["damage"], (float)weapon.Damage);
                maxStats["range"] = Mathf.Max(maxStats["range"], (float)weapon.Range);
                maxStats["rate"] = Mathf.Max(maxStats["rate"], (float)weapon.FireRate);
                maxStats["mobility"] = Mathf.Max(maxStats["mobility"], (float)weapon.Weight);
            }
        }
    }
}
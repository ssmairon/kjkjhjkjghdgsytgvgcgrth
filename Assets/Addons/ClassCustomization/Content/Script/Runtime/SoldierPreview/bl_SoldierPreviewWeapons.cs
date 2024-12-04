using UnityEngine;

namespace MFPS.Addon.ClassCustomization
{
    public class bl_SoldierPreviewWeapons : MonoBehaviour
    {
        public int GunID;
        public GunType gunType;
        public Transform leftHandIK;

        private void OnEnable()
        {
            if (bl_SoldierPreview.Instance != null)
            {
                bl_SoldierPreview.Instance.SetActiveWeapon(this);
            }

#if CUSTOMIZER
            var customizer = GetComponent<bl_CustomizerWeapon>();
            if (customizer != null)
            {
                customizer.LoadAttachments();
                customizer.ApplyAttachments();
            }
#endif
        }
    }
}
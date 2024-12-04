using System;

namespace MFPS.Addon.ClassCustomization
{
    [Serializable]
    public class WeaponItemData
    {
        public string Name;
        public int GunID;
        public bool isEnabled = true;

        private bl_GunInfo m_gunInfo = null;
        /// <summary>
        /// Weapon information
        /// </summary>
        public bl_GunInfo Info
        {
            get
            {
                if (m_gunInfo == null)
                {
                    m_gunInfo = bl_GameData.Instance.GetWeapon(GunID);
                }
                return m_gunInfo;
            }
        }

        public GunType Type => Info.Type;
    }
}
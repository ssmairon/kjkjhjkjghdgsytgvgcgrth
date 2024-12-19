using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MFPS.Internal.Structures
{
    public class TPPlayerWeaponPositions : MonoBehaviour
    {
        [LovattoToogle] public bool SearchByName = true;
        public List<PositionInfo> TPWeapons = new List<PositionInfo>();

        public List<bl_NetworkGun> Sources;

        [ContextMenu("Fetch Info")]
        public void FetchInfo()
        {
            for (int i = 0; i < Sources.Count; i++)
            {
                var w = Sources[i];

                var pi = new PositionInfo();
                pi.Position = w.transform.localPosition;
                pi.Rotation = w.transform.localRotation;
                pi.Size = w.transform.localScale;

                if (w.LeftHandPosition != null)
                {
                    pi.IKTargetPosition = w.LeftHandPosition.localPosition;
                    pi.IKTargetRotation = w.LeftHandPosition.localRotation;
                }

                pi.GunID = w.LocalGun.GunID;
                var gunInfo = bl_GameData.Instance.GetWeapon(pi.GunID);
                pi.WeaponName = gunInfo.Name;

                TPWeapons.Add(pi);
            }
        }

        public void ApplyModificationsToPlayer(bl_PlayerNetwork player)
        {
            for (int i = 0; i < TPWeapons.Count; i++)
            {
                var info = TPWeapons[i];
                bl_NetworkGun tpw = null;
                if (SearchByName) tpw = player.NetworkGuns.Find(x => x.Info.Name == info.WeaponName);
                else tpw = player.NetworkGuns.Find(x => x.GetWeaponID == info.GunID);

                if (tpw == null)
                {
                    Debug.LogWarning($"The weapon {info.WeaponName} couldn't be found in the player prefab, maybe it have a different name?");
                    continue;
                }

                tpw.transform.localPosition = info.Position;
                tpw.transform.localRotation = info.Rotation;
                tpw.transform.localScale = info.Size;

                if (tpw.LeftHandPosition != null)
                {
                    tpw.LeftHandPosition.localPosition = info.IKTargetPosition;
                    tpw.LeftHandPosition.localRotation = info.IKTargetRotation;
                }
#if UNITY_EDITOR
                EditorUtility.SetDirty(tpw.transform);
#endif
            }
        }

        [Serializable]
        public class PositionInfo
        {
            public string WeaponName;
            public int GunID;
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Size;

            public Vector3 IKTargetPosition;
            public Quaternion IKTargetRotation;
        }
    }
}
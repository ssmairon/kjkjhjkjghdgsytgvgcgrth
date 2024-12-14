using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using MFPSEditor;

namespace MFPS.Others
{
    public class bl_WeaponPackIntegration : MonoBehaviour
    {
        [Serializable]
        public class DefaultsIds
        {
            public string WeaponName;
            public string GameObjectName;
            public int DefaultGunID;

            public string GetObjectName()
            {
                return WeaponName.Replace(" [FP]", "");
            }
        }

        [Serializable]
        public class DefaultLoadouts
        {
            public List<string> WeaponNames;
            public bl_PlayerClassLoadout Loadout;
        }

        public List<bl_GunInfo> GunData = new List<bl_GunInfo>();
        public bl_WeaponExported[] weaponPack;
        public List<DefaultsIds> defaultData;
        public List<DefaultLoadouts> loadouts;

        [ContextMenu("Get Info")]
        void GetInfo()
        {
            GunData.Clear();
            defaultData = new List<DefaultsIds>();
            var all = bl_GameData.Instance.AllWeapons;
            int startIndex = all.ToList().FindIndex(x => x.Name == "Rifle02");
            int stopIndex = all.ToList().FindIndex(x => x.Name == "SniperRifle2");

            for (int i = 0; i < all.Count; i++)
            {
                if (i > stopIndex) break;
                if (i < startIndex) continue;

                GunData.Add(all[i]);

                var defaults = new DefaultsIds()
                {
                    WeaponName = all[i].Name,
                    DefaultGunID = i,
                };

                var exportInfo = GetWeapon(i);
                if (exportInfo != null)
                {
                    defaults.GameObjectName = exportInfo.FPWeapon.name;
                }

                defaultData.Add(defaults);
            }

            foreach (var loadout in loadouts)
            {
                loadout.WeaponNames = new List<string>();
                loadout.WeaponNames.Add(all[loadout.Loadout.Primary].Name);
                loadout.WeaponNames.Add(all[loadout.Loadout.Secondary].Name);
                loadout.WeaponNames.Add(all[loadout.Loadout.Perks].Name);
                loadout.WeaponNames.Add(all[loadout.Loadout.Letal].Name);
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="GunID"></param>
        /// <returns></returns>
        public bl_WeaponExported GetWeapon(int GunID)
        {
            int index = weaponPack.ToList().FindIndex(x => x.FPWeapon.GunID == GunID);
            if (index == -1) return null;

            return weaponPack[index];
        }

        /// <summary>
        /// 
        /// </summary>
        public void VerifyLoadouts()
        {
            var gameData = bl_GameData.Instance.AllWeapons;
            foreach (var loadout in loadouts)
            {
                int gdIndex = gameData.FindIndex(x => x.Name == loadout.WeaponNames[0]);
                if (gdIndex != -1 && gdIndex != loadout.Loadout.Primary)
                {
                    loadout.Loadout.Primary = gdIndex;
                }

                gdIndex = gameData.FindIndex(x => x.Name == loadout.WeaponNames[1]);
                if (gdIndex != -1 && gdIndex != loadout.Loadout.Secondary)
                {
                    loadout.Loadout.Secondary = gdIndex;
                }

                gdIndex = gameData.FindIndex(x => x.Name == loadout.WeaponNames[2]);
                if (gdIndex != -1 && gdIndex != loadout.Loadout.Perks)
                {
                    loadout.Loadout.Perks = gdIndex;
                }

                gdIndex = gameData.FindIndex(x => x.Name == loadout.WeaponNames[3]);
                if (gdIndex != -1 && gdIndex != loadout.Loadout.Letal)
                {
                    loadout.Loadout.Letal = gdIndex;
                }

#if UNITY_EDITOR
                EditorUtility.SetDirty(loadout.Loadout);
#endif
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool VerifyExportsIds()
        {
            bool hasDifferences = false;
            var gameData = bl_GameData.Instance.AllWeapons;
            for (int i = 0; i < weaponPack.Length; i++)
            {
                var weapon = weaponPack[i];
                if (weapon == null) continue;

                int gameDataIndex = gameData.FindIndex(x => x.Name == weapon.WeaponInfo.Name);
                if (gameDataIndex == -1)
                {
                    Debug.Log($"The weapon {weapon.WeaponInfo.Name} has not been setup in GameData or the weapon name had change.");
                    continue;
                }

                if (weapon.FPWeapon.GunID == gameDataIndex) continue;

                weapon.FPWeapon.GunID = gameDataIndex;
                if (weapon.WeaponInfo.PickUpPrefab != null) weapon.WeaponInfo.PickUpPrefab.GunID = gameDataIndex;
#if UNITY_EDITOR
                EditorUtility.SetDirty(weapon.FPWeapon);
                EditorUtility.SetDirty(weapon.WeaponInfo.PickUpPrefab);
                EditorUtility.SetDirty(weapon);
#endif
                hasDifferences = true;
                // Debug.Log($"The export weapon {weapon.WeaponInfo.Name} has a different ID");
            }

            return hasDifferences;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        public void VerifyPlayerWeaponsIds(bl_PlayerReferences player)
        {
            var fpWeapons = player.gunManager.AllGuns;
            var gameData = bl_GameData.Instance.AllWeapons;

            foreach (var weapon in fpWeapons)
            {
                if (weapon == null) continue;

                int defaultIdIndex = defaultData.FindIndex(x => x.DefaultGunID == weapon.GunID);
                if (defaultIdIndex == -1) continue;

                var defaultInfo = defaultData[defaultIdIndex];

                int gameDataIndex = gameData.FindIndex(x => x.Name == defaultInfo.WeaponName);
                if (gameDataIndex == -1) continue;

                // the GunID is the correct one
                if (gameDataIndex == defaultInfo.DefaultGunID) continue;

                if (weapon.name.ToLower() != defaultInfo.GetObjectName().ToLower())
                {
                    Debug.LogWarning($"Can't automatically assign the correct GunID to the '{weapon.name}' FPWeapon, please assign manually in the inspector of bl_Gun.cs");
                    continue;
                }

                weapon.GunID = gameDataIndex;

#if UNITY_EDITOR
                EditorUtility.SetDirty(weapon);
                EditorUtility.SetDirty(player);
#endif
                //Debug.Log($"Adjusted GunID of the {weapon.gameObject.name} FPWeapon");
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(bl_WeaponPackIntegration))]
    public class bl_WeaponPack1IntegrationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.indentLevel++;
            GUILayout.BeginVertical("box");
            var property = serializedObject.FindProperty("GunData");
            serializedObject.Update();
            EditorGUILayout.PropertyField(property, true);
            var propertyPack = serializedObject.FindProperty("weaponPack");
            EditorGUILayout.PropertyField(propertyPack, true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultData"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("loadouts"), true);
            serializedObject.ApplyModifiedProperties();
            GUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }
    }
#endif
}
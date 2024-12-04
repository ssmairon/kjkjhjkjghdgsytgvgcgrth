using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MFPS.Addon.ClassCustomization
{
    public class bl_SoldierPreview : MonoBehaviour
    {
        public Transform soldierReference;
        public Animator playerAnimator;

        public bl_PlayerNetwork tempPlayer;
        public List<bl_SoldierPreviewWeapons> weapons = new List<bl_SoldierPreviewWeapons>();
        public bl_SoldierPreviewWeapons ActiveWeapon { get; set; }


        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            playerAnimator.SetInteger("UpperState", 0);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ShowWeapon(int gunID)
        {
            weapons.ForEach(x => x.gameObject.SetActive(false));
            if(weapons.Exists(x => x.GunID == gunID))
            {
                weapons.Find(x => x.GunID == gunID).gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetActiveWeapon(bl_SoldierPreviewWeapons weapon)
        {
            playerAnimator.SetInteger("GunType", (int)weapon.gunType);
            ActiveWeapon = weapon;
        }

        [ContextMenu("Integrate")]
        public void IntegratePlayer()
        {
            if (tempPlayer == null) return;

            if (soldierReference != null) DestroyImmediate(soldierReference.gameObject);
            weapons = new List<bl_SoldierPreviewWeapons>();

            var newPlayer = Instantiate(tempPlayer.gameObject) as GameObject;

            soldierReference = newPlayer.transform;
            soldierReference.parent = transform;
            soldierReference.localPosition = Vector3.zero;
            soldierReference.localEulerAngles = Vector3.zero;

            soldierReference = newPlayer.GetComponent<bl_PlayerSettings>().RemoteObjects.transform;
            soldierReference.parent = transform;
            playerAnimator = soldierReference.GetComponentInChildren<Animator>();
            var spa = playerAnimator.gameObject.AddComponent<bl_SoldierPreviewAnimation>();
#if UNITY_EDITOR
            EditorUtility.SetDirty(spa);
            EditorUtility.SetDirty(playerAnimator.gameObject);
#endif
            FetchNetworkWeapons(soldierReference);

            var cj = soldierReference.GetComponentsInChildren<CharacterJoint>(true);
            for (int i = 0; i < cj.Length; i++)
                DestroyImmediate(cj[i]);

            var rig = soldierReference.GetComponentsInChildren<Rigidbody>(true);
            for (int i = 0; i < rig.Length; i++)
                DestroyImmediate(rig[i]);

            var monos = soldierReference.GetComponentsInChildren<MonoBehaviour>(true);
            for (int i = 0; i < monos.Length; i++)
            {
                if (monos[i] is bl_SoldierPreviewWeapons || monos[i] is bl_SoldierPreview || monos[i] is bl_SoldierPreviewAnimation) continue;
#if CUSTOMIZER
                if (monos[i] is bl_CustomizerWeapon) continue;
#endif
                DestroyImmediate(monos[i]);
            }
#if UNITY_EDITOR
            EditorUtility.SetDirty(soldierReference);
#endif
            DestroyImmediate(newPlayer);
        }

        public void FetchNetworkWeapons(Transform root)
        {
            var all = root.GetComponentsInChildren<bl_NetworkGun>(true);
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i].LocalGun == null) continue;

                var w = all[i].gameObject.AddComponent<bl_SoldierPreviewWeapons>();
                w.GunID = all[i].LocalGun.GunID;
                w.gunType = bl_GameData.Instance.GetWeapon(all[i].LocalGun.GunID).Type;
                w.leftHandIK = all[i].LeftHandPosition;
                weapons.Add(w);
                DestroyImmediate(all[i]);
#if UNITY_EDITOR
                EditorUtility.SetDirty(w);
#endif
            }
        }

        private static bl_SoldierPreview _instance;
        public static bl_SoldierPreview Instance
        {
            get
            {
                if (_instance == null) { _instance = FindObjectOfType<bl_SoldierPreview>(); }
                return _instance;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MFPS.Mobile;

public class bl_WeaponMobileSwitcher : MonoBehaviour
{
    public GameObject content;
    [SerializeField] private Image PreviewImage = null;

    private bl_GunManager GunManager;
    private bl_GameManager Manager;
    private bl_GameData Data;
    private bool manualChanged = false;

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        Manager = bl_GameManager.Instance;
        Data = bl_GameData.Instance;
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnEnable()
    {
        bl_EventHandler.onLocalPlayerSpawn += OnLocalSpawn;
        bl_EventHandler.onChangeWeapon += OnWeaponChanged;
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDisable()
    {
        bl_EventHandler.onLocalPlayerSpawn -= OnLocalSpawn;
        bl_EventHandler.onChangeWeapon -= OnWeaponChanged;
    }

    /// <summary>
    /// 
    /// </summary>
    void OnLocalSpawn()
    {
        GunManager = bl_MFPS.LocalPlayerReferences.gunManager;
        Invoke(nameof(TakeCurrent), 1);
    }

    /// <summary>
    /// 
    /// </summary>
    void OnWeaponChanged(int gunID)
    {
        if (manualChanged)
        {
            manualChanged = false;
            return;
        }

        TakeCurrent();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="forwa"></param>
    public void ChangeWeapon(bool forwa)
    {
        if (GunManager == null)
            return;

        int c = 0;
        manualChanged = true;
        if (forwa)
        {
            c = GunManager.SwitchNext();
            bl_TouchHelper.onMobileButton?.Invoke(FPSMobileButton.WeaponRight);
        }
        else
        {
            c = GunManager.SwitchPrevious();
            bl_TouchHelper.onMobileButton?.Invoke(FPSMobileButton.WeaponLeft);
        }
        PreviewImage.sprite = Data.GetWeapon(GunManager.PlayerEquip[c].GunID).GunIcon;
    }

    /// <summary>
    /// 
    /// </summary>
    void TakeCurrent()
    {
        if (GunManager == null || GunManager.CurrentGun == null)
        {
            content.SetActive(false);
            return;
        }

        PreviewImage.sprite = Data.GetWeapon(GunManager.CurrentGun.GunID).GunIcon;
        content.SetActive(true);
    }
}
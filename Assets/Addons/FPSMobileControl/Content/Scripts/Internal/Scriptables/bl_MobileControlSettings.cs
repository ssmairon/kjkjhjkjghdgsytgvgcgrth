using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MobileControlSettings", menuName = "MFPS/Mobile/Settings")]
public class bl_MobileControlSettings : ScriptableObject
{
    [LovattoToogle] public bool autoAimWhenFire = false;
    [LovattoToogle] public bool autoAimForSniper = true;
    [LovattoToogle] public bool disableRecoil = true;
    public List<GunType> autoAimWeaponTypes;

    [Header("Auto Fire Settings")]
    public float autoFireDelay = 0.5f;
    public float detectRange = 50;
    [Range(1, 7)] public float SniperRangeMultiplier = 3;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gunType"></param>
    /// <returns></returns>
    public static bool CanAutoAimWeaponType(GunType gunType)
    {
        if (Instance == null) return false;

        return Instance.autoAimWeaponTypes.Contains(gunType);
    }
    
    private static bl_MobileControlSettings instance;
    public static bl_MobileControlSettings Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<bl_MobileControlSettings>("MobileControlSettings");
            }
            return instance;
        }
    }
}
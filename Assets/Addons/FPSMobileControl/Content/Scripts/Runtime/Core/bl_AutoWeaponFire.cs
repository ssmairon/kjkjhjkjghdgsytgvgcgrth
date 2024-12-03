using UnityEngine;
using UnityEngine.UI;

public class bl_AutoWeaponFire : bl_PhotonHelper
{
    [Header("References")]
    [SerializeField] private GameObject BarUI = null;
    [SerializeField] private Image BarImage = null;

    private Camera PlayerCamera;
    private RaycastHit Ray;
    private bool HitSome = false;
    private float waitTime = 0;
    private bl_GunInfo ActualGun;
    private float LastOverTime = 0;

    /// <summary>
    ///
    /// </summary>
    private void Awake()
    {
#if MFPSM
        if (!bl_GameData.Instance.AutoWeaponFire)
        {
            this.enabled = false;
        }
#endif
        BarUI.SetActive(false);
    }

    /// <summary>
    ///
    /// </summary>
    private void OnEnable()
    {
        bl_EventHandler.onLocalPlayerSpawn += OnLocalSpawn;
        bl_EventHandler.onLocalPlayerDeath += OnLocalDeath;
        bl_EventHandler.onChangeWeapon += OnChangeWeapon;
        if (PlayerCamera == null && bl_MFPS.LocalPlayerReferences != null)
        {
            PlayerCamera = bl_MFPS.LocalPlayerReferences.playerCamera;
        }
    }

    /// <summary>
    ///
    /// </summary>
    private void OnDisable()
    {
        bl_EventHandler.onLocalPlayerSpawn -= OnLocalSpawn;
        bl_EventHandler.onLocalPlayerDeath -= OnLocalDeath;
        bl_EventHandler.onChangeWeapon -= OnChangeWeapon;
    }

    /// <summary>
    ///
    /// </summary>
    private void OnLocalSpawn()
    {
        PlayerCamera = bl_MFPS.LocalPlayerReferences.playerCamera;
    }

    /// <summary>
    ///
    /// </summary>
    private void OnLocalDeath()
    {
        ResetParamt();
        BarUI.SetActive(false);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="GunID"></param>
    private void OnChangeWeapon(int GunID)
    {
        ActualGun = bl_GameData.Instance.GetWeapon(GunID);
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public bool Fire()
    {
        if (PlayerCamera == null)
            return false;

        float range = bl_MobileControlSettings.Instance.detectRange;
        if (ActualGun != null)
        {
            if (ActualGun.Type == GunType.Knife) { range = 3; }
            else if (ActualGun.Type == GunType.Grenade) { return false; }
            else if (ActualGun.Type == GunType.Sniper) { range = bl_MobileControlSettings.Instance.detectRange * bl_MobileControlSettings.Instance.SniperRangeMultiplier; }
        }

        Ray r = new Ray(PlayerCamera.transform.position, PlayerCamera.transform.forward);
        Debug.DrawRay(PlayerCamera.transform.position, PlayerCamera.transform.forward * range, Color.yellow);
        if (Physics.Raycast(r, out Ray, range))
        {

            // if hit a player or bot
            if (Ray.transform.CompareTag(bl_MFPS.HITBOX_TAG) || Ray.transform.CompareTag(bl_MFPS.AI_TAG))
            {
                // if is a multiple team mode
                if (!isOneTeamMode)
                {
                    var references = Ray.transform.root.GetComponent<bl_PlayerReferencesCommon>();
                    if (references == null) return false;

                    // if the player we are aiming at is a teammate
                    if (references.PlayerTeam == bl_MFPS.LocalPlayer.Team) return false; // don't auto shoot
                }

                return OnOverEnemy();
            }
            else
            {
                ResetParamt();
            }           
        }
        else
        {
            ResetParamt();
        }
        return false;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    private bool OnOverEnemy()
    {
        HitSome = true;
        float wait = bl_MobileControlSettings.Instance.autoFireDelay;
        if (waitTime >= wait)
        {
            BarUI.SetActive(false);
            LastOverTime = Time.time;
            return true;
        }
        else
        {
            waitTime += Time.deltaTime;
            waitTime = Mathf.Clamp(waitTime, 0, wait);
            BarUI.SetActive(true);
            BarImage.fillAmount = waitTime / wait;
            return false;
        }
    }

    /// <summary>
    ///
    /// </summary>
    private void ResetParamt()
    {
        if (HitSome && (Time.time - LastOverTime) > 0.22f)
        {
            HitSome = false;
            BarUI.SetActive(false);
            BarImage.fillAmount = 0;
            waitTime = 0;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static void OnManualFire()
    {
        if (Instance == null) return;

        Instance.ResetParamt();
        Instance.LastOverTime = Time.time;
    }

    private static bl_AutoWeaponFire _instance;

    public static bl_AutoWeaponFire Instance
    {
        get
        {
            if (_instance == null) { _instance = FindObjectOfType<bl_AutoWeaponFire>(); }
            return _instance;
        }
    }
}
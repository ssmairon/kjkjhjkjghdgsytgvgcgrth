using UnityEngine;
using System;
using MFPS.Mobile;

public class bl_TouchHelper : MonoBehaviour
{
    [SerializeField] private GameObject PushToTalkButton;

    #region Events
    public delegate void ButtonEvent();
    public static ButtonEvent OnFireClick;
    public static ButtonEvent OnReload;
    public static ButtonEvent OnCrouch;
    public static ButtonEvent OnJump;
    public static ButtonEvent OnKit;
    public static ButtonEvent OnPause;
    public delegate void BoolEvent(bool b);
    public static BoolEvent OnTransmit;

    public static Action<FPSMobileButton> onMobileButton;
    public static Action<Vector2> OnVehicleDirection; 
    #endregion

    private static bl_TouchHelper m_Instance;
    private bool _fireDown = false;
    
    /// <summary>
    /// Is the fire button pressed?
    /// </summary>
    public bool FireDown
    {
        get => _fireDown;
        set
        {
            if(_fireDown != value)
            {
                OnFirePressChanged(value);
            }
            
            _fireDown = value;
        }
    }
    
    /// <summary>
    /// Is the aim button pressed?
    /// </summary>
    public bool isAim
    {
        get;
        set;
    }
    
    /// <summary>
    /// Current vehicle direction button
    /// </summary>
    public Vector2 VehicleInput { get; set; } = new Vector2();

    #region Unity Methods
    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        bl_EventHandler.onLocalPlayerSpawn += OnSpawn;
        bl_EventHandler.onRoundEnd += OnRoundEnd;
        bl_EventHandler.onGamePause += OnGamePause;
        bl_EventHandler.onLocalPlayerDeath += OnLocalDeath;

        SetMobileCanvasVisible(false);

        if (bl_GameData.isDataCached || bl_PhotonNetwork.OfflineMode)
            PushToTalkButton.SetActive(bl_GameData.Instance.UseVoiceChat);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDestroy()
    {
        bl_EventHandler.onLocalPlayerSpawn -= OnSpawn;
        bl_EventHandler.onLocalPlayerDeath -= OnLocalDeath;
        bl_EventHandler.onRoundEnd -= OnRoundEnd;
        bl_EventHandler.onGamePause -= OnGamePause;
    } 
    #endregion

    /// <summary>
    /// 
    /// </summary>
    void OnSpawn()
    {
        SetMobileCanvasVisible(true);

        isAim = false;
        bl_MobileButtonLayers.Instance.SetActiveButtonGroup(0);
    }

    /// <summary>
    /// 
    /// </summary>
    void OnLocalDeath()
    {
        VehicleInput = Vector2.zero;
    }

    /// <summary>
    /// 
    /// </summary>
    void OnRoundEnd()
    {
        SetMobileCanvasVisible(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isPressed"></param>
    void OnFirePressChanged(bool isPressed)
    {
        if (!bl_MobileControlSettings.CanAutoAimWeaponType(GetCurrentWeaponType())) return;

#if MFPSTPV
        if (bl_CameraViewSettings.IsThirdPerson()) return;
#endif
        
        if (isPressed)
        {
            if (bl_MobileControlSettings.Instance.autoAimWhenFire)
            {
                isAim = true;
            }
        }
        else
        {
            if (bl_MobileControlSettings.Instance.autoAimWhenFire)
            {
                isAim = false;
            }
        }
    }

    #region Button Callbacks
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pause"></param>
    void OnGamePause(bool pause)
    {
        SetMobileCanvasVisible(!pause);

        VehicleInput = Vector2.zero;
    }

    /// <summary>
    /// Called when the fire button is clicked
    /// </summary>
    public void OnFireClicked()
    {
        if(bl_MobileControlSettings.Instance.autoAimForSniper && GetCurrentWeaponType() == GunType.Sniper)
        {
            isAim = true;
            return;
        }

        OnFireClick?.Invoke();
        OnMobileButton(FPSMobileButton.Fire);
    }

    /// <summary>
    /// Called when the fire button is click up
    /// </summary>
    public void OnFireClickUp()
    {
        if (bl_MobileControlSettings.Instance.autoAimForSniper && GetCurrentWeaponType() == GunType.Sniper)
        {
            OnFireClick?.Invoke();
            OnMobileButton(FPSMobileButton.Fire);
            isAim = false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnReloadClicked()
    {
        OnReload?.Invoke();
        OnMobileButton(FPSMobileButton.Reload);
    }

    public void OnCrouchClicked()
    {
        OnCrouch?.Invoke();
        OnMobileButton(FPSMobileButton.Crouch);
    }

    public void OnJumpClicked()
    {
        OnJump?.Invoke();
        OnMobileButton(FPSMobileButton.Jump);
    }

    public void OnKitClicked()
    {
        OnKit?.Invoke();
        OnMobileButton(FPSMobileButton.Kit);
    }

    public void OnPauseClick()
    {
        OnPause?.Invoke();
        OnMobileButton(FPSMobileButton.Pause);
    }

    public void EnableMicro(bool yes)
    {
        OnTransmit?.Invoke(yes);
        OnMobileButton(FPSMobileButton.Voice);
    }

    public void OnPushToTalkChange(bool active)
    {
        PushToTalkButton.SetActive(active);
    }

    public void OnVehicleHorizontal(float dir)
    {
        var v = VehicleInput;
        v.x = dir;
        VehicleInput = v;
        OnVehicleDirection?.Invoke(VehicleInput);
    }

    public void OnVehicleVertical(float dir)
    {
        var v = VehicleInput;
        v.y = dir;
        VehicleInput = v;
        OnVehicleDirection?.Invoke(VehicleInput);
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetAim()
    {
        isAim = !isAim;
        OnMobileButton(FPSMobileButton.Aim);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mobileButton"></param>
    private void OnMobileButton(FPSMobileButton mobileButton)
    {
        onMobileButton?.Invoke(mobileButton);
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnChat()
    {
        bl_RoomChatBase.Instance?.OnSubmit();
        OnMobileButton(FPSMobileButton.Chat);
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="visible"></param>
    public void SetMobileCanvasVisible(bool visible)
    {
        MobileCanvas.enabled = visible;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private GunType GetCurrentWeaponType()
    {
        if (bl_MFPS.LocalPlayerReferences == null) return GunType.None;

        bl_Gun weapon = bl_MFPS.LocalPlayerReferences.gunManager.GetCurrentWeapon();
        if (weapon == null) return GunType.None;

        return weapon.OriginalWeaponType;
    }

    public Canvas m_mobileCanvas;
    public Canvas MobileCanvas
    {
        get
        {
            if(m_mobileCanvas == null)
            {
                m_mobileCanvas = transform.GetComponentInParent<Canvas>();
            }
            return m_mobileCanvas;
        }
    }
    
    public static bl_TouchHelper Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType(typeof(bl_TouchHelper)) as bl_TouchHelper;
            }
            return m_Instance;
        }
    }
}
using MFPS.Runtime.Vehicles;
using UnityEngine;

public class bl_TankInput : bl_MonoBehaviour
{
    public bl_TankController tankController;
    public bl_TankTurret tankTurret;
    public bl_TankCannon tankCannon;
    
    private float horizontal, vertical;
    private float xAxisAcceleration, yAxisAcceleration;
    private bool isAiming = false;

    public float YAxisAcceleration
    {
        get
        {
            if (tankController.realWheels)
            {
                return yAxisAcceleration = bl_GameInput.Vertical;
            }
            return yAxisAcceleration = tankController.Accelerate(yAxisAcceleration, bl_GameInput.Vertical, tankController.yAxisAccelerationStep, tankController.yAxisInertion, xAxis: false);
        }
    }

    public float XAxisAcceleration
    {
        get
        {
            if (tankController.realWheels)
            {
                return xAxisAcceleration = bl_GameInput.Horizontal;
            }
            return xAxisAcceleration = tankController.Accelerate(xAxisAcceleration, bl_GameInput.Horizontal, tankController.xAxisAccelerationStep, tankController.xAxisInertion, xAxis: true);
        }

    }

    protected override void Awake()
    {
        base.Awake();
        if (tankCannon == null) tankCannon = GetComponent<bl_TankCannon>();
    }

    /// <summary>
    /// 
    /// </summary>
    public override void OnFixedUpdate()
    {
        tankController.Move(vertical, horizontal);
    }

    /// <summary>
    /// 
    /// </summary>
    public override void OnUpdate()
    {
        if (bl_UtilityHelper.isMobile) return;
        
        vertical = bl_GameInput.Vertical;
        horizontal = bl_GameInput.Horizontal;

        if (bl_GameInput.Fire(GameInputType.Down))
        {
            OnFireInput();
        }

        if (bl_GameInput.ChangeView())
        {
            if (tankTurret != null)
            {
                tankTurret.SwitchCameraView();
            }
        }

        if (bl_GameInput.Aim())
        {
            if (!isAiming)
            {
                bl_VehicleCamera.Instance.SetZoom(new bl_VehicleCamera.ZoomTransition()
                {
                    TargetZoom = tankTurret.aimZoom,
                    TransitionDuration = tankTurret.transitionDuration,
                    Curve = tankTurret.aimTransitionCurve
                });
                isAiming = true;
            }
        }
        else
        {
            if (isAiming)
            {
                bl_VehicleCamera.Instance.ResetZoom();
                isAiming = false;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();

#if MFPSM
        bl_TouchHelper.OnVehicleDirection += OnMobileDirection;
        bl_TouchHelper.OnFireClick += OnFireInput;
        bl_TouchHelper.onMobileButton += OnMobileButton;
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void OnDisable()
    {
        base.OnDisable();
        
#if MFPSM
        bl_TouchHelper.OnVehicleDirection -= OnMobileDirection;
        bl_TouchHelper.OnFireClick -= OnFireInput;
        bl_TouchHelper.onMobileButton -= OnMobileButton;
#endif
    }

#if MFPSM
    /// <summary>
    /// 
    /// </summary>
    /// <param name="button"></param>
    void OnMobileButton(MFPS.Mobile.FPSMobileButton button)
    {
        if (button == MFPS.Mobile.FPSMobileButton.Aim)
        {
            isAiming = !isAiming;
            if (isAiming)
            {
                bl_VehicleCamera.Instance.SetZoom(new bl_VehicleCamera.ZoomTransition()
                {
                    TargetZoom = tankTurret.aimZoom,
                    TransitionDuration = tankTurret.transitionDuration,
                    Curve = tankTurret.aimTransitionCurve
                });
            }
            else
            {
                bl_VehicleCamera.Instance.ResetZoom();
            }
        }
    }
#endif
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dir"></param>
    void OnMobileDirection(Vector2 dir)
    {
        vertical = dir.y;
        horizontal = dir.x;
    }

    /// <summary>
    /// 
    /// </summary>
    void OnFireInput()
    {
        tankCannon.TryToFire();
    }
}
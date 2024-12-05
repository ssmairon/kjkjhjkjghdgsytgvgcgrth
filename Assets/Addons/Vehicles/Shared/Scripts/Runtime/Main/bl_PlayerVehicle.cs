using MFPS.Internal.Vehicles;
using MFPS.Runtime.Vehicles;
using UnityEngine;

public class bl_PlayerVehicle : bl_MonoBehaviour
{
    public bl_VehicleManager CurrentVehicle { get; set; }
    private bl_BodyIKHandler armsIK;

    /// <summary>
    /// Called on local client only, when the local client enter in a vehicle
    /// </summary>
    public void LocalEnterInVehicle(bl_VehicleManager vehicle, VehicleSeat seat)
    {
        PlayerReferences.playerRagdoll.IgnoreColliders(vehicle.AllColliders, true);
        EnterInSeat(seat, vehicle.transform);
        bl_CrosshairBase.Instance.Show(false);
        bl_CrosshairBase.Instance.Block = true;
        CurrentVehicle = vehicle;
        PlayerReferences.firstPersonController.State = PlayerState.InVehicle;
        PlayerReferences.weaponBob.Stop();

#if MFPSTPV
        var tpv = PlayerReferences.GetComponent<bl_PlayerCameraSwitcher>();
        if(tpv != null)
        {
            tpv.CanManualySwitchView = false;
        }
#endif

        SetActiveObject(false, seat);
        if (seat.PlayerCanShoot)
        {
            if (IsThirdPerson()) return;

            PlayerReferences.firstPersonController.GetMouseLook().ClampHorizontalRotation(seat.FPViewClamp.x, seat.FPViewClamp.y);
            vehicle.UpdateList.Add(this);
            bl_CrosshairBase.Instance.Block = false;
            bl_CrosshairBase.Instance.Show(true);
        }
        else if (seat.PlayerVisible)
        {
            PlayerReferences.playerSettings.RemoteObjects.SetActive(true);
            PlayerReferences.playerIK.HeadLookTarget = bl_VehicleCamera.Instance.headLook;
            if (seat.IsDriver)
                SetArmsIkToSteer(vehicle);
        }
        else if (!seat.PlayerVisible)
        {
            PlayerReferences.playerSettings.RemoteObjects.SetActive(false);
        }
    }

    /// <summary>
    /// Called on local client only, when the local client exit a vehicle
    /// </summary>
    public void LocalExitVehicle(bl_VehicleManager vehicle, VehicleSeat seat)
    {
        ExitSeat(seat);
        SetActiveObject(true, seat);

        if (seat.PlayerCanShoot)
        {
            if (!IsThirdPerson())
            {
                if (vehicle.UpdateList.Contains(this))
                    vehicle.UpdateList.Remove(this);
            }
        }

#if MFPSTPV
        var tpv = PlayerReferences.GetComponent<bl_PlayerCameraSwitcher>();
        if (tpv != null)
        {
            tpv.CanManualySwitchView = true;
        }
#endif

        PlayerReferences.playerSettings.RemoteObjects.SetActive(IsThirdPerson());
        armsIK = PlayerReferences.playerIK.CustomArmsIKHandler = null;
        PlayerReferences.playerIK.HeadLookTarget = null;

        PlayerReferences.firstPersonController.GetMouseLook().UnClampHorizontal();
        PlayerReferences.playerRagdoll.IgnoreColliders(vehicle.AllColliders, false);
        PlayerReferences.firstPersonController.State = PlayerState.Idle;
        bl_CrosshairBase.Instance.Block = false;
        bl_CrosshairBase.Instance.Show(true);
        CurrentVehicle = null;
    }

    /// <summary>
    /// Called on remote clients when this player enter in vehicle
    /// </summary>
    public void RemoteEnterInVehicle(bl_VehicleManager vehicle, VehicleSeat seat)
    {
        CurrentVehicle = vehicle;
        if (seat.IsDriver) CurrentVehicle.DriverPlayer = PlayerReferences;
        PlayerReferences.playerAnimations.BodyState = PlayerState.InVehicle;
        PlayerReferences.playerNetwork.NetworkBodyState = PlayerState.InVehicle;
        if (!seat.PlayerVisible)
        {
            PlayerReferences.RemotePlayerObjects.SetActive(false);
        }
        else
        {
            PlayerReferences.playerRagdoll.IgnoreColliders(vehicle.AllColliders, true);
            if (seat.IsDriver) SetArmsIkToSteer(vehicle);
        }
        PlayerReferences.characterController.enabled = false;
        EnterInSeat(seat, vehicle.transform, true);
    }

    /// <summary>
    /// 
    /// </summary>
    public void RemoteExitVehicle(bl_VehicleManager vehicle, VehicleSeat seat)
    {
        ExitSeat(seat);
        PlayerReferences.RemotePlayerObjects.SetActive(true);
        PlayerReferences.playerAnimations.BodyState = PlayerState.Idle;
        PlayerReferences.playerRagdoll.IgnoreColliders(vehicle.AllColliders, false);
        if(PlayerReferences.playerIK != null)armsIK = PlayerReferences.playerIK.CustomArmsIKHandler = null;
        if (seat.IsDriver && CurrentVehicle != null)
            CurrentVehicle.DriverPlayer = null;

        CurrentVehicle = null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="seat"></param>
    public void EnterInSeat(VehicleSeat seat, Transform parent, bool remote = false)
    {
        transform.parent = parent;
        seat.SitPlayer(this, remote);
    }

    /// <summary>
    /// 
    /// </summary>
    public void ExitSeat(VehicleSeat seat)
    {
        seat.GetPlayerOut(transform);
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdatePlayerInside()
    {
        PlayerReferences.firstPersonController.UpdateMouseLook();
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetArmsIkToSteer(bl_VehicleManager vehicle)
    {
        if (vehicle.steerWheel.SteerWheel == null) return;

        armsIK = new bl_BodyIKHandler();
        armsIK.Initialize(PlayerReferences.PlayerAnimator);
        armsIK.AddBone(vehicle.steerWheel.SteerWheel, AvatarIKGoal.LeftHand, true, true, 0, 1).SetRightOffset(vehicle.steerWheel.SteeringHandSpace, true);
        armsIK.AddBone(vehicle.steerWheel.SteerWheel, AvatarIKGoal.RightHand, true, true, 0, 1).SetRightOffset(vehicle.steerWheel.SteeringHandSpace, false);
        PlayerReferences.playerIK.CustomArmsIKHandler = armsIK;
    }

    /// <summary>
    /// 
    /// </summary>
    void SetActiveObject(bool active, VehicleSeat seat)
    {
        PlayerReferences.playerSettings.LocalOnlyScripts.ForEach(x => { if (x != null) x.enabled = active; });
        PlayerReferences.characterController.enabled = active;

        if (!active && seat.PlayerCanShoot && !IsThirdPerson()) return;

        PlayerReferences.playerSettings.LocalObjects.SetActive(active);
    }

    public bool IsThirdPerson()
    {
#if MFPSTPV
        return bl_CameraViewSettings.IsThirdPerson();
#else
        return false;
#endif
    }

    private bl_PlayerReferences m_playerReferences;
    public bl_PlayerReferences PlayerReferences
    {
        get
        {
            if (m_playerReferences == null) m_playerReferences = GetComponent<bl_PlayerReferences>();
            return m_playerReferences;
        }
    }
}
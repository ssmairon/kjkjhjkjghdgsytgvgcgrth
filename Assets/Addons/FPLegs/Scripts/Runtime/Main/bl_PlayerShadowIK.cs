using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bl_PlayerShadowIK : bl_MonoBehaviour
{
    #region Public members
    public Transform Target;
    [Header("UPPER BODY")]
    [Range(0, 1)] public float Weight;
    [Range(0, 1)] public float Body;
    [Range(0, 1)] public float Head;
    [Range(0, 1)] public float Eyes;
    [Range(0, 1)] public float Clamp;
    [Range(1, 20)] public float Lerp = 8;

    public Vector3 HandOffset;
    public Vector3 AimSightPosition = new Vector3(0.02f, 0.19f, 0.02f);

    public bool IsCustomHeadTarget { get; set; } = false;
    
    public bl_BodyIKHandler CustomArmsIKHandler
    {
        get;
        set;
    } = null;
    /// <summary>
    /// When this is true, you shouldn't control the arms with IK.
    /// </summary>
    public bool ControlArmsWithIK
    {
        get;
        set;
    } = true;

    public bl_PlayerLegs playerLegs { get; set; }
    #endregion

    #region Private members
    private Animator animator;
    private Vector3 targetPosition;
    private float rightArmIKRotationWeight = 1;
    private float leftArmIkWight = 1;
    private float rightArmIKPositionWeight = 0;
    private Transform m_headTransform, rightUpperArm;
    private float deltaTime = 0;
    private Transform m_headTarget;
    private bl_PlayerIK playerIKSource;
    private Vector3 leftHandPos;
    private Quaternion leftHandRot;
    #endregion

    /// <summary>
    /// 
    /// </summary>
    protected void Start()
    {
        Init();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Init()
    {
        if (playerReferences == null) return;

        playerIKSource = (bl_PlayerIK)playerReferences.playerIK;
        animator = GetComponent<Animator>();
        if (HeadLookTarget == null) HeadLookTarget = Target;

        m_headTransform = animator.GetBoneTransform(HumanBodyBones.Head);
        rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);

        HandOffset = playerIKSource.HandOffset;
        AimSightPosition = playerIKSource.AimSightPosition;
        Weight = playerIKSource.Weight;
        Body = playerIKSource.Body;
        Head = playerIKSource.Head;
        Clamp = playerIKSource.Clamp;
        Target = playerIKSource.Target;
    }

    /// <summary>
    /// Called from the Animator after the animation update
    /// </summary>
    void OnAnimatorIK(int layer)
    {
        if (playerIKSource == null) return;
        
        HeadLookTarget = playerIKSource.HeadLookTarget;
        CustomArmsIKHandler = playerIKSource.CustomArmsIKHandler;
        
        if (HeadLookTarget == null || animator == null)
            return;

        deltaTime = Time.deltaTime;

        if (layer == 0) BottomBody();
        else if (layer == 1) UpperBody();
    }

    /// <summary>
    /// Control the legs IK
    /// </summary>
    void BottomBody()
    {
        animator.SetLookAtWeight(Weight, Body, Head, Eyes, Clamp);
        targetPosition = Vector3.Slerp(targetPosition, HeadLookTarget.position, deltaTime * 8);
        animator.SetLookAtPosition(targetPosition);
    }

    /// <summary>
    /// Control the arms and head IK bones
    /// </summary>
    void UpperBody()
    {
        CustomArmsIKHandler = playerIKSource.CustomArmsIKHandler;
        //If there's another script handling the arms IK
        if (playerIKSource.CustomArmsIKHandler != null)
        {
            
            CustomArmsIKHandler.OnUpdate();
        }
        else if (LeftHandTarget != null && ControlArmsWithIK)
        {
            ArmsIK();
        }
        else
        {
            ResetWeightIK();
        }
    }

    /// <summary>
    /// Control left and right arms
    /// </summary>
    void ArmsIK()
    {
       if(rightUpperArm == null)
        {
            m_headTransform = animator.GetBoneTransform(HumanBodyBones.Head);
            rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
        }
        //control the arms only when the player is aiming or firing
        float weight = (inPointMode) ? 1 : 0;
        float lweight = (PlayerSync.FPState != PlayerFPState.Running && PlayerSync.FPState != PlayerFPState.Reloading) ? 1 : 0;
        rightArmIKRotationWeight = Mathf.Lerp(rightArmIKRotationWeight, lweight, deltaTime * 6);
        leftArmIkWight = Mathf.Lerp(leftArmIkWight, weight, deltaTime * 6);

        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandRot);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPos);

        if (rightArmIKRotationWeight > 0)
        {
            // Make the right arm aim where the player is looking at
            // Get the look at direction
            Quaternion lookAt = Quaternion.LookRotation(targetPosition - rightUpperArm.position);
            lookAt *= Quaternion.Euler(HandOffset);
            animator.SetIKRotation(AvatarIKGoal.RightHand, lookAt);
        }

        float rpw = (PlayerSync.FPState == PlayerFPState.Aiming || PlayerSync.FPState == PlayerFPState.FireAiming) ? 0.5f : 0;
        rightArmIKPositionWeight = Mathf.Lerp(rightArmIKPositionWeight, rpw, deltaTime * 7);

        Vector3 relativeAimPosition = m_headTransform.TransformPoint(AimSightPosition);
        animator.SetIKPosition(AvatarIKGoal.RightHand, relativeAimPosition);

        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightArmIKRotationWeight);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightArmIKPositionWeight);

        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftArmIkWight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftArmIkWight);
    }

    private void Update()
    {
        if (LeftHandTarget == null) return;

        leftHandPos = LeftHandTarget.position;
        leftHandRot = LeftHandTarget.rotation;
    }

    /// <summary>
    /// 
    /// </summary>
    void ResetWeightIK()
    {
        leftArmIkWight = 0;
        rightArmIKRotationWeight = 0;
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0.0f);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.0f);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
    }

    /// <summary>
    /// 
    /// </summary>
    public Transform HeadLookTarget
    {
        get
        {
            if (m_headTarget == null)
            {
                m_headTarget = Target;
                IsCustomHeadTarget = false;
            }
            return m_headTarget;
        }
        set
        {
            if (value == null)
            {
                m_headTarget = Target;
                IsCustomHeadTarget = false;
            }
            else
            {
                m_headTarget = value;
                IsCustomHeadTarget = true;
            }
        }
    }

    /// <summary>
    /// If the player in an state where the arms should be controlled by IK
    /// </summary>
    private bool inPointMode
    {
        get
        {
            return (PlayerSync.FPState != PlayerFPState.Running && PlayerSync.FPState != PlayerFPState.Reloading);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private Transform LeftHandTarget
    {
        get
        {
            if (playerLegs != null && playerLegs.CurrentGun != null)
            {
                return playerLegs.CurrentGun.LeftHandPosition;
            }
            return null;
        }
    }

    private bl_PlayerReferences m_playerReferences;
    private bl_PlayerReferences playerReferences
    {
        get
        {
            if (m_playerReferences == null) m_playerReferences = CachedTransform.GetComponentInParent<bl_PlayerReferences>();
            return m_playerReferences;
        }
    }

    private bl_PlayerNetwork PlayerSync
    {
        get
        {
            if (playerReferences != null)
                return playerReferences.playerNetwork;

            return null;
        }
    }
}
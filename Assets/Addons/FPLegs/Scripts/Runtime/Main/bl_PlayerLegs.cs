using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class bl_PlayerLegs : bl_MonoBehaviour
{
    #region Public properties
    /// <summary>
    /// 
    /// </summary>
    [SerializeField] private Animator m_animator = null;
    public float blendSmoothness = 5;
    public Animator Animator
    {
        get => m_animator;
        set => m_animator = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public PlayerState BodyState
    {
        get;
        set;
    } = PlayerState.Idle;

    /// <summary>
    /// 
    /// </summary>
    public PlayerFPState FPState
    {
        get;
        set;
    } = PlayerFPState.Idle;

    /// <summary>
    /// Is this player touching the ground?
    /// This value should be provided by bl_PhotonNetwork.cs
    /// </summary>
    public bool IsGrounded
    {
        get;
        set;
    }

    /// <summary>
    /// The velocity of this player
    /// </summary>
    public Vector3 Velocity
    {
        get;
        set;
    } = Vector3.zero;

    /// <summary>
    /// The local velocity of this player
    /// </summary>
    public Vector3 LocalVelocity
    {
        get;
        set;
    } = Vector3.zero;
    public float VelocityMagnitude
    {
        get;
        set;
    }

    public bl_NetworkGun CurrentGun
    {
        get;
        private set;
    }
    #endregion

    #region Private members
    private PlayerState lastBodyState = PlayerState.Idle;
    private float deltaTime = 0.02f;
    private Transform m_Transform;
    private Dictionary<string, int> animatorHashes;
    private float vertical, horizontal;
    private Transform PlayerRoot;
    private float turnSpeed;
    private float TurnLerp = 0;
    private float lastYRotation;
    private float movementSpeed;
    private float timeSinceLastMove = 0;
    private Animator shadowAnimator;
    private Transform shadowWeaponHolder;
    private List<bl_NetworkGun> shadowWeapons;
    #endregion

    /// <summary>
    /// 
    /// </summary>
    protected override void Awake()
    {
        if (!PlayerReferences.photonView.IsMine)
        {
            gameObject.SetActive(false);
            return;
        }

        base.Awake();

        m_Transform = transform;
        PlayerRoot = PlayerReferences.transform;

        if (animatorHashes == null)
        {
            FetchHashes();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();
        bl_EventHandler.onChangeWeapon += OnChangeWeapon;
        bl_EventHandler.onLocalPlayerFire += OnLocalFire;
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void OnDisable()
    {
        base.OnDisable();
        bl_EventHandler.onChangeWeapon += OnChangeWeapon;
        bl_EventHandler.onLocalPlayerFire -= OnLocalFire;
    }

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        if (!PlayerReferences.photonView.IsMine)
        {
            return;
        }

        if (bl_FPLegsSettings.Instance.drawFPShadow)
        {
            SetupShadow();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="active"></param>
    /// <param name="localVerification"></param>
    public void SetActive(bool active, bool localVerification = true)
    {
        if (localVerification && !PlayerReferences.photonView.IsMine)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(active);
    }

    /// <summary>
    /// 
    /// </summary>
    void FetchHashes()
    {
        // cache the hashes in a Array will be more appropriate but to be more readable for other users
        // I decide to cached them in a Dictionary with the key name indicating the parameter that contain
        animatorHashes = new Dictionary<string, int>
        {
            { "BodyState", Animator.StringToHash("BodyState") },
            { "Vertical", Animator.StringToHash("Vertical") },
            { "Horizontal", Animator.StringToHash("Horizontal") },
            { "Speed", Animator.StringToHash("Speed") },
            { "Turn", Animator.StringToHash("Turn") },
            { "isGround", Animator.StringToHash("isGround") },
            { "UpperState", Animator.StringToHash("UpperState") },
            { "Move", Animator.StringToHash("Move") },
            { "GunType", Animator.StringToHash("GunType") }
        };
    }

    /// <summary>
    /// 
    /// </summary>
    public override void OnUpdate()
    {
        if (m_animator == null) return;
        
        deltaTime = Time.deltaTime;
        ControllerInfo();
        Animate();
        UpperControll();
    }

    /// <summary>
    /// 
    /// </summary>
    void ControllerInfo()
    {
        if (m_animator == null) return;
        
        BodyState = PlayerReferences.firstPersonController.State;
        IsGrounded = PlayerReferences.firstPersonController.isGrounded;
        Velocity = PlayerReferences.firstPersonController.Velocity;
        var cfp = PlayerReferences.gunManager.GetCurrentWeapon();
        if (cfp != null)
        {
            FPState = PlayerReferences.gunManager.GetCurrentWeapon().FPState;
        }

        if (PlayerRoot != null)
            LocalVelocity = PlayerRoot.InverseTransformDirection(Velocity);

        float lerp = deltaTime * blendSmoothness;
        vertical = Mathf.Lerp(vertical, LocalVelocity.z, lerp);
        horizontal = Mathf.Lerp(horizontal, LocalVelocity.x, lerp);

        VelocityMagnitude = Velocity.magnitude;
        movementSpeed = Mathf.Lerp(movementSpeed, VelocityMagnitude, lerp);

        if (VelocityMagnitude > 0.1f)
        {
            timeSinceLastMove = Time.time;
        }

        turnSpeed = Mathf.DeltaAngle(lastYRotation, PlayerRoot.localEulerAngles.y);
        TurnLerp = Mathf.Lerp(TurnLerp, turnSpeed, lerp);

        if (Time.time - timeSinceLastMove < 1)
        {
            TurnLerp = 0;
        }

        if (Time.frameCount % 7 == 0) lastYRotation = PlayerRoot.localEulerAngles.y;
    }

    /// <summary>
    /// 
    /// </summary>
    void Animate()
    {
        if (Animator == null)
            return;

        CheckPlayerStates();

        Animator.SetInteger(animatorHashes["BodyState"], (int)BodyState);
        Animator.SetFloat(animatorHashes["Vertical"], vertical);
        Animator.SetFloat(animatorHashes["Horizontal"], horizontal);
        Animator.SetFloat(animatorHashes["Speed"], movementSpeed);
        Animator.SetFloat(animatorHashes["Turn"], TurnLerp);
        Animator.SetBool(animatorHashes["isGround"], IsGrounded);
        
        if (shadowAnimator != null)
        {
            shadowAnimator.SetInteger(animatorHashes["BodyState"], (int)BodyState);
            shadowAnimator.SetFloat(animatorHashes["Vertical"], vertical);
            shadowAnimator.SetFloat(animatorHashes["Horizontal"], horizontal);
            shadowAnimator.SetFloat(animatorHashes["Speed"], movementSpeed);
            shadowAnimator.SetFloat(animatorHashes["Turn"], TurnLerp);
            shadowAnimator.SetBool(animatorHashes["isGround"], IsGrounded);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void CheckPlayerStates()
    {
        if (m_animator == null) return;
        
        if (BodyState != lastBodyState)
        {
            if (lastBodyState == PlayerState.Sliding && BodyState != PlayerState.Sliding)
            {
                Animator.CrossFade(animatorHashes["Move"], 0.2f, 0);
                if (shadowAnimator != null)
                {
                    shadowAnimator.CrossFade(animatorHashes["Move"], 0.2f, 0);
                }
            }
            if (BodyState == PlayerState.Sliding)
            {
                Animator.Play("Slide", 0, 0);
                if (shadowAnimator != null)
                {
                    shadowAnimator.Play("Slide", 0, 0);
                }
            }
            else if (OnEnterPlayerState(PlayerState.Dropping))
            {
                Animator.Play("EmptyUpper", 1, 0);
                if (shadowAnimator != null)
                {
                    shadowAnimator.Play("EmptyUpper", 1, 0);
                }
            }
            else if (OnEnterPlayerState(PlayerState.Gliding))
            {
                Animator.Play("EmptyUpper", 1, 0);
                Animator.CrossFade("gliding-1", 0.33f, 0);
                if (shadowAnimator != null)
                {
                    shadowAnimator.Play("EmptyUpper", 1, 0);
                    shadowAnimator.CrossFade("gliding-1", 0.33f, 0);
                }
            }

            if (OnExitPlayerState(PlayerState.Dropping))
            {
                m_Transform.localRotation = Quaternion.identity;
            }

            lastBodyState = BodyState;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool OnEnterPlayerState(PlayerState playerState)
    {
        if (BodyState == playerState && lastBodyState != playerState)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    public bool OnExitPlayerState(PlayerState playerState)
    {
        if (lastBodyState == playerState && BodyState != playerState)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    void UpperControll()
    {
        int _fpState = (int)FPState;
        if (_fpState == 9) { _fpState = 1; }
        //Animator.SetInteger(animatorHashes["UpperState"], _fpState);
        if (shadowAnimator != null) shadowAnimator.SetInteger(animatorHashes["UpperState"], _fpState);
    }

    /// <summary>
    /// 
    /// </summary>
    private void SetupShadow()
    {
        if (Animator == null) return;

        var shadowPlayer = Instantiate(Animator.gameObject).transform;
        shadowPlayer.name = shadowPlayer.name.Replace("(Clone)", "Shadow");
        shadowPlayer.SetParent(Animator.transform.parent);
        // copy the position and rotation
        shadowPlayer.localPosition = Animator.transform.localPosition;
        shadowPlayer.localRotation = Animator.transform.localRotation;

        shadowAnimator = shadowPlayer.GetComponent<Animator>();
        // get the spine bone from the shadow animator
        var spine = shadowAnimator.GetBoneTransform(HumanBodyBones.Spine);
        // get the child from that bone
        var upperBone = spine.GetChild(0);
        // check if the child name is UpperBone
        if (upperBone.name == "UpperBones")
        {
            // revert the scale from that transform to the original
            upperBone.localScale = Vector3.one;
            // set all the child transform from that transform to the parent transform
            foreach (Transform child in upperBone)
            {
                child.SetParent(upperBone.parent);
            }
            Destroy(upperBone.gameObject);
            // Not sure why but this fixes the GetBoneTransform() returning null
            shadowPlayer.gameObject.SetActive(false);
            shadowPlayer.gameObject.SetActive(true);
        }

        var renderers = shadowPlayer.GetComponentsInChildren<Renderer>(true);
        foreach (var renderer in renderers)
        {
            renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        }
        shadowAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

        var shadowIK = shadowAnimator.gameObject.AddComponent<bl_PlayerShadowIK>();
        shadowIK.playerLegs = this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gunID"></param>
    void OnChangeWeapon(int gunID)
    {
        if (shadowAnimator == null) return;

        if(shadowWeaponHolder == null)
        {
            var tpRoot = shadowAnimator.transform.GetComponentInChildren<bl_NetworkGun>(true);
            if (tpRoot == null)
            {
                // copy the network guns parent transform
                var rightHand = shadowAnimator.GetBoneTransform(HumanBodyBones.RightHand);
                var source = bl_MFPS.LocalPlayerReferences.playerNetwork.NetGunsRoot;
                var copy = Instantiate(source.gameObject);
                shadowWeaponHolder = copy.transform;
                shadowWeaponHolder.transform.parent = rightHand;
                shadowWeaponHolder.transform.localPosition = source.localPosition;
                shadowWeaponHolder.transform.localRotation = source.localRotation;
            }
            else
            {
                shadowWeaponHolder = tpRoot.transform.parent;
            }
            
            // fetch all the network weapons child of the copy transform
            shadowWeapons = shadowWeaponHolder.GetComponentsInChildren<bl_NetworkGun>(true).ToList();
        }

        if (shadowWeapons == null || shadowWeapons.Count == 0) return;

        foreach (var item in shadowWeapons)
        {
            item.AutoSetup = false;
            item.gameObject.SetActive(false);
        }

        // find the network gun by the GunID
        var tpWeapon = shadowWeapons.Find(x => x.GetWeaponID == gunID);
        CurrentGun = tpWeapon;
        
        if (tpWeapon == null) return;

        tpWeapon.gameObject.SetActive(true);
        shadowAnimator.SetInteger("GunType", (int)tpWeapon.Info.Type);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnLocalFire(int gunID)
    {
        if (shadowAnimator == null || CurrentGun == null) return;

        var gunType = CurrentGun.Info.Type;
        switch (gunType)
        {
            case GunType.Knife:
                shadowAnimator.Play("FireKnife", 1, 0);
                break;
            case GunType.Machinegun:
                shadowAnimator.Play("RifleFire", 1, 0);
                break;
            case GunType.Burst:
                shadowAnimator.Play("BurstFire", 1, 0);
                break;
            case GunType.Pistol:
                shadowAnimator.Play("PistolFire", 1, 0);
                break;
            case GunType.Launcher:
                shadowAnimator.Play("LauncherFire", 1, 0);
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [ContextMenu("Setup Legs")]
    public void SetupLegs()
    {
        if(Animator == null)
        {
            Debug.LogWarning("The animator for the player legs has not been assigned and can't be found.");
            return;
        }

        var chest = Animator.GetBoneTransform(HumanBodyBones.Spine);
        if(chest == null)
        {
            Debug.LogWarning("The upper chest bone has not been assigned and can't be found.");
            return;
        }

        // check if the chest object have a child with the name "UpperBones"
        var upperBone = chest.Find("UpperBones");
        if (upperBone == null)
        {
            upperBone = new GameObject("UpperBones").transform;
            upperBone.SetParent(chest);
            upperBone.localPosition = Vector3.zero;
            upperBone.localRotation = Quaternion.identity;
            upperBone.localScale = Vector3.one;

            // put all the childs of the chest object into the UpperBones object
            foreach (Transform child in chest)
            {
                if (child.name != "UpperBones")
                {
                    child.SetParent(upperBone);
                }
            }

            upperBone.localScale = Vector3.zero;
        }

        var modelTransform = Animator.transform;
        // position the model transform 0.3 meters behind the current position
        modelTransform.position = modelTransform.position - (modelTransform.forward * 0.3f);
        modelTransform.transform.localEulerAngles -= new Vector3(4, 0, 0);

        // Optimization 
        // get all the Renderer components of the legs object and set the cast shadows to false
        var renderers = transform.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.shadowCastingMode = ShadowCastingMode.Off;
        }
        Animator.cullingMode = AnimatorCullingMode.CullCompletely;

        // Remove all the CharacterJoint components ins the child of this transform
        var joins = transform.GetComponentsInChildren<CharacterJoint>();
        foreach (var join in joins)
        {
            DestroyImmediate(join);
        }

        // Remove all the Rigidbodies components in the child of this transform
        var components = transform.GetComponentsInChildren<Rigidbody>();
        foreach (var component in components)
        {
            DestroyImmediate(component);
        }

        // Remove all the Collider components in the child of this transform
        var colliders = transform.GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            DestroyImmediate(col);
        }

        // remove any component in the child's of this transform
        var components2 = transform.GetComponentsInChildren<MonoBehaviour>();
        foreach (var component in components2)
        {
            if (component == this) continue;
            if (component is bl_NetworkGun) continue;
            
            DestroyImmediate(component);
        }
    }

    private bl_PlayerReferences _playerReferences = null;
    private bl_PlayerReferences PlayerReferences
    {
        get
        {
            if (_playerReferences == null) _playerReferences = transform.GetComponentInParent<bl_PlayerReferences>();
            return _playerReferences;
        }
    }
}
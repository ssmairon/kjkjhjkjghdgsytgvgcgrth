using System;
using UnityEngine;
using System.Collections;
using MFPS.Internal.Interfaces;
using Photon.Pun;

namespace MFPS.Runtime.Vehicles
{
    public class bl_TankCannon : bl_Tank, IVehicleUpdate
    {
        [Serializable]
        public enum FireTargetMode
        {
            CannonTarget,
            CameraTarget
        }

        #region Public members
        public FireTargetMode fireTargetMode = FireTargetMode.CameraTarget;
        public int ammunition = 100;
        public float fireRate = 0.17f;
        public float maxDamage = 150;
        public float explosionRadius = 8;
        public float recoilForce;
        public float shellVelocity = 500f;
        public Vector3 barrelKickbackPosition;
        public LayerMask hittableLayers;

        [Header("References")]
        public GameObject ShellPrefab;
        public bl_WeaponFXBase fireFx;

        [Header("Audio")]
        public AudioClip shotSound;
        public AudioSource barrelSource;
        #endregion

        #region Private members
        private float lastFire;
        private Vector3 positionRecoil;
        private int bulletLeft;
        private bl_VehicleManager vehicleManager;
        private bl_TankTurret tankTurret;
        private Vector3 barrelOriginPosition;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            TryGetComponent(out vehicleManager);
            TryGetComponent(out tankTurret);
            if (vehicleManager != null) vehicleManager.AddUpdateComponent(this);

            if (tankTurret.cannonPivot != null) barrelOriginPosition = tankTurret.cannonPivot.localPosition;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            bulletLeft = ammunition;
            lastFire = fireRate;
        }

        /// <summary>
        /// 
        /// </summary>
        void IVehicleUpdate.WhenInsideUpdateVehicle()
        {
            HandleInput();
        }

        /// <summary>
        /// 
        /// </summary>
        void IVehicleUpdate.WhenInsideFixedUpdateVehicle()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        void HandleInput()
        {
           
        }

        /// <summary>
        /// 
        /// </summary>
        public void TryToFire()
        {
            if (!vehicleManager.IsLocalPlayerInside()) return;

            DoLocalFire();
        }

        /// <summary>
        /// 
        /// </summary>
        private void DoLocalFire()
        {
            if (Time.time - lastFire < fireRate || bulletLeft <= 0)
                return;

            lastFire = Time.time;
            Vector3 direction = tankTurret.firePoint.forward;
            var rotation = tankTurret.firePoint.rotation;

            if (fireTargetMode == FireTargetMode.CameraTarget)
            {
                var cameraT = bl_VehicleCamera.Instance.CurrentVehicleCamera.transform;
                RaycastHit ray;
                if (Physics.Raycast(cameraT.position, cameraT.forward, out ray, 2000, hittableLayers, QueryTriggerInteraction.Ignore))
                {
                    direction = cameraT.forward;
                    rotation = Quaternion.LookRotation(ray.point - tankTurret.firePoint.position);
                }
            }

            photonView.RPC(nameof(RpcTankFire), RpcTarget.All, direction, rotation, bl_MFPS.LocalPlayer.ViewID);
        }

        /// <summary>
        /// 
        /// </summary>
        [PunRPC]
        public void RpcTankFire(Vector3 direction, Quaternion rottation, int actorViewID, PhotonMessageInfo messageInfo)
        {

            bool isLocal = bl_MFPS.Utility.IsLocalPlayer(actorViewID);

            if (ShellPrefab != null)
            {
                GameObject shell = Instantiate(ShellPrefab, tankTurret.firePoint.position, rottation) as GameObject;

                var actor = bl_GameManager.Instance.GetMFPSActor(actorViewID);

                var projectile = shell.GetComponent<bl_ProjectileBase>();
                projectile.InitProjectile(new BulletData()
                {
                    isNetwork = !isLocal,
                    Damage = maxDamage,
                    WeaponName = "Tank",
                    Position = transform.position,
                    MFPSActor = actor,
                    ActorViewID = actorViewID,
                    ImpactForce = explosionRadius,
                    WeaponID = -2
                });

                var shellBody = shell.GetComponent<Rigidbody>();
                shellBody.AddRelativeForce(Vector3.forward * shellVelocity, ForceMode.Impulse);
            }

            if (fireFx) fireFx.PlayFireFX();

            if (isLocal) bulletLeft--;

            barrelSource.pitch = UnityEngine.Random.Range(0.95f, 1.1f);
            barrelSource.PlayOneShot(shotSound);

            Vector3 cannonForward = tankTurret.upAxis == bl_TankTurret.UpAxis.ZIsUp ? tankTurret.cannonPivot.forward : -tankTurret.cannonPivot.forward;
            Rigidbody.AddForceAtPosition(cannonForward * recoilForce, tankTurret.cannonPivot.position, ForceMode.Impulse);

            StopCoroutine(nameof(DoBarrelRecoil));
            StartCoroutine(nameof(DoBarrelRecoil));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator DoBarrelRecoil()
        {
            if (tankTurret.cannonPivot == null) yield break;

            Vector3 origin = tankTurret.cannonPivot.localPosition;
            float d = 0;
            while (d < 1)
            {
                d += Time.deltaTime / 0.05f;
                tankTurret.cannonPivot.localPosition = Vector3.Slerp(origin, barrelKickbackPosition, d);
                yield return null;
            }

            d = 0;
            while (d < 1)
            {
                d += Time.deltaTime * 3f;
                tankTurret.cannonPivot.localPosition = Vector3.Slerp(barrelKickbackPosition, barrelOriginPosition, d);
                yield return null;
            }
        }
    }
}
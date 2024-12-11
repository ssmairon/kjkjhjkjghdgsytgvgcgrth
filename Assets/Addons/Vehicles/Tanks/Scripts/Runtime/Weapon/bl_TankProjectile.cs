using UnityEngine;
using MFPS.Runtime.Misc;

namespace MFPS.Runtime.Vehicles
{
    public class bl_TankProjectile : bl_ProjectileBase
    {
        public GameObject explosionPrefab;
        public bl_ParticleFader trailFader;

        private BulletData bulletData;
        private bool detecting = true;
        private float instanceTime = 0;

        /// <summary>
        /// 
        /// </summary>
        public override void InitProjectile(BulletData data)
        {
            bulletData = data;
            instanceTime = Time.time;
            Invoke(nameof(Dispose), 5); // Max time to live
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collider"></param>
        void OnCollisionEnter(Collision collider)
        {
            OnHit(collider);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        void OnHit(Collision collision)
        {
            if (!detecting) return;
            if (bulletData != null)
            {
                if (!bulletData.isNetwork)
                {
                    bl_PlayerNetwork pn = collision.gameObject.GetComponent<bl_PlayerNetwork>();
                    if (pn != null && pn.isMine) { return; }//if the player hit itself
                }
                else
                {
                    if (Time.time - instanceTime <= 0.005f) return;
                }
            }

            if (explosionPrefab != null)
            {
                GameObject e = Instantiate(explosionPrefab, collision.contacts[0].point, Quaternion.identity) as GameObject;
                var blast = e.GetComponent<bl_ExplosionBase>();
                if (blast != null)
                {
                    bulletData.Position = transform.position;
                    blast.SetRadius(bulletData.ImpactForce);
                    blast.InitExplosion(bulletData, bl_MFPS.LocalPlayer.MFPSActor);
                }
            }
            detecting = false;
            Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        void Dispose()
        {
            if(trailFader != null)
            {
                trailFader.transform.parent = null;
                trailFader.DoFadeOut();
            }
            CancelInvoke();
            Destroy(gameObject);
        }
    }
}
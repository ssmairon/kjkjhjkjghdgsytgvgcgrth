using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFPS.Runtime.Vehicles
{
    public class bl_TankFX : MonoBehaviour
    {
        public bl_TankController tank;
        public bl_ParticlePlayer[] onMoveParticles;

        private bool wasMoving = false;
        private float normalizeMovementSpeed = 0;

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if (tank == null) return;
            if (Time.frameCount % 6 != 0) return;

            HandleMove();
        }

        /// <summary>
        /// 
        /// </summary>
        void HandleMove()
        {
            if (onMoveParticles == null || onMoveParticles.Length <= 0) return;

            bool isMoving = tank.Velocity.magnitude > 0.05f;
            if (isMoving != wasMoving)
            {
                wasMoving = isMoving;
            }

            if (normalizeMovementSpeed <= 0 && !isMoving)
            {
                SetActiveMoveParticles(false);
            }
            else if (isMoving && !onMoveParticles[0].IsPlaying)
            {
                SetActiveMoveParticles(true);
            }

            normalizeMovementSpeed = Mathf.Lerp(normalizeMovementSpeed, tank.CurrentMovingSpeed / tank.topSpeed, 4);
            for (int i = 0; i < onMoveParticles.Length; i++)
            {
                onMoveParticles[i].SetRate(normalizeMovementSpeed);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="active"></param>
        public void SetActiveMoveParticles(bool active)
        {
            for (int i = 0; i < onMoveParticles.Length; i++)
            {
                if (active) onMoveParticles[i].Play();
                else onMoveParticles[i].Stop();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MFPS.Runtime.Vehicles
{
    [Serializable]
    public class bl_CaterpillaMotion
    {
        [Serializable]
        public enum MaterialScaleAxi
        {
            X,
            Y
        }

        public Renderer trackMesh;    
        public MaterialScaleAxi materialScaleAxi = MaterialScaleAxi.Y;
        public float moveGain = -0.015f;
        public float rotationFactor = 0.18f;
        public float spinDirection = 1;

        private Vector2 materialScale = Vector2.zero;
        private float offset;
        private Material trackMaterial;

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            if (!Application.isPlaying) return;

            if(trackMesh == null)
            {
                Debug.LogWarning("Track mesh has not been assigned yet!");
                return;
            }

            // Make a new instance of the material so the movement of a tank
            // doesn't affect the others
            trackMaterial = trackMesh.material;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        public void UpdateMotion(float speed, float angular, bool right)
        {
            if (trackMaterial == null) return;

            float velocity = moveGain * speed * Time.fixedDeltaTime;
            velocity = velocity * spinDirection;
            offset += velocity;

            velocity = rotationFactor * angular * Time.fixedDeltaTime;
            velocity = velocity * spinDirection;

            if (right) offset -= velocity;
            else offset += velocity;

            if (materialScaleAxi == MaterialScaleAxi.X) materialScale.Set(offset, 0f);
            else materialScale.Set(0f, offset);

            trackMaterial.mainTextureOffset = materialScale;
        }
    }
}
using System;
using UnityEngine;

namespace MFPS.Runtime.Vehicles
{
    [Serializable]
    public class bl_TrackWheel 
    {
        public WheelCollider WheelCollider;
        public Transform WheelModel;
        public Transform TrackBone;

        public float AngularVelocityMagnitude { get; private set; } = 0;

        private Vector3 wheelPosition = Vector3.zero;
        private Quaternion wheelRotation = Quaternion.identity;
        private Vector3 currentPosition, bonePosition, initBonePos;
        private bl_TankTrack m_track;

        /// <summary>
        /// 
        /// </summary>
        public void Init(bl_TankController controller, bl_TankTrack track)
        {
            m_track = track;
            if (TrackBone != null) initBonePos = TrackBone.localPosition;
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateTorque(float motorTorque, float breakTorque)
        {
            if (WheelCollider == null) return;

            WheelCollider.motorTorque = motorTorque;
            WheelCollider.brakeTorque = breakTorque;
        }

        /// <summary>
        /// 
        /// </summary>
        public void WheelUpdate(float speed, float angularSpeed)
        {
            if (WheelCollider != null)
            {
                WheelCollider.GetWorldPose(out wheelPosition, out wheelRotation);
                currentPosition = Vector3.Slerp(currentPosition, wheelPosition, Time.fixedDeltaTime * 50f);
                WheelModel.position = currentPosition;
            }

            if (m_track.isRightTrack)
            {
                angularSpeed *= -1f;
            }

            if (WheelModel != null)
            {
                AngularVelocityMagnitude = (speed + angularSpeed) * m_track.spinRatio * Time.fixedDeltaTime;
                WheelModel.RotateAround(WheelModel.position, WheelModel.right, AngularVelocityMagnitude);
            }

            if (TrackBone != null && WheelCollider != null)
            {
                bonePosition = TrackBone.position;
                bonePosition.y = (currentPosition.y - WheelCollider.radius) - m_track.trackVerticalOffset;
                TrackBone.position = bonePosition;

                bonePosition = TrackBone.localPosition;
                bonePosition.x = initBonePos.x;
                bonePosition.z = initBonePos.z;
                TrackBone.localPosition = bonePosition;
            }
        }
    }
}
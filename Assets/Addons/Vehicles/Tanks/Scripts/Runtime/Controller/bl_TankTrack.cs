using UnityEngine;

namespace MFPS.Runtime.Vehicles
{
    public class bl_TankTrack : MonoBehaviour
    {
        public bl_TrackWheel[] Wheels;
        public WheelCollider wheelReference;
        public bool isRightTrack = false;
        public float angularSpinRatio = 20f;
        public float spinRatio = 20f;
        public float trackVerticalOffset = 0.02f;
        public bl_CaterpillaMotion caterpillaMotion;
        public bool showBonePositionGizmo = true;

        /// <summary>
        /// 
        /// </summary>
        public void Init(bl_TankController controller)
        {
            caterpillaMotion.Init();
            for (int i = 0; i < Wheels.Length; i++)
            {
                Wheels[i].Init(controller, this);

                if(wheelReference == null && Wheels[i].WheelCollider != null)
                {
                    wheelReference = Wheels[i].WheelCollider;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateTorques(float motorTorque, float breakTorque)
        {
            for (int i = 0; i < Wheels.Length; i++)
            {
                Wheels[i].UpdateTorque(motorTorque, breakTorque);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void WheelUpdate(float localAngularY, float speed)
        {
            float angularSpeed = localAngularY * angularSpinRatio;
            for (int i = 0; i < Wheels.Length; i++)
            {
                Wheels[i].WheelUpdate(speed, angularSpeed);
            }

            caterpillaMotion.UpdateMotion(speed, localAngularY, isRightTrack);
        }

        private void OnDrawGizmos()
        {
            if (!showBonePositionGizmo || Application.isPlaying) return;

            for (int i = 0; i < Wheels.Length; i++)
            {
                var w = Wheels[i];
                if (w.TrackBone == null) continue;

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(Wheels[i].TrackBone.position, 0.1f);

                if(w.WheelCollider != null)
                {
                    Gizmos.DrawLine(w.WheelCollider.transform.position, Wheels[i].TrackBone.position);
                }

                if (w.WheelModel != null)
                {
                    Gizmos.DrawLine(w.WheelModel.transform.position, Wheels[i].TrackBone.position);
                }

                if (w.WheelModel != null && w.WheelCollider != null)
                {
                    Gizmos.DrawLine(w.WheelModel.transform.position, w.WheelCollider.transform.position);
                }

#if UNITY_EDITOR
                UnityEditor.Handles.Label(Wheels[i].TrackBone.position, $"Bone {i}", UnityEditor.EditorStyles.miniLabel);
#endif
                Gizmos.color = Color.white;
            }
        }
    }
}
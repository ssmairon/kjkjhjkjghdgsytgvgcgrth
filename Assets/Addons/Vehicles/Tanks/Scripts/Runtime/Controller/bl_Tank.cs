using UnityEngine;

namespace MFPS.Runtime.Vehicles
{
    public class bl_Tank : bl_Vehicle
    {

        private float m_currentTorque = 0;
        public float CurrentTorque
        {
            get => m_currentTorque;
            set => m_currentTorque = value;
        }

        private float m_currentAcceleration = 0;
        public float CurrentAcceleration
        {
            get => m_currentAcceleration;
            set => m_currentAcceleration = value;
        }

        public Vector3 Velocity => Rigidbody.velocity;
        public Vector3 LocalVelocity => Transform.InverseTransformDirection(Velocity);
        public virtual Vector3 AngularVelocity
        {
            get
            {
                return Rigidbody.angularVelocity;
            }
        }
        public Vector3 LocalAngularVelocity => Transform.InverseTransformDirection(AngularVelocity);

        private Rigidbody _rigidbody;
        public Rigidbody Rigidbody
        {
            get
            {
                if (_rigidbody == null) TryGetComponent(out _rigidbody);
                return _rigidbody;
            }
        }

        private Transform _transform;
        public Transform Transform
        {
            get
            {
                if (_transform == null) _transform = transform;
                return _transform;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="acceleration"></param>
        /// <param name="steering"></param>
        /// <param name="velocity"></param>
        public override void GetVehicleInput(ref float acceleration, ref float steering, ref Vector3 velocity)
        {
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="acceleration"></param>
        /// <param name="steering"></param>
        /// <param name="isRemote"></param>
        public override void SetVehicleInput(float acceleration, float steering, bool isRemote = true)
        {
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="acceleration"></param>
        /// <param name="steering"></param>
        /// <param name="velocity"></param>

        public override void SetVehicleInput(float acceleration, float steering, Vector3 velocity)
        {
           
        }
    }
}
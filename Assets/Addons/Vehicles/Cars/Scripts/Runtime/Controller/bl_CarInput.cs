using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFPS.Runtime.Vehicles
{
    public class bl_CarInput : bl_MonoBehaviour
    {
        private bl_CarController car;
        private float horizontal, vertical, handBrake = 0;

        /// <summary>
        /// 
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            car = GetComponent<bl_CarController>();
        }

#if MFPSM
        protected override void OnEnable()
        {
            base.OnEnable();
            bl_TouchHelper.OnVehicleDirection += OnMobileDirection;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            bl_TouchHelper.OnVehicleDirection -= OnMobileDirection;
        }
#endif
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dir"></param>
        void OnMobileDirection(Vector2 dir)
        {
            vertical = dir.y;
            horizontal = dir.x;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnFixedUpdate()
        {
            // TODO: Input in Update() maybe?
            if (!bl_UtilityHelper.isMobile)
            {
                vertical = bl_GameInput.Vertical;
                horizontal = bl_GameInput.Horizontal;
                handBrake = bl_GameInput.Jump(GameInputType.Hold) ? 1 : 0;
            }
            car.Move(horizontal, vertical, vertical, handBrake, false);
        }
    }
}
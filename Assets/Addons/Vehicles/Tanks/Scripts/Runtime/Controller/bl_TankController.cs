using UnityEngine;

namespace MFPS.Runtime.Vehicles
{
	public class bl_TankController : bl_Tank
	{
		#region Public members
		public bl_TankTrack leftTrack;
		public bl_TankTrack rightTrack;
		public Vector3 centerOfMass = new Vector3(0, -1, 0);
		public float torque = 1500;
		public float brakeTorque = 1000f;
		public float accelerationStep = 0.05f;
		public float angularTorque = 50;
		public float maxAcceleration = 1;
		public float topSpeed = 25;
		public float rotationSpeedQualifier = 1.2f;
		public float transmissionSwitch = 0.5f;
		public float gravity = 9.18f;
		public float xAxisAccelerationStep = 4f;
		public float yAxisAccelerationStep = 1.5f;
		public float yAxisChangingDirectionInertion = 5f;
		public float xAxisInertion = 1.2f;
		public float yAxisInertion = 0.45f;
		[SerializeField]
		private float m_RevRangeBoundary = 1f;
		[LovattoToogle] public bool realWheels = false;
		#endregion

		#region Private members
		private float m_horizontalInput;
		private float m_verticalInput;
		private AudioSource m_audioSource;
		private float lastBreak = 0;
		private float accumulateAcceleration = 0;
		private float verticalKoefficient = 1;
		private float curMaxRotationSpeed = 0;
		private Vector3 requiredLocalVelocity, requiredLocalAngularVelocity;
		private float angle;
		const float KOEFFICIENT_OFFSET = 0.75f;
		const float BRAKE_KOEFFICIENT = 50f;
		private int m_GearNum;
		private float m_GearFactor;
		[SerializeField]
		private static int NoOfGears = 5;
        #endregion

        #region Public properties
        public float Revs { get; private set; }
        public float CurrentMovingSpeed { get; private set; } = 0;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        protected override void Awake()
		{
			base.Awake();
			m_audioSource = GetComponent<AudioSource>();
			Init();
			leftTrack?.Init(this);
			rightTrack?.Init(this);
		}

		/// <summary>
		/// 
		/// </summary>
		void Init()
        {
			Rigidbody.centerOfMass = centerOfMass;
			if (realWheels)
			{
				Rigidbody.drag = 0f;
				return;
			}
			Rigidbody.drag = 2f;
			angularTorque *= 3f;
			Move(0, 0);
		}	

		/// <summary>
		/// 
		/// </summary>
		public void Move(float vertical, float horizontal)
		{
			m_verticalInput = vertical;
			m_horizontalInput = horizontal;
			CurrentAcceleration = Acceleration(m_verticalInput) * m_verticalInput;

			curMaxRotationSpeed = angularTorque * rotationSpeedQualifier * m_horizontalInput;
			OnGround();

			if (gravity > 0f)
			{
				Rigidbody.AddForce(Vector3.down * gravity * Rigidbody.drag, ForceMode.Acceleration);
			}
		}

		/// <summary>
		/// Handle the accumulative acceleration based on the use input
		/// </summary>
		/// <returns></returns>
		private float Acceleration(float yAxisAcce)
		{
			if (Mathf.Abs(yAxisAcce) > 0.05f)
			{
				if (accumulateAcceleration < maxAcceleration)
				{
					accumulateAcceleration += Time.fixedDeltaTime * accelerationStep / Mathf.Clamp(verticalKoefficient, 0.1f, 1f);
				}
				accumulateAcceleration = Mathf.Min(accumulateAcceleration, maxAcceleration);
			}
			else
			{
				accumulateAcceleration -= Time.fixedDeltaTime;
				accumulateAcceleration = Mathf.Max(accumulateAcceleration, 0);
			}
			return accumulateAcceleration;
		}

		/// <summary>
		/// 
		/// </summary>
		private void OnGround()
		{
			if (Mathf.Abs(curMaxRotationSpeed) > 0.1f && Mathf.Abs(CurrentAcceleration) < 0.05f)
			{
				CurrentAcceleration = 0.1f;
			}
			requiredLocalVelocity = base.LocalVelocity;
			requiredLocalVelocity.x = 0f;
			requiredLocalVelocity.z = (Mathf.Abs(CurrentAcceleration) > 0.05f) ? CurrentAcceleration : 0f;
			Vector3 moveDirection = Transform.TransformDirection(requiredLocalVelocity);
			angle = Vector3.Angle(moveDirection, Vector3.up) * (Mathf.PI / 180f);
			verticalKoefficient = Mathf.Clamp01(KOEFFICIENT_OFFSET - Mathf.Cos(angle));

			if (realWheels)
			{
				MoveWheels();
			}
			else
			{
				moveDirection.y = 0f;
				moveDirection *= verticalKoefficient;
				moveDirection.y = Rigidbody.velocity.y;
				Rigidbody.velocity = moveDirection;
			}

			ApplyAngularVelocity();
			CalculateRevs();
		}

		/// <summary>
		/// 
		/// </summary>
		void MoveWheels()
		{
			CurrentMovingSpeed = LocalVelocity.z;
			float brakeTorque = 0f;
			CurrentTorque = 0;
			bool roomForAcce = Mathf.Abs(CurrentMovingSpeed) < topSpeed;
			bool isMoving = Mathf.Abs(CurrentAcceleration) > 0.1f;

			if (Mathf.Sign(CurrentAcceleration) == Mathf.Sign(CurrentMovingSpeed) && isMoving)
			{
				lastBreak = Time.time;
			}

			if (roomForAcce && isMoving)
			{
				CurrentTorque = CurrentAcceleration * torque;
			}
			else if (leftTrack.wheelReference != null && Mathf.Abs(leftTrack.wheelReference.rpm) > 150f || (Time.time - lastBreak) >= transmissionSwitch)
			{
				brakeTorque = torque * BRAKE_KOEFFICIENT;
			}
			leftTrack?.UpdateTorques(CurrentTorque, brakeTorque);
			rightTrack?.UpdateTorques(CurrentTorque, brakeTorque);
		}

		/// <summary>
		/// 
		/// </summary>
		private void ApplyAngularVelocity()
		{
			bool movingInReverse = m_verticalInput <= -1f;
			if (Mathf.Abs(curMaxRotationSpeed) > 0.1f)
			{
				requiredLocalAngularVelocity = base.LocalAngularVelocity;
				requiredLocalAngularVelocity.y = (!movingInReverse ? 1 : -1) * curMaxRotationSpeed * 0.03f;
				requiredLocalAngularVelocity = Transform.TransformDirection(requiredLocalAngularVelocity);
				Rigidbody.angularVelocity = requiredLocalAngularVelocity * verticalKoefficient;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void UpdateWheelPoses()
		{
			float vel = LocalVelocity.magnitude;
			leftTrack?.WheelUpdate(LocalAngularVelocity.y, vel * LocalVelocity.z);
			rightTrack?.WheelUpdate(LocalAngularVelocity.y, vel * LocalVelocity.z);
		}

		/// <summary>
		/// 
		/// </summary>
		public void UpdateWheelPoses(Vector3 localVelocity, float angular)
		{
			leftTrack?.WheelUpdate(angular, localVelocity.magnitude * localVelocity.z);
			rightTrack?.WheelUpdate(angular, localVelocity.magnitude * localVelocity.z);
		}

		/// <summary>
		/// 
		/// </summary>
		private void CalculateGearFactor()
		{
			float f = (1 / (float)NoOfGears);
			// gear factor is a normalized representation of the current speed within the current gear's range of speeds.
			// We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
			var targetGearFactor = Mathf.InverseLerp(f * m_GearNum, f * (m_GearNum + 1), Mathf.Abs(CurrentMovingSpeed / topSpeed));
			m_GearFactor = Mathf.Lerp(m_GearFactor, targetGearFactor, Time.deltaTime * 5f);
		}

		/// <summary>
		/// 
		/// </summary>
		private void CalculateRevs()
		{
			// calculate engine revs (for display / sound)
			// (this is done in retrospect - revs are not used in force/power calculations)
			CalculateGearFactor();
			float gearNumFactor = m_GearNum / (float)NoOfGears;
			float revsRangeMin = ULerp(0f, m_RevRangeBoundary, CurveFactor(gearNumFactor));
			float revsRangeMax = ULerp(m_RevRangeBoundary, 1f, gearNumFactor);
			Revs = ULerp(revsRangeMin, revsRangeMax, m_GearFactor);
		}

		// simple function to add a curved bias towards 1 for a value in the 0-1 range
		private static float CurveFactor(float factor)
		{
			return 1 - (1 - factor) * (1 - factor);
		}


		// unclamped version of Lerp, to allow value to exceed the from-to range
		private static float ULerp(float from, float to, float value)
		{
			return (1.0f - value) * from + value * to;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public float Accelerate(float oldSpeed, float newSpeed, float step, float inertionRatio, bool xAxis)
		{
			bool des = newSpeed < oldSpeed && newSpeed >= 0f && oldSpeed > 0f;
			bool acc = newSpeed > oldSpeed && newSpeed <= 0f && oldSpeed < 0f;

			if (des || acc)
			{
				step *= inertionRatio;
			}
			if ((newSpeed > 0f && oldSpeed < 0f) || (newSpeed < 0f && oldSpeed > 0f))
			{
				step *= yAxisChangingDirectionInertion;
			}
			step *= Time.deltaTime;
			return Mathf.MoveTowards(oldSpeed, newSpeed, step);
		}

		/// <summary>
		/// 
		/// </summary>
        public override void OnEnterVehicle()
        {
			bl_TankCrosshair.Instance?.SetActive(true);
			bl_TankStateUI.Instance?.SetupTank(bl_VehicleCamera.Instance.cameraTransform, transform);
			bl_TankStateUI.Instance?.SetActive(true);
			GetComponent<bl_VehicleManager>().AddUpdateComponent(bl_TankStateUI.Instance);
		}

		/// <summary>
		/// 
		/// </summary>
        public override void OnExitVehicle()
        {
			bl_TankCrosshair.Instance?.SetActive(false);
			bl_TankStateUI.Instance?.SetActive(false);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="acceleration"></param>
		/// <param name="steering"></param>
		/// <param name="velocity"></param>
        public override void GetVehicleInput(ref float acceleration, ref float steering, ref Vector3 velocity)
        {
			velocity = Velocity;
			acceleration = m_verticalInput;
			steering = m_horizontalInput;
		}

        /// <summary>
        /// 
        /// </summary>
        private void OnDrawGizmos()
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawSphere(transform.TransformPoint(centerOfMass), 0.2f);
			Gizmos.DrawWireSphere(transform.TransformPoint(centerOfMass), 0.2f);
			Gizmos.color = Color.white;
		}
	}
}
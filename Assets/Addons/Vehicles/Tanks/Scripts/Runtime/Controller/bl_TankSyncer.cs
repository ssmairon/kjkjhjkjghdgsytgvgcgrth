using Photon.Pun;
using UnityEngine;

namespace MFPS.Runtime.Vehicles
{
    /// <summary>
    /// This is scripts handle the synchronization of the tank child transforms like the turret and the cannon
    /// along with some properties specifically for the tank controller that the <see cref="bl_VehicleTransformView"/> script doesn't handle.
    /// </summary>
    public class bl_TankSyncer : bl_MonoBehaviour, IPunObservable
    {
        public bl_TankController tankController;
        public bl_TankTurret tankTurret;
        public bl_VehicleManager vehicleManager;

        private Vector3 netLocalVelocity = Vector3.zero;
        private Vector3 netCannonAngle, netBaseTurretAngle = Vector3.zero;
        private float netAngular = 0;

        /// <summary>
        /// 
        /// </summary>
        public override void OnFixedUpdate()
        {
            if (vehicleManager.isMine)
            {
                tankController.UpdateWheelPoses();
            }
            else
            {
                tankController.UpdateWheelPoses(netLocalVelocity, netAngular);
                tankTurret.SetBaseAngle(netBaseTurretAngle);
                tankTurret.SetCannonAngle(netCannonAngle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(tankController.LocalVelocity);
                stream.SendNext(tankController.LocalAngularVelocity.y);
                stream.SendNext(tankTurret.GetBaseTurretAngle());
                stream.SendNext(tankTurret.GetCannonAngle());
            }
            else
            {
                netLocalVelocity = (Vector3)stream.ReceiveNext();
                netAngular = (float)stream.ReceiveNext();
                netBaseTurretAngle = (Vector3)stream.ReceiveNext();
                netCannonAngle = (Vector3)stream.ReceiveNext();
            }
        }
    }
}
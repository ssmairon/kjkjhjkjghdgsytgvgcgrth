using System;
using System.Collections.Generic;
using UnityEngine;

namespace MFPS.Mobile
{
    [Serializable, Flags]
    public enum MobileButtonsLayers
    {
        All = 0,
        MovementButtons = 1,
        MovementJoystick = 2,
        WeaponButtons = 4,
        WeaponSelector = 8,
        Chat = 16,
        Voice = 32,
        Kits = 64,
    }

    [Serializable]
    public enum FPSMobileButton
    {
        MovementJoystick,
        Fire,
        FireHold,
        FireReleased,
        Reload,
        Aim,
        Jump,
        Crouch,
        Kit,
        Pause,
        WeaponLeft,
        WeaponRight,
        Chat,
        Voice
    }
}
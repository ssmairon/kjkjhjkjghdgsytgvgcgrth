using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "FPLegsSettings", menuName = "MFPS/Addons/Legs/Settings")]
public class bl_FPLegsSettings : ScriptableObject
{
    [LovattoToogle] public bool drawFPShadow = true;
    
    private static bl_FPLegsSettings m_Data;
    public static bl_FPLegsSettings Instance
    {
        get
        {
            if (m_Data == null)
            {
                m_Data = Resources.Load("FPLegsSettings", typeof(bl_FPLegsSettings)) as bl_FPLegsSettings;
            }
            return m_Data;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bl_PlayerLegsActivator : MonoBehaviour
{
    //private bool wasLegsEnabled = false;
    
    /// <summary>
    /// 
    /// </summary>
    private void OnEnable()
    {
        // When enable this object, disable the legs
        SetLegsActive(false);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDisable()
    {
        SetLegsActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="active"></param>
    public void SetLegsActive(bool active)
    {
        var legs = GetInstance();
        if (legs == null) return;

        legs.SetActive(active);
    }
    
    private bl_PlayerLegs _playerLegs = null;
    public bl_PlayerLegs GetInstance()
    {
        if (_playerLegs == null)
        {
            _playerLegs = transform.root.GetComponentInChildren<bl_PlayerLegs>(true);
        }
        return _playerLegs;
    }
}
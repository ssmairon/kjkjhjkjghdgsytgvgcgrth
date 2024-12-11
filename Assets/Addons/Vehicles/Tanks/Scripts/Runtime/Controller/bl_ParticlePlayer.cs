using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bl_ParticlePlayer : MonoBehaviour
{

    public float emissionRate = 10;

    /// <summary>
    /// 
    /// </summary>
    public void Play()
    {
        Particle.Play(true);
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetRate(float normalize)
    {
        var emission = Particle.emission;
        emission.rateOverTimeMultiplier = emissionRate * normalize;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Stop()
    {
        Particle.Stop(true,ParticleSystemStopBehavior.StopEmitting);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Clear()
    {
        Particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsPlaying => Particle.isPlaying;


    private ParticleSystem _particle;
    public ParticleSystem Particle
    {
        get
        {
            if (_particle == null) _particle = GetComponentInChildren<ParticleSystem>();
            return _particle;
        }
    }
}
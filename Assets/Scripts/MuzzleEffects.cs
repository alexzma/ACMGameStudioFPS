using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleEffects : MonoBehaviour
{
    public ParticleSystem mf;
    public void CreateMuzzleEffect()
    {
        mf.Play();
    }
}

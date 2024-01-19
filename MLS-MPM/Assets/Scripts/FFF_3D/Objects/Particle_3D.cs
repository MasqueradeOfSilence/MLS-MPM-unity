using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Particle_3D : ScriptableObject
{
    private double3 position;
    private double3 velocity;
    private double mass;
    private double3x3 affineMomentumMatrix;
    // Bubble class is already fine for 3D, so no need to make a 3D version.
    private Bubble bubble = null;
    private bool initialized = false;

    public void Init(double3 position, double3 velocity, double mass, double3x3 affineMomentumMatrix)
    {
        this.position = position;
        this.velocity = velocity;
        this.mass = mass;
        this.affineMomentumMatrix = affineMomentumMatrix;
        initialized = true;
    }

    public bool IsInitialized()
    {
        return initialized;
    }
}

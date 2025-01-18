using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class Particle_Debug
{
    private double3 position;
    private double3 velocity;
    private double mass;
    private double3x3 C_affineMomentumMatrix;

    public Particle_Debug(double3 position, double3 velocity, double mass, double3x3 c) 
    { 
        this.position = position;
        this.velocity = velocity;
        this.mass = mass;
        this.C_affineMomentumMatrix = c;
    }
}

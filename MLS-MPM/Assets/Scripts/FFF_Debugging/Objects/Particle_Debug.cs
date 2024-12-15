using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class Particle_Debug
{
    private double3 position;
    private double3 velocity;
    private double mass;
    private double3x3 affineMomentumMatrix_C;

    public Particle_Debug(double3 position, double3 velocity, double mass, double3x3 c) 
    { 
        this.position = position;
        this.velocity = velocity;
        this.mass = mass;
        this.affineMomentumMatrix_C = c;
    }
}

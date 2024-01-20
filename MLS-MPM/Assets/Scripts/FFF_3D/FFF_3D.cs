using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * FFF_3D: Simulates a 3D foam with a small pool of water at the bottom.
 *  Uses the Foam Fraction Flow method. 
 */

public class FFF_3D : MonoBehaviour
{
    /**
     * Data members
     */
    private Particle_3D[][][] particles;
    private Grid_3D[] grid;
    private int gridResolution = 64;
    private const double timestep = 0.2;
    private const int numberOfSimulationsPerUpdateCall = (int) (1 / timestep);
    private const double gravity = -9.8;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

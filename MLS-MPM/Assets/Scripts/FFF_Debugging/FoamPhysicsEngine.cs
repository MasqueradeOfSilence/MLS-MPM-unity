using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * In this class, I will slowly build up the non-Voronoi pieces of FFF 
 *  until I ascertain the source of the right-trending divergence.
 *  
 *  This will entail testing it for quite a few frames, which means that multithreading this time is imperative. 
 *  
 *  Reference along with nialltl: https://github.com/MasqueradeOfSilence/MLS-MPM-unity/blob/main/MLS-MPM/Assets/Scripts/FFF_3D/FFF_Optimized_3D.cs
 *  
 *  Parallelized reference for speedup: https://github.com/nialltl/incremental_mpm/blob/master/Assets/3.%20MLS_MPM_Fluid_Multithreaded/MLS_MPM_Fluid_Multithreaded.cs
 *  
 *  First objective: It was working fine in 2D. Is it the 3D that causes the divergence?
 */

public class FoamPhysicsEngine : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting foam physics engine!");
        InitializeFluid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeFluid()
    {
        InitializeGrid();
        InitializeParticles();
    }

    public void InitializeGrid()
    {

    }

    public void InitializeParticles()
    {

    }
}

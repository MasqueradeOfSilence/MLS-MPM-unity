using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * In this class, I will slowly build up the non-Voronoi pieces of FFF 
 *  until I ascertain the source of the right-trending divergence.
 *  
 *  This will entail testing it for quite a few frames, which means that multithreading this time is imperative. 
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

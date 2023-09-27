// water particles converted to unity by http://unitycoder.com/blog
// original source: http://www.dhteumeuleu.com/aqualibrium

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace ptest
{

	public class JaggedObjectArray : MonoBehaviour 
	{

    public ThreadStart thread_start = null;
    public Thread thread;
		
	public Material mat;
		
	public static bool mouseDown = false;
	public static float pointerX = 0.0f;
	public static float pointerY = 0.0f;
	
	public Transform prefab;
	private int pcount;

	public static List<TestParticle> grid = new List<TestParticle>(10*10);
	
	// particle list
	private List<TestParticle> particles = new List<TestParticle>();

	// Use this for initialization
	void Start () {
		
		for (var n=0;n<700;n++)
		{
			float xpos = Random.value*250;
			float ypos = Random.value*250;
			Transform clone = Instantiate(prefab,new Vector3(xpos,0.0f,ypos),Quaternion.identity) as Transform;
			particles.Add(new TestParticle( clone, xpos,ypos));
		}
		
		pcount = particles.Count;
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		// mouseworldpos
		if (Input.GetMouseButton(0))
		{
			Vector3 p = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,260.0f));
			pointerX = p.x;
			pointerY = p.z;
		}
		
		if (Input.GetMouseButtonDown(0))
		{
			mouseDown = true;
		}
		
		if (Input.GetMouseButtonUp(0))
		{
			mouseDown = false;
		}
		
		


		for(int i = 0; i < pcount; i++) particles[i].UpdateNeighbours(particles,32); // distance 32
		for(int i = 0; i < pcount; i++) particles[i].physics();
		for(int j = 0; j < pcount; j++) particles[j].move();



		
	}
}
}
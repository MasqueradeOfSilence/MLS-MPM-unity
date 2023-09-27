// water particles converted to unity by http://unitycoder.com/blog
// original source: http://www.dhteumeuleu.com/aqualibrium

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ptest
{

public class TestParticle 
{
	private int pdiam = 8;
	private int pdiam2 = 64;
	private int pdiamHalf = 4;
	private int screenMaxXm = 500-4;
	private int screenMaxYm = 500-4;
	private int screenMaxXp = 500+4;
	private int screenMaxYp = 500+4;
	
	private int gw = 15;
	private int gh = 5;
	
	private float X;
	private float Y;
	private float DX;
	private float DY;
	private float VX;
	private float VY;
	private float AX;
	private float AY;
	
	private Transform obj;
	
	private List<TestParticle> neighbours = new List<TestParticle>();


	public TestParticle(Transform prefab, float x, float y)
	{
		this.X = x;
		this.Y = y;
		this.DX = 0.0f;
		this.DY = 0.0f;
		this.VX = 0.0f;
		this.VY = 0.0f;
		this.AX = 0.0f;
		//this.AY = -1.0f+UnityEngine.Random.value;//-0.05f:0.05f;
		this.AY = -0.05f;
	//	this.obj = insta(prefab);
		this.obj = prefab;
	}


	public float x
	{
		get {return X;}
		set {X = value;}
	}

	public float y
	{
		get {return Y;}
		set {Y = value;}
	}
	
	public float ax
	{
		get {return AX;}
		set {AX = value;}
	}

	public float ay
	{
		get {return AY;}
		set {AY = value;}
	}

	public float vx
	{
		get {return VX;}
		set {VX = value;}
	}

	public float vy
	{
		get {return VY;}
		set {VY = value;}
	}

	public float dx
	{
		get {return DX;}
		set {DX = value;}
	}

	public float dy
	{
		get {return DY;}
		set {DY = value;}
	}
	
	
	// ==== move particle ==== 
	public void move()
	{
		this.X  += this.dx;
		this.Y  += this.dy;
		this.VX += this.dx;
		this.VY += this.dy;
		this.DX  = 0.0f;
		this.DY  = 0.0f;
		// ---- draw particle ---- 

		this.obj.position = new Vector3(x,0,y);

		
	}	
	
	public void UpdateNeighbours(List<TestParticle> allParticles, float maxdist)
	{
		//this.physics();
		
		
		// clear neighbours
		this.neighbours.Clear();
		
		int ncount = 0;
		// get neighbours within this range
		for (int n=0;n<allParticles.Count;n++)
		{
			if (allParticles[n] != this)
			{

				
				// 1500pcs, 104ms
				float xd = this.x-allParticles[n].x;
				float yd = this.y-allParticles[n].y;
				if ((xd*xd + yd*yd) < (maxdist*maxdist))
				
				//Debug.Log("n:"+n+" dist:"+dist);
//				if (dist<maxdist)
				{
				//	Debug.DrawLine (this.obj.position, allParticles[n].obj.position, Color.blue);
					//Debug.DrawLine (this.obj.position, allParticles[n].obj.position, Color.blue);
					neighbours.Add(allParticles[n]);
					ncount++;
				}
			}
		}

	}	
	
	private int getNeighbourCount()
	{
		return this.neighbours.Count;
	}	
	
	private TestParticle getNeighbour(int index)
	{
		//Debug.Log(index);
		return this.neighbours[index];
	}
		
	public Vector3 getPos
	{
		//Debug.Log(index);
		get {return new Vector3(this.X,this.Y,500.0f);}
		//get {return new Vector3(this.X/500.0f,this.Y/500.0f, 0.0f);}
	}
	
	public Vector3 getObjPos
	{
		//Debug.Log(index);
		get {return this.obj.position;}
		//get {return new Vector3(this.X/500.0f,this.Y/500.0f, 0.0f);}
	}
	
	
		// ==== fluid simulation ==== 
	public void physics()
	{
		// TODO: not needed for every particle!!, take one value when clicked, use that for all
		if (JaggedObjectArray.mouseDown) 
		{
			float dx = this.x - JaggedObjectArray.pointerX;
			float dy = this.y - JaggedObjectArray.pointerY;
			float d = Mathf.Sqrt(dx * dx + dy * dy)*3.0f; // *3, slows down force

			if (d < 250) 
			{
				this.dx += dx / d * 0.5f;
				this.dy += dy / d * 0.5f;
//				this.dx += dx * 0.01f;
//				this.dy += dy * 0.01f;
			}
		}
			
		
		// ---- gravity and acceleration ---- 
		this.vx += ax;
		this.vy += ay;
		this.x += this.vx;
		this.y += this.vy;
		// ---- screens limits stop at---- /
		if (this.x < pdiam*0.5f) this.dx += (pdiam*0.5f - this.x);
		else if (this.x > 250.0f - pdiam*0.5f) this.dx -= (this.x - 250.0f + pdiam*0.5f);

		
		if (this.y < pdiam*0.5f) this.dy += (pdiam*0.5f - this.y);
		else if (this.y > 250.0f - pdiam*0.5f) this.dy -= (this.y - 250.0f + pdiam*0.5f);		

				int l = this.getNeighbourCount();
				
				for (int j = 0; j < l; j++) 
				{
					float dx = this.getNeighbour(j).x - this.x;
					float dy = this.getNeighbour(j).y - this.y;
					
					float d = Mathf.Sqrt(dx * dx + dy * dy);
					if (d < pdiam) {
						dx = (dx / d) * (pdiam - d) * 0.25f;
						dy = (dy / d) * (pdiam - d) * 0.25f;
						this.dx -= dx;
						this.dy -= dy;
					}

					
				}
	}
}


}
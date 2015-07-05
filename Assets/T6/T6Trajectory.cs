﻿using UnityEngine;
using System.Collections;

public class T6Trajectory : MonoBehaviour {
	// Use this for initialization
    LineRenderer l;
    public Vector3 orbitPane;
	void Start () {
	    
	}

    void OnEnable()
    {
        l = this.GetComponent<T6ViewController>().getLineRenderer();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
       var points =  calculateTrajectory(100);
       drawTrajectory(points);
       
    }

    Vector3[] calculateTrajectory(int iterations)
    {
        //initialize output
        Vector3[] points = new Vector3[iterations];
        //Raw data
        Vector3 velocity = this.GetComponent<Rigidbody>().velocity;
        points[0] = this.transform.position;
        for (int i = 1; i < iterations; i++)
        {
            Vector3 trajectory = velocity;
            Vector3 direction = T6PlantesLogic.planet.transform.position - points[i - 1];
            long r = (long)direction.magnitude;
            direction.Normalize();
            double G = 6.674;
            Vector3 grav = direction * (float)(G * 100 * T6PlantesLogic.planetMass / (r * r));
            trajectory += grav*0.01f;
			velocity = trajectory;
            points[i] = points[i - 1] + trajectory;

 

        }
        orbitPane = Vector3.Cross(points[0] - points[iterations / 2], points[iterations / 2] - points[iterations - 1]);
        return points;

    }

    void drawTrajectory(Vector3[] points)
    {
        l.SetColors(Color.red, Color.red);
        l.SetVertexCount(points.Length);
        l.SetWidth(100, 100);
        l.sortingLayerID = 5;
        for (int i = 0; i < points.Length; i++)
        {
            l.SetPosition(i, points[i]);
        }
    }
}

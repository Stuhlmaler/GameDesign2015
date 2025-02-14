﻿using UnityEngine;
using System.Collections;

public class T4PathHandler : MonoBehaviour {
    // is attached to the ship
    private T4PathCollector pc;
    private int cPP_i, cPO_i; // current Path Point i   ,   current Path Object i
    private float current_distance, next_distance;
    private bool current_distcalc = false;
    private T4Logic logic;
    private int radius = 80;
    private Vector3 prev_pos;
    private T4GUISpeedbarHandler spbar;
    private string prev_tag;
 
	// Use this for initialization
	void Start () {
        cPP_i = 0;
        cPO_i = 0;

        pc = GameObject.Find("Path").GetComponent<T4PathCollector>();
        logic = GameObject.Find("Logic").GetComponent<T4Logic>();
        spbar = this.GetComponent<T4GUISpeedbarHandler>();
        prev_pos = transform.position;
        prev_tag = "Untagged";
	}
	
	// Update is called once per frame
    void FixedUpdate() {
        if (Level.AllowMotion) {
            /** calc closest poiunt on path */
            if (cPP_i+1 < pc.getPathPointCount()-1) {
            
                current_distance = Vector3.Distance(pc.getPathPoint(cPP_i), this.transform.position);
                next_distance = Vector3.Distance(pc.getPathPoint(cPP_i+1), this.transform.position);
                

                if (current_distance > next_distance) {
                    cPP_i++;
                    if (!pc.getPathObject(cPP_i).tag.Equals(prev_tag)) {
                        Debug.Log(pc.getPathObject(cPP_i).tag);
                    }
                    prev_tag = pc.getPathObject(cPP_i).tag;
                }
            }

            /** push ship forward along the path */
            if ((cPP_i + 1 < pc.getPathPointCount() - 1) && (logic.countdownOver)) {
                Vector3 forward = (pc.getPathPoint(cPP_i + 1) - pc.getPathPoint(cPP_i)).normalized * 4000;
                this.GetComponent<Rigidbody>().AddForce(forward);
            }

            /** rotate ship towards current path_point+40 */
            if ((cPP_i + 40 < pc.getPathPointCount() - 1) && (logic.countdownOver)) {
                // pid controller to adjust the torgue of the ship
                // ref: http://webber.physik.uni-freiburg.de/~hon/vorlss02/Literatur/Ingenieurswiss/pid/pid+matlab/PID%20systems%20tutorial.htm
                VectorPid angularVelocityController = new VectorPid(1.7766f, 0, 0.2553191f);
                VectorPid headingController = new VectorPid(1.244681f, 0, 0.06382979f);

                Vector3 target = pc.getPathPoint(cPP_i + 40);
                var angularVelocityError = this.GetComponent<Rigidbody>().angularVelocity * -1;
                Debug.DrawRay(this.transform.position, this.GetComponent<Rigidbody>().angularVelocity * 10, Color.black);

                var angularVelocityCorrection = angularVelocityController.Update(angularVelocityError, Time.deltaTime);
                Debug.DrawRay(this.transform.position, angularVelocityCorrection, Color.green);

                this.GetComponent<Rigidbody>().AddTorque(angularVelocityCorrection);

                var desiredHeading = target - this.transform.position;
                Debug.DrawRay(this.transform.position, desiredHeading, Color.yellow);

                var currentHeading = this.transform.forward;
                Debug.DrawRay(this.transform.position, currentHeading * 15, Color.blue);

                var headingError = Vector3.Cross(currentHeading, desiredHeading);
                var headingCorrection = headingController.Update(headingError, Time.deltaTime);

                this.GetComponent<Rigidbody>().AddTorque(headingCorrection);
            }

            /** drag  ship to its connected point*/
            // apply force
            float perc_dist = current_distance / radius;
            // is the ship in the inner 60% around the actual portalpoint?
            if (perc_dist > 0.6f) {
                // calc the impact 0 - 100% of the force that should be applied
                float impact_factor = (perc_dist - 0.6f) / 0.4f; // 0 - 0.4
                Vector3 push_f = ((pc.getPathPoint(cPP_i) - this.transform.position).normalized * 22500) * impact_factor;
                this.GetComponent<Rigidbody>().AddForce(push_f);
            }

            /** limit velocity of ship */
            float current_vel = fastEuclidDist(transform.position, prev_pos);
            if (current_vel > 3.6f) { // too fast > reduce maxspeed
                spbar.setSpeed(100);
                this.GetComponent<Rigidbody>().velocity = this.GetComponent<Rigidbody>().velocity * 0.99f;
            } else {
                if (current_vel > 3.5f) { 
                    spbar.setSpeed(100); 
                } else {
                    //Debug.Log("current spedd" + (current_vel / 3.6f));
                    int speed = Mathf.RoundToInt((current_vel / 3.6f) * 100);
                    spbar.setSpeed(speed);
                }
            }
            prev_pos = transform.position;
        }
	}

    void OnDrawGizmos() {
        Gizmos.color = new Color32(255, 255, 255, 255);
        Gizmos.DrawLine(pc.getPathPoint(cPP_i), this.transform.position);
    }

    private float fastEuclidDist(Vector3 a, Vector3 b) {
        if (a == b) {
            return 0;
        }
        // fast cause of no squareroot
        return (Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2) + Mathf.Pow(a.z - b.z, 2));
    }
}

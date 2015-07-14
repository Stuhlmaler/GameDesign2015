using UnityEngine;
using System.Collections;

public class T7Level : MonoBehaviour {

	float countDownThreeStart = 0f;
	float countDownThree = 3f;
	bool started = false;

    void OnGUI()
    {
        //if (!Level.AllowMotion)
        if(!started)
		{

            if (GUI.Button(new Rect(Screen.width / 2 - 125, Screen.height / 2, 250, 40), "Start"))
            {
				Debug.Log ("Schiffanzahl ="+Level.ActiveShips.Length);
				Transform[] startPoints = new Transform[Level.ActiveShips.Length];

				for (int i = 0; i< Level.ActiveShips.Length; i++) {
					startPoints[i] = Level.ActiveShips[i].transform;
					addLightsToShip(Level.ActiveShips[i]);
					startPoints[i].position = new Vector3 (-5500 + ( i * 500 ),-1500,1200);
				}
				if(Level.ActiveShips.Length==1){
					startTenSecondCountdown();
					GUI.Box (new Rect (0,0,100,50), "Top-left");

				}

				Level.DefineStartPoints(startPoints);

				foreach (var ship in Level.ActiveShips)
                {
                    ship.Attach(new T1RaceTracker());

                }
				Debug.Log ("############On GUI##########");
				started = true;
                //Level.EnableMotion(true);
            }
        }
        else
        {
			if(countDownThree >=0)
				GUI.Label(new Rect(Screen.width / 2 -150,Screen.height / 2,300,300),"Das Rennen startet in "+(int)countDownThree);
			else{

				Level.EnableMotion(true);
			}
            foreach (var ship in Level.ActiveShips)
            {
                if (ship.ctrlAttachedCamera.enabled)
                {

                    GUI.Label(new Rect(Screen.width * ship.ctrlAttachedCamera.rect.min.x, Screen.height * (1f - ship.ctrlAttachedCamera.rect.max.y), 50, 50), ship.GetAttachment<T1RaceTracker>().progress.ToString());

                }

            }

        }
    }




	// Use this for initialization
	void Start () {
        Level.drag = 0.3f;
        Level.angularDrag = 0.8f;
		Physics.gravity = new Vector3 (0f,0f,0f);

		GameObject mainlight = GameObject.Find ("Directional Light");
		Light l  = (Light) mainlight.GetComponent("Light");
		l.enabled = false;

        Level.InitializationDone();
	}
	
	// Update is called once per frame
	void Update () {
		//10 second-Countdown at Beginning
		if (countDownThreeStart != 0f && countDownThree > 0 )
		{
			countDownThree = 3f -(Time.realtimeSinceStartup - countDownThreeStart);
			int ValueToShow = (int)countDownThree;

		}
		if (countDownThree <= 0) {
			Level.EnableMotion(true);
		}
	}

	void startTenSecondCountdown(){
		countDownThreeStart = Time.realtimeSinceStartup;
	}

	void countDown(){
	}

	void addLightsToShip (Controller ship)
	{
		//Front-Light
		GameObject light_front = new GameObject("Front Light");
		Light light_front_component = ship.gameObject.AddComponent<Light>();
		light_front_component.color = Color.white;
		light_front_component.range = 10000f;
		light_front_component.type = LightType.Spot;
		light_front_component.intensity = 1000f;
		light_front.SetActive (true);
	}
}

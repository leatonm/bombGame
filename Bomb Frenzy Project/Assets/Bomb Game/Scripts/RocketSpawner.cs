using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketSpawner : MonoBehaviour {
	public GameObject _Rocket;



	public void spawnRocketNow()
	{
		Instantiate (_Rocket, transform.position, transform.rotation);
	}
}

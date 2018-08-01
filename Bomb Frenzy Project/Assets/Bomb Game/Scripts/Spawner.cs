using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

	public GameObject[] _BoxesToSpawn;
	public Vector3 _SpawnValue;
	public float _WaitForSpawn;
	public bool _stopSpawner;
	public int _lvlIncrease = 16;
	private float spwntimer;
	public GameObject[] _BossesToSpawn;
	private float BadBlockRate = 0f;





	IEnumerator BoxSpawner()
	{
		yield return new WaitForSeconds (_WaitForSpawn);
		while (!_stopSpawner) 
		{
			if (Random.value <= BadBlockRate) {
				Vector3 spawnPosition = new Vector3 (10, Random.Range (-_SpawnValue.y, _SpawnValue.y), Random.Range (14, _lvlIncrease));
				Instantiate (_BoxesToSpawn [0], spawnPosition, Quaternion.identity);
				yield return new WaitForSeconds (getSpawnerTimer ());
			} else {
				Vector3 spawnPosition = new Vector3 (10, Random.Range (-_SpawnValue.y, _SpawnValue.y), Random.Range (14, _lvlIncrease));
				Instantiate (_BoxesToSpawn [1], spawnPosition, Quaternion.identity);
				yield return new WaitForSeconds (getSpawnerTimer ());
			}
		}
	}


	public void SpawnSpecial()
	{
		Debug.Log ("Spawning Special");
		Vector3 spawnPosition = new Vector3 (10, Random.Range (-_SpawnValue.y, _SpawnValue.y), Random.Range (14, _lvlIncrease));
		Instantiate (_BoxesToSpawn [2], spawnPosition, Quaternion.identity);
	}
		
		


	public void SetGameStart()
	{
		StartCoroutine (BoxSpawner ());
	}


	private float getSpawnerTimer()
	{
		//default timer 3.5f; 
		if (DamageManager.sharedInstance.GetTotalScore() >= 75 && DamageManager.sharedInstance.GetTotalScore() < 200) 
		{
			spwntimer = 2f;
			BadBlockRate = 0.1f;
		}
		if (DamageManager.sharedInstance.GetTotalScore() >=200 && DamageManager.sharedInstance.GetTotalScore() < 300) 
		{
			spwntimer = 1.5f;
			BadBlockRate = 0.2f;
		}
		if (DamageManager.sharedInstance.GetTotalScore() >=300 && DamageManager.sharedInstance.GetTotalScore() < 400) 
		{
			spwntimer = 1f;
			BadBlockRate = 0.2f;
		}
		if (DamageManager.sharedInstance.GetTotalScore() >= 400 && DamageManager.sharedInstance.GetTotalScore() < 500) 
		{
			spwntimer = 3.0f;
			BadBlockRate = 0.3f;
		}
		if (DamageManager.sharedInstance.GetTotalScore() >=500 && DamageManager.sharedInstance.GetTotalScore() < 600) 
		{
			spwntimer = 1.5f;
			BadBlockRate = 0.3f;
		}
		if (DamageManager.sharedInstance.GetTotalScore() >=600 && DamageManager.sharedInstance.GetTotalScore() < 700) 
		{
			spwntimer = 1.0f;
			BadBlockRate = 0.4f;
		}
		if (DamageManager.sharedInstance.GetTotalScore() >=700 && DamageManager.sharedInstance.GetTotalScore() < 800) 
		{
			spwntimer = 1f;
			BadBlockRate = 0.5f;
		}
		if (DamageManager.sharedInstance.GetTotalScore() >=800 && DamageManager.sharedInstance.GetTotalScore() < 900) 
		{
			spwntimer = 1.5f;
			BadBlockRate = 0.6f;
		}
		if (DamageManager.sharedInstance.GetTotalScore() >=900 && DamageManager.sharedInstance.GetTotalScore() < 1000) 
		{
			spwntimer = 1.5f;
			BadBlockRate = 0.7f;
		} if (DamageManager.sharedInstance.GetTotalScore () >= 1000) {
			spwntimer = 1.0f;
			BadBlockRate = 0.8f;
		}if (DamageManager.sharedInstance.GetTotalScore () < 75){
			spwntimer = 2.5f;
			BadBlockRate = 0f;
		}


			
		return spwntimer;
	}

//	private void WaveController()
//	{
//		
//	}

		
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skyboxscript : MonoBehaviour {

	public Material[] _skybox;
	float curRot = 0;
	private int _total = 450;
	int index = 0;

	// Use this for initialization
	void Start () {
		RenderSettings.skybox = _skybox [index];
	}
	
	// Update is called once per frame
	void Update () {
		curRot += 15 * Time.deltaTime;
		curRot %= 360;
		RenderSettings.skybox.SetFloat("_Rotation", curRot);

		ChangeBG ();
	}


	private void ChangeBG()
	{
		if (DamageManager.sharedInstance.GetTotalScore () >= _total) {
			
			index += 1;
			_total += 200;
			RenderSettings.skybox = _skybox [index];

			if (index >= 4) {
				index = -1;
			}

		} else
			return;
	}
		
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

	/*
	 * reset score
	 * reset life
	 * reset ball position
	 * rest timer before game start
	*/


public class InitManager : MonoBehaviour {

	public ThrowRocket rocketScript;
	public float countDownTotalTimer;
	public Text textTimerLabel;
	public GameObject StartButton;
	private bool isGameReady;
	private bool isReadStart;
	public GameObject topBar;
	public Spawner spawner;
	public GameObject theBomb;


	void Start()
	{
		//disbale the rocket movement
		rocketScript.enabled = false;
		textTimerLabel.text ="";
		topBar.SetActive (true);
		isReadStart = false;
		isGameReady = false;
		StartButton.SetActive (true);
		Time.timeScale = 1; //pause
	}

	void LateUpdate() 
	{
		if (isReadStart) {
			if (countDownTotalTimer <= 0 && !isGameReady) {
				textTimerLabel.text = "Go!";
				startTheGame ();	
			} else if (!isGameReady) {
				countDownTotalTimer -= Time.deltaTime;
				textTimerLabel.text = countDownTotalTimer.ToString ("f0");
			}
		} else
			return;
	}


	private void startTheGame()
	{
		Invoke ("disbaleTextonScreen",0.2f);
		isGameReady = true;
		rocketScript.enabled = true;
		this.gameObject.SetActive (false);
		theBomb.SetActive (true);
		//textTimerLabel.enabled = false; //play text out animation
	}

	private void disbaleTextonScreen()
	{
		textTimerLabel.enabled = false; //play text out animation
	}


	public void Gameover()
	{
		Time.timeScale = 0; //pause
		topBar.SetActive (false);
		try{
			rocketScript.enabled = false;
		}catch{
		}

	}



	public bool TimerEnd()
	{
		return isGameReady;
	}



	public void StartButtonClick()
	{
		isReadStart = true;
		StartButton.SetActive (false);
		GameObject ads_first = GameObject.Find ("ads_first");
		textTimerLabel.text ="5";
		spawner.SetGameStart ();

		try{
			ads_first.SetActive (false);
		}catch {
			return;
		}
	}








}

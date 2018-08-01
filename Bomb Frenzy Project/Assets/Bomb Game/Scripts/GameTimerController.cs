using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class GameTimerController : MonoBehaviour {
	
	//private Slider counterSlider;
	//private float counterValue = 1f;
	public Text bombsLeftString;
	private int bombsleft;
	private bool isGameOver;
	private int hitsInARow;
	public InitManager inmgr;

	//user interface
	public GameObject gameOverUI;
	public GameObject BombGainObject;
	private Animator BombAnimator;
	public AudioClip BonusTime;
	private FacebookAndPlayFabManager _facebookAndPlayFabManager;


	// Use this for initialization
	void Start () {
		//counterSlider = this.GetComponent<Slider> ();
		_facebookAndPlayFabManager = FacebookAndPlayFabManager.Instance;
		gameOverUI.SetActive (false);
		bombsleft = 50;
		BombAnimator = BombGainObject.GetComponent<Animator> ();
//		if (_facebookAndPlayFabManager.IsLoggedOnPlayFab) {
//			bombsleft = DamageManager.sharedInstance.GetPlayerBombs ();
//		} else {
//			bombsleft = 100;
//		}
		isGameOver = false;
	}

	// Update is called once per frame
	void Update () {

		if (isGameOver)
		{
			Time.timeScale = 0; //pause
		}
		
		bombsLeftString.text = bombsleft.ToString();
		if (bombsleft <= 0) 
		{
			//Game Over
			gameOver ();
		}

	}

	void setTheBombsRemianing()
	{
		DamageManager.sharedInstance.SetBombs (bombsleft);
	}


	public void sendHit()
	{
		hitsInARow += 1;
		Debug.Log ("Hits are at : " + hitsInARow);
		if (hitsInARow >= 3) {
			GetComponent<AudioSource> ().PlayOneShot (BonusTime);
			PlayBombGainAnimation ("+5");
			//updateTime (0.09f);
			gainBombs(5);
			hitsInARow = 0;
		}
	}

	public void setBombleft(int x)
	{
		PlayBombGainAnimation ("+"+x.ToString());
		bombsleft += x;
	}

	public void gainBombs(int x)
	{
		bombsleft = bombsleft + x;
		//DamageManager.sharedInstance.SetPlayerBombs (bombsleft);
	}

	public void gameOver()
	{
		setTheBombsRemianing ();
		isGameOver = true;
		gameOverUI.SetActive (true);
		DamageManager.sharedInstance.UpdateScore ();
		inmgr.Gameover();
	}

	private void PlayBombGainAnimation (string x)
	{
		Text txValue = GameObject.Find ("GainText").GetComponent<Text> ();
		GetComponent<AudioSource> ().PlayOneShot (BonusTime);
		txValue.text = x;
		BombAnimator.Play ("TimeGain");
	}

}

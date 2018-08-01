using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageManager : MonoBehaviour {
	public static DamageManager sharedInstance = null;
	public GameObject ScoreText;
	public GameObject damageText;
	private int totalScore;
	public Text publictotalScore;
	public Text GameoverSocre;
	private FacebookAndPlayFabManager _facebookAndPlayFabManager;
	public GameObject facebookButton;
	public GameObject facebookUI;

	public GameObject WaittoSubmit;
	public GameObject DoneWaiting;
	private bool instantSpawn;

	private int bloxspeed = 2; //Default Speed
	private int BombsLeft;



	void Awake()
	{
		if (sharedInstance == null) 
		{
			sharedInstance = this;
		} else if (sharedInstance != this) 
		{
			Destroy (gameObject);
		}
	}

	void Start()
	{
		_facebookAndPlayFabManager = FacebookAndPlayFabManager.Instance;
	}

	public void setInstantSpawnStatus(bool x)
	{
		instantSpawn = x;
	}

	public bool getInstantSpawnStatus()
	{
		return instantSpawn;
	}

	public void DoDamage (int damage, Vector3 pos)
	{
		setScore (damage);
		GameObject damageT = Instantiate(damageText, pos, Quaternion.identity) as GameObject;
		damageT.gameObject.GetComponent<TextMesh> ().text = damage.ToString ();

		Destroy (damageT, 1.0f);
	}
		



	private void setScore(int scr)
	{
		totalScore = totalScore + scr;
		ScoreText.GetComponent<Animator>().Play("AddScore");
		ScoreText.GetComponent<Text>().text = totalScore.ToString ();
		GameoverSocre.text = totalScore.ToString ();
	}

	public int GetTotalScore()
	{
		return totalScore;
	}

	public void UpdateScore()
	{
		publictotalScore.text = GetTotalScore ().ToString ();
		Debug.Log ("Submitting Score");
		if (_facebookAndPlayFabManager.IsLoggedOnFacebook) {
			Debug.Log ("is logged in to Facebook");
			submitScore ();
		} else 
		{
			Debug.Log ("is NOT logged in to Facebook");
			facebookUI.SetActive (true); //hide facebook Dailog box
		}
	}


	void submitScore()
	{
		//get a fresh updated user information
		_facebookAndPlayFabManager._GetUserData ();
		_facebookAndPlayFabManager._SetUserData (GetTotalScore(), BombsLeft);
	}


	public void fblogin()
	{
		Debug.Log ("Trying to sign in");
		if (string.IsNullOrEmpty(_facebookAndPlayFabManager.playFabTitleId))
		{
			Debug.LogError("PlayFab Title Id is null.");
			return;
		}
		_facebookAndPlayFabManager.LogOnFacebook(res =>
			{
				StartCoroutine(WaitForPlayFabLoginCoroutine());
			});
	}


	private IEnumerator WaitForPlayFabLoginCoroutine()
	{
		waiting();
		Debug.Log ("signed in Facebook Good");
		yield return new WaitUntil(() => _facebookAndPlayFabManager.IsLoggedOnPlayFab);
		Debug.Log ("sconnect to playfab Good");
		//submitScore();
		doneWaiting();
		facebookButton.SetActive (false); //hide facebook Dailog
		facebookUI.SetActive (false);
	}

//	public int GetPlayerBombs()
//	{
//		return _facebookAndPlayFabManager.OnGetUserBombs ();
//	}

	public void SetBombs(int x)
	{
		BombsLeft = x;
	}

	public void NeverMind()
	{
		Debug.Log ("Never Mind");
		facebookUI.SetActive (false);
	}

	void waiting()
	{
		WaittoSubmit.SetActive (true);
		DoneWaiting.SetActive(false);
	}

	private void doneWaiting()
	{
		WaittoSubmit.SetActive (false);
		DoneWaiting.SetActive(true);
		Invoke ("removeUI",0.4f);
	}

	private void removeUI()
	{
		WaittoSubmit.SetActive (false);
		DoneWaiting.SetActive(false);
	}
		

	public int getBlockSpeed()
	{
		if (GetTotalScore() >= 75 && GetTotalScore () < 400) 
		{
			bloxspeed = 2;
		} 
		if (GetTotalScore() >= 400 && GetTotalScore () < 700) 
		{
			bloxspeed = 4;
		} 
		if (GetTotalScore() >= 700 && GetTotalScore () < 800) 
		{
			bloxspeed = 5;
		} 
		if (GetTotalScore() >= 800 && GetTotalScore () < 900) 
		{
			bloxspeed = 2;
		} 
		if (GetTotalScore() >= 900 && GetTotalScore () < 1000) 
		{
			bloxspeed = 5;
		} 
		if (GetTotalScore () >= 1000) {
			bloxspeed = 1;
		} if(GetTotalScore() < 75) {
			bloxspeed = 2;
		} 
		return bloxspeed;
	}


}

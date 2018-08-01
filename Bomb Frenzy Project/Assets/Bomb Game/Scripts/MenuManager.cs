using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

	public GameObject pausemenu;
	public GameObject PleaseWait;

	public void onMenuClick()
	{
		Time.timeScale = 0; //pause
		pausemenu.SetActive(true);//show the menu
	}

	public void onRetryClick()
	{
		PleaseWait.SetActive (true);
		Time.timeScale = 1; //pause
		pausemenu.SetActive(false);//show the menu
		SceneManager.LoadScene("game", LoadSceneMode.Single);//reload scene
	}

	public void onPlayClick()
	{
		Time.timeScale = 1; //pause
		pausemenu.SetActive(false);//show the menu
	}

	public void onGoHomeClick()
	{
		PleaseWait.SetActive (true);
		Time.timeScale = 1; //pause
		pausemenu.SetActive(false);//show the menu
		SceneManager.LoadScene("main");//back to main menu
	}
}

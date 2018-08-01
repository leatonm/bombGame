using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SampleScene : MonoBehaviour
{
    private int _entries = 0;
    private bool _canLoadMoreEntries = true;
    private FacebookAndPlayFabManager _facebookAndPlayFabManager;
    private ScrollRect _scrollRect;
    private Sprite _defaultFacebookSprite;


    [Header("Login section screen objects")]
    public Image profilePicture;
    public Text userName;
	public GameObject facebookButton;
	public GameObject ProfileUI;
    //public Text loginButtonText;
	public Text _PlayerHighScore;
	private bool isplayerHightScoreSet;
	private bool newsLoaded;
	private bool showNews;
	private List<TitleNewsItem> news;
	//public Text _PlayerBombs;
	//public Text _PlayerLife;
	//public GameObject facebookShare;
	//public GameObject facebookboard;

    [Space(10)]
    [Header("Leaderboard section screen objects")]
    public GameObject leaderboardPanel;
    public Slider filterSlider;
    public LeaderboardEntry entryPrefab;
    public Transform leaderboardEntryParent;
    public GameObject messagePanel;
    public Scrollbar leaderboardScrollbar;
	public GameObject leaderboardGameObject;
	public GameObject menuElement;


[Space(10)]
[Header("Top 3 Players Ranked")]
  public Image[] TopPicture; 
  public Text[] TopScore;
  public Text[] TopName;
  public GameObject[] TopPlayerUI;


    [Space(10)]
    [Header("Leaderboard parameters")]
    public int maxResultsCount;


	[Space(10)]
	[Header("most be logged into Facebook animation")]
	public Animator facebook_Animator;


	[Space(10)]
	[Header("Settings")]
	public GameObject SettingsMenu;
	public Text FbButtonText;

    private void Start()
    {
		SettingsMenu.SetActive (false);
		Debug.Log ("Todays date: " + System.DateTime.Now.Date);
		//facebookShare.SetActive (false);
		//facebookboard.SetActive (false);
		isplayerHightScoreSet = false;
        _facebookAndPlayFabManager = FacebookAndPlayFabManager.Instance;
        _defaultFacebookSprite = profilePicture.sprite;

		if (_facebookAndPlayFabManager.IsLoggedOnFacebook) 
		{
			AlreadyLogin ();
			FbButtonText.text = "LOGOUT";
		}

    }


	private void AlreadyLogin()
	{
		StartCoroutine(WaitForPlayFabLoginCoroutine());
		StartCoroutine(WaitForUserNameCoroutine());
		StartCoroutine(WaitForProfilePictureCoroutine());
		StartCoroutine(WaitForPlayerDataCoroutine());

	}

    public void FacebookLogin()
    {
        if (string.IsNullOrEmpty(_facebookAndPlayFabManager.playFabTitleId))
        {
            Debug.LogError("PlayFab Title Id is null.");
            return;
        }

        if (_facebookAndPlayFabManager.IsLoggedOnFacebook)
        {
            _facebookAndPlayFabManager.LogOutFacebook();
            profilePicture.sprite = _defaultFacebookSprite;
            userName.text = "Not Signed into Facebook";
			ProfileUI.SetActive (false);
			facebookButton.SetActive (true);//loginButtonText.text = "LOGIN";
			FbButtonText.text = "LOGIN";
			//facebookShare.SetActive (false);
			//facebookboard.SetActive (false);
			TopPlayerUI[0].SetActive (false); //disable top daily players ui
            ClearLeaderboard();
            leaderboardPanel.SetActive(false); //turn off Facebook leaderboard Panel when signed out
            return;
        }

        _facebookAndPlayFabManager.LogOnFacebook(res =>
        {
            StartCoroutine(WaitForPlayFabLoginCoroutine());
            StartCoroutine(WaitForUserNameCoroutine());
            StartCoroutine(WaitForProfilePictureCoroutine());
			StartCoroutine(WaitForPlayerDataCoroutine());
            
        });
    }


    private IEnumerator WaitForPlayFabLoginCoroutine()
    {
        yield return new WaitUntil(() => _facebookAndPlayFabManager.IsLoggedOnPlayFab);
		//LoadUserDataMoeny ();
		facebookButton.SetActive (false); //loginButtonText.text = "LOGOUT";
		//facebookShare.SetActive (true);
		//facebookboard.SetActive (true);
		ProfileUI.SetActive (true);
		FbButtonText.text = "LOGOUT";
		SettingsMenu.SetActive (false);
		_facebookAndPlayFabManager._GetUserData ();
		GetTopPlayers();
		Debug.Log ("Getting Player Data");
		LoadNews ();
    }

    private IEnumerator WaitForUserNameCoroutine()
    {
        yield return new WaitUntil(() => !string.IsNullOrEmpty(_facebookAndPlayFabManager.FacebookUserName));
        userName.text = _facebookAndPlayFabManager.FacebookUserName;
    }

    private IEnumerator WaitForProfilePictureCoroutine()
    {
        yield return new WaitUntil(() => _facebookAndPlayFabManager.FacebookUserPictureSprite != null);
        profilePicture.sprite = _facebookAndPlayFabManager.FacebookUserPictureSprite;
    }

	//New Data here
	private IEnumerator WaitForPlayerDataCoroutine()
	{
		yield return new WaitUntil(() => _facebookAndPlayFabManager.IsPlayerDataDone);
		try{
//			_PlayerBombs.text = _facebookAndPlayFabManager._result.Data["Bombs"].Value.ToString();
			_PlayerHighScore.text = _facebookAndPlayFabManager._result.Data["Highscore"].Value.ToString();
		}catch{
			Debug.Log ("Most be a new player -- Setting up new information");
		}
	}
		

	//players click the leaderboard Icon
	public void onLeaderboardClick()
	{
		if (!_facebookAndPlayFabManager.IsLoggedOnFacebook)
		{
			Debug.Log("Cannot Show Leaderboard => Not logged on Facebook!");
			//play anumation most be logged into Facebook
			facebook_Animator.Play("mostBeLogged");
			return;
		}

		ClearLeaderboard();
		menuElement.SetActive (false); //hide  or animate menu out
		TopPlayerUI[0].SetActive (false); //disable top daily players ui
		leaderboardGameObject.SetActive (true); //animationEvent menu in
		_scrollRect = leaderboardEntryParent.GetComponentInParent<ScrollRect>();
		GetLeaderboard(PlayFabStatConstants.Test, filterSlider.value == 0, 0); // this starts the leaderboard
	}


	public void onBackButtonClick()
	{
		menuElement.SetActive (true); //show or animate menu out
		facebookButton.SetActive (false); //loginButtonText.text = "LOGOUT";
		GetTopPlayers(); // enable top 3 daily players UI
		leaderboardGameObject.SetActive (false); //animationEvent menu out
	}



//get the top players
private void GetTopPlayers()
{
    _facebookAndPlayFabManager.GetLeaderboard(PlayFabStatConstants.DailyBoard, false, 3, GetTopPlayerCallback, 0);
}

 public void GetTopPlayerCallback(GetLeaderboardResult result)
{
		if (result.Leaderboard.Count > 0) {TopPlayerUI [0].SetActive (true);} //enable the top players frame


    //int index = 0;
    foreach (var entry in result.Leaderboard)
    {
			TopPlayerUI [entry.Position + 1].SetActive (true); //enable the frames before using them

        if(entry.Position == 0)
        {
            TopScore[0].text = entry.StatValue.ToString();

				_facebookAndPlayFabManager.GetFacebookUserName(entry.DisplayName, res =>
					{
						TopName[0].text = res.ResultDictionary["name"].ToString();
					});
				
            _facebookAndPlayFabManager.GetFacebookUserPicture(entry.DisplayName, 100, 100, res =>
                {
                    TopPicture[0].sprite = Sprite.Create(res.Texture, new Rect(0, 0, 100, 100), Vector2.zero);
                });
				
        }
            if(entry.Position == 1)
        {
            TopScore[1].text = entry.StatValue.ToString();
				_facebookAndPlayFabManager.GetFacebookUserName(entry.DisplayName, res =>
					{
						TopName[1].text = res.ResultDictionary["name"].ToString();
					});
            _facebookAndPlayFabManager.GetFacebookUserPicture(entry.DisplayName, 100, 100, res =>
                {
                    TopPicture[1].sprite = Sprite.Create(res.Texture, new Rect(0, 0, 100, 100), Vector2.zero);
                });
        }
            if(entry.Position == 2)
        {
            TopScore[2].text = entry.StatValue.ToString();
				_facebookAndPlayFabManager.GetFacebookUserName(entry.DisplayName, res =>
					{
						TopName[2].text = res.ResultDictionary["name"].ToString();
					});
            _facebookAndPlayFabManager.GetFacebookUserPicture(entry.DisplayName, 100, 100, res =>
                {
                    TopPicture[2].sprite = Sprite.Create(res.Texture, new Rect(0, 0, 100, 100), Vector2.zero);
                });
        }
        
    }
}

    public void GetLeaderboard(string statisticName, bool friendsOnly, int startPosition)
    {
        _scrollRect.vertical = false;
        messagePanel.SetActive(true);
        _facebookAndPlayFabManager.GetLeaderboard(statisticName, friendsOnly, maxResultsCount, GetLeaderboardCallback, startPosition);
    }

    public void GetLeaderboardCallback(GetLeaderboardResult result)
    {
        _scrollRect.vertical = true;
        messagePanel.SetActive(false);
        filterSlider.interactable = true;
        leaderboardPanel.SetActive(true);

        if (result.Leaderboard.Count < maxResultsCount)
            _canLoadMoreEntries = false;

        foreach (PlayerLeaderboardEntry playerEntry in result.Leaderboard)
        {
            LeaderboardEntry entry = Instantiate(entryPrefab.gameObject, leaderboardEntryParent).GetComponent<LeaderboardEntry>();

            int width = 100;
            int height = 100;

            entry.SetUserPosition(playerEntry.Position + 1);
            entry.SetUserScore(playerEntry.StatValue);

            if (playerEntry.DisplayName == _facebookAndPlayFabManager.FacebookUserId)
            {
                entry.SetUserName(_facebookAndPlayFabManager.FacebookUserName);
                entry.SetUserPictureSprite(_facebookAndPlayFabManager.FacebookUserPictureSprite);
            }
            else
            {
                _facebookAndPlayFabManager.GetFacebookUserName(playerEntry.DisplayName, res =>
                {
                    entry.SetUserName(res.ResultDictionary["name"].ToString());
                });

                _facebookAndPlayFabManager.GetFacebookUserPicture(playerEntry.DisplayName, width, height, res =>
                {
                    entry.SetUserPictureSprite(Sprite.Create(res.Texture, new Rect(0, 0, width, height), Vector2.zero));
                });

                // ATTENTION:
                // If you're having trouble getting the profile picture please comment the call above and uncomment the following.

                //_facebookAndPlayFabManager.GetFacebookUserPictureFromUrl(playerEntry.DisplayName, width, height, res =>
                //{
                //    StartCoroutine(_facebookAndPlayFabManager.GetTextureFromGraphResult(res, tex =>
                //    {
                //        entry.SetUserPictureSprite(Sprite.Create(tex, new Rect(0, 0, width, height), Vector2.zero));
                //    }));
                //});
            }

            _entries++;
        }
    }

    public void ClearLeaderboard()
    {
        for (int i = 0; i < leaderboardEntryParent.childCount; i++)
        {
            Destroy(leaderboardEntryParent.GetChild(i).gameObject);
        }
    }

    public void OnScrollbarValueChanged()
    {
        if (leaderboardScrollbar.value == 0)
        {
            if (_canLoadMoreEntries)
                GetLeaderboard(PlayFabStatConstants.Test, filterSlider.value == 0, _entries);
        }
    }

    public void OnFilterChanged()
    {
        if (!_facebookAndPlayFabManager.IsLoggedOnPlayFab)
            return;

        ClearLeaderboard();

        filterSlider.interactable = false;

        GetLeaderboard(PlayFabStatConstants.Test, filterSlider.value == 0, 0);
    }



	//Get money from facebook account


    public void FacebookShare()
    {
		if (!_facebookAndPlayFabManager.IsLoggedOnFacebook)
		{
			facebook_Animator.Play("mostBeLogged");
			return;
		}
        _facebookAndPlayFabManager.ShareOnFacebook();
    }

    public void FacebookInvite()
    {
        _facebookAndPlayFabManager.InviteOnFacebook();
    }
    
	public void GoToGameScene()
	{
		messagePanel.SetActive(true);
		SceneManager.LoadScene("game", LoadSceneMode.Single);
	}
	//====================TEST
    public void _UpdateScore()
    {
		//_facebookAndPlayFabManager.UpdateStat("score", 458);
		//_facebookAndPlayFabManager._SetUserData ();
    }

	public void _GetScore()
	{
		//_facebookAndPlayFabManager._GetUserData ();
	}

	void LateUpdate()
	{
		if (_facebookAndPlayFabManager.IsLoggedOnPlayFab && !isplayerHightScoreSet) {
			//_PlayerBombs.text = _facebookAndPlayFabManager._result.Data["Bombs"].Value.ToString();
			try{
				_PlayerHighScore.text = _facebookAndPlayFabManager._result.Data["Highscore"].Value.ToString(); } catch {
			}
			isplayerHightScoreSet = true;
		}
	}

	public void CloseWlecomeMessage()
	{
		//_PlayerBombs.text = _facebookAndPlayFabManager._result.Data["Bombs"].Value.ToString();
	}





	private void LoadNews()
	{
		GetTitleNewsRequest request = new GetTitleNewsRequest ();
		PlayFabClientAPI.GetTitleNews (request, OnNewsResult, OnPlayFabError);
	}


	private void OnNewsResult(GetTitleNewsResult result)
	{
		if (result.News != null) {
			news = result.News;
			if (news.Count > 0) {
				showNews = true;

				Debug.Log ("News date is: " + news[0].Timestamp.Date); //system.datatime
				Debug.Log ("Message of the day Body: " + news [0].Title);
				Debug.Log ("Message of the day Title: " + news [0].Body);
			} else {
				Debug.Log ("Empty Message");
			}
		} else
			Debug.Log ("No Message!");
	}

	private void OnPlayFabError(PlayFabError error)
	{
		Debug.Log ("Got an Error Getting Message");
	}



	public void showSettings()
	{
		SettingsMenu.SetActive (true);
	}


	public void leaveGame()
	{
		Application.Quit();
	}

	public void FacebookLogout()
	{
		FacebookLogin ();
	}
}




/*
	private void SavePlayerState()
		{
			if (PlayFabGameBridge.totalKills != totalKillsOld && PlayFabData.AuthKey != null) {	// we need to save
				// first save as a player property (poor man's save game)
				Debug.Log ("Saving player data...");
				UpdateUserDataRequest request = new UpdateUserDataRequest ();
				request.Data = new Dictionary<string, string> ();
				request.Data.Add ("TotalKills", PlayFabGameBridge.totalKills.ToString ());
				PlayFabClientAPI.UpdateUserData (request, PlayerDataSaved, OnPlayFabError);

				// also save score as a stat for the leaderboard use...
				Dictionary<string,int> stats = new Dictionary<string,int> ();
				stats.Add("score", PlayFabGameBridge.totalKills);
				storeStats(stats);
			}
			totalKillsOld = PlayFabGameBridge.totalKills;
		}
*/
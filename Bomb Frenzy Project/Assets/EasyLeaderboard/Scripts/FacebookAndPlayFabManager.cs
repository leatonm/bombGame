using Facebook.Unity;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class FacebookAndPlayFabManager : MonoSingleton<FacebookAndPlayFabManager>
{
    private string[] _permissions = { "public_profile", "email", "user_friends" };
    private const int PictureWidth = 140;
    private const int PictureHeight = 140;
	public GetUserDataResult _result;
	public bool IsPlayerDataDone { get; private set; }

	[Header("iOS Ads")]
	[Tooltip("GameID iOS")]
	public string ads_iOSGameId;


	[Header("Android Ads")]
	[Tooltip("GameID Android")]
	public string ads_AndroidId;


    [Header("PlayFab Info")]
    [Tooltip("ID of your application on PlayFab")]
    public string playFabTitleId;

    [Header("Facebook Share Info")]
    [Tooltip("Application URL")]
    public string contentUrl;

    [Tooltip("Application name")]
    public string contentTitle;

    [TextArea]
    [Tooltip("Message to your Facebook friends")]
    public string contentDescription;

    [Tooltip("Application image URL")]
    public string pictureUrl;

    [Header("Facebook Invite Info")]
    [Tooltip("Facebook App URL")]
    public string appLink;

    [Tooltip("Preview Image URL")]
    public string previewImageLink;

    public string PlayFabUserId { get; private set; }
    public bool IsLoggedOnPlayFab { get; private set; }
    public string UserCountryCode { get; private set; }
    public Sprite UserCountryFlagSprite { get; private set; }

    public bool IsLoggedOnFacebook { get; private set; }
    public string FacebookUserId { get; private set; }
    public string FacebookUserName { get; private set; }
    public Sprite FacebookUserPictureSprite { get; private set; }

	//private int PlayerBombs;
	public GameObject WelcomeMessage;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
		adsINIT ();
        Debug.Log("FacebookAndPlayFabManager.Awake => FB.IsInitialized: " + FB.IsInitialized);

        if (FB.IsInitialized)
            return;

        FB.Init(() => FB.ActivateApp());
    }


	private void adsINIT()
	{
		Debug.Log ("initializing ads");

		//setting up ads
		#if UNITY_IOS
			string ads_gameid =  ads_iOSGameId;
		#elif UNITY_ANDROID
			string ads_gameid = ads_AndroidId;
		#endif
		//setting up ads

		Advertisement.Initialize(ads_gameid, true);
	}



    private void Start()
    {
        PlayFabSettings.TitleId = playFabTitleId;
    }

    /// <summary>
    /// Logs the player into Facebook.
    /// </summary>
    /// <param name="successCallback">Action to be executed when the process is done correctly.</param>
    /// <param name="errorCallback">Action to be executed when the process fails.</param>
    public void LogOnFacebook(Action<ILoginResult> successCallback = null, Action<ILoginResult> errorCallback = null)
    {
        Debug.Log("FacebookAndPlayFabManager.LogOnFacebook => FB.IsInitialized: " + FB.IsInitialized);
        Debug.Log("FacebookAndPlayFabManager.LogOnFacebook => FB.IsLoggedIn: " + FB.IsLoggedIn);

        if (FB.IsLoggedIn)
        {
            SetLoggedInfo();

            if (successCallback != null)
                successCallback(null);

            return;
        }

        FB.LogInWithReadPermissions(_permissions,
            (res =>
            {
                if (!ValidateResult(res))
                {
                    if (errorCallback != null)
                        errorCallback(res);

                    return;
                }

                FacebookUserId = res.ResultDictionary["user_id"].ToString();

                Debug.Log("FacebookAndPlayFabManager.LogOnFacebook => FB.IsLoggedIn: " + FB.IsLoggedIn);

                SetLoggedInfo();

                if (successCallback != null)
                    successCallback(res);
            }));
    }

    private void SetLoggedInfo()
    {
        IsLoggedOnFacebook = true;

        GetFacebookUserName("me", res =>
        {
            FacebookUserName = res.ResultDictionary["name"].ToString();
        });

        LogOnPlayFab();
    }

    /// <summary>
    /// Gets the player's Facebook profile name.
    /// </summary>
    /// <param name="id">Unique identifier of a Facebook profile.</param>
    /// <param name="successCallback">Action to be executed when the process is done correctly.</param>
    /// <param name="errorCallback">Action to be executed when the process fails.</param>
    public void GetFacebookUserName(string id, Action<IGraphResult> successCallback = null, Action<IGraphResult> errorCallback = null)
    {
        //FB.API("/" + id + "?fields=name,picture.height(100).width(100)", HttpMethod.GET,

        FB.API("/" + id, HttpMethod.GET,
            (res =>
            {
                if (!ValidateResult(res))
                {
                    if (errorCallback != null)
                        errorCallback(res);

                    return;
                }

                Debug.Log("FacebookAndPlayFabManager.GetFacebookUserName => Success!");

                if (successCallback != null)
                    successCallback(res);
            }));
    }

    /// <summary>
    /// Gets the player's Facebook profile picture.
    /// </summary>
    /// <param name="id">Unique identifier of a Facebook profile.</param>
    /// <param name="width">Width the returning image is supposed to have.</param>
    /// <param name="height">Height the returning image is supposed to have.</param>
    /// <param name="successCallback">Action to be executed when the process is done correctly.</param>
    /// <param name="errorCallback">Action to be executed when the process fails.</param>
    public void GetFacebookUserPicture(string id, int width, int height, Action<IGraphResult> successCallback = null, Action<IGraphResult> errorCallback = null)
    {
        string query = string.Format("/{0}/picture?type=square&height={1}&width={2}", id, height, width);

        FB.API(query, HttpMethod.GET,
            (res =>
            {
                if (!ValidateResult(res) || res.Texture == null)
                {
                    if (errorCallback != null)
                        errorCallback(res);

                    return;
                }

                if (successCallback != null)
                    successCallback(res);

                Debug.Log("FacebookAndPlayFabManager.GetFacebookUserPicture => Success!");
            }));
    }

    public void GetFacebookUserPictureFromUrl(string id, int width, int height, Action<IGraphResult> successCallback = null, Action<IGraphResult> errorCallback = null)
    {
        string query = string.Format("/{0}/picture?type=square&height={1}&width={2}&redirect=false", id, height, width);

        FB.API(query, HttpMethod.GET,
            (res =>
            {
                if (!ValidateResult(res))
                {
                    if (errorCallback != null)
                        errorCallback(res);

                    return;
                }

                if (successCallback != null)
                    successCallback(res);

                Debug.Log("FacebookAndPlayFabManager.GetFacebookUserPictureUrl => Success!");
            }));
    }

    public IEnumerator GetTextureFromGraphResult(IGraphResult result, Action<Texture2D> callback)
    {
        var data = result.ResultDictionary["data"] as IDictionary<string, object>;
        var url = data["url"].ToString();

        WWW request = new WWW(url);

        yield return new WaitUntil(() => request.isDone);

        if (callback != null)
            callback(request.texture);
    }

    /// <summary>
    /// Logs the player out of Facebook.
    /// </summary>
    public void LogOutFacebook()
    {
        FB.LogOut();

        IsLoggedOnFacebook = false;
        IsLoggedOnPlayFab = false;
        FacebookUserName = string.Empty;
        FacebookUserPictureSprite = null;
    }

    private void LogOnPlayFab()
    {
        if (IsLoggedOnPlayFab)
            return;

        var loginWithFacebookRequest = new LoginWithFacebookRequest
        {
            TitleId = PlayFabSettings.TitleId,
            AccessToken = AccessToken.CurrentAccessToken.TokenString,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithFacebook(loginWithFacebookRequest, PlayFabLoginSuccessCallback, PlayFabErrorCallback);
    }

    private void PlayFabLoginSuccessCallback(PlayFab.ClientModels.LoginResult result)
    {
        Debug.Log("FacebookAndPlayFabManager.LogOnPlayFab => Success!");

        IsLoggedOnPlayFab = true;
        PlayFabUserId = result.PlayFabId;

        GetFacebookUserPicture("me", PictureHeight, PictureWidth, res =>
        {
            FacebookUserPictureSprite = Sprite.Create(res.Texture, new Rect(0, 0, PictureHeight, PictureWidth), Vector2.zero);
        });

        // ATTENTION:
        // If you're having trouble getting the profile picture please comment the call above and uncomment the following.

        //GetFacebookUserPictureFromUrl("me", PictureWidth, PictureHeight, res =>
        //{
        //    StartCoroutine(GetTextureFromGraphResult(res, tex =>
        //    {
        //        FacebookUserPictureSprite = Sprite.Create(tex, new Rect(0, 0, PictureWidth, PictureHeight), Vector2.zero);
        //    }));
        //});

        UpdatePlayFabDisplayUserName();

    }

    private void PlayFabErrorCallback(PlayFabError error)
    {
        string message = string.Format("FacebookAndPlayFabManager.PlayFabErrorCallback => {0}\r\n", error.ErrorMessage);

        foreach (var item in error.ErrorDetails)
            message += string.Format("{0} => {1}\r\n", item.Key, string.Join(" | ", item.Value.ToArray()));

        Debug.LogError(message);
    }

    /// <summary>
    /// Updates the value of a given PlayFab statistic.
    /// </summary>
    /// <param name="statisticName">Name of the PlayFab statistic to be updated.</param>
    /// <param name="value">Value the statistic must receive.</param>
    /// <param name="successCallback">Action to be executed when the process is done correctly.</param>
    public void UpdateStat(string statisticName, int value, Action<UpdatePlayerStatisticsResult> successCallback = null)
    {
        if (!IsLoggedOnPlayFab)
        {
            Debug.Log("FacebookAndPlayFabManager.UpdateStat => Not logged on PlayFab!");
            return;
        }

        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = statisticName,
                    Value = value
                }
            }
        };

        successCallback += (result) =>
        {
            Debug.Log("FacebookAndPlayFabManager.UpdateStat => Success!");
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, successCallback, PlayFabErrorCallback);
    }


    private void UpdatePlayFabDisplayUserName()
    {
        if (!IsLoggedOnPlayFab)
        {
            Debug.Log("FacebookAndPlayFabManager.UpdatePlayFabDisplayUserName => Not logged on PlayFab!");
            return;
        }

        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = FacebookUserId
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, null, PlayFabErrorCallback);
    }

    /// <summary>
    /// Gets values from a given PlayFab statistic.
    /// </summary>
    /// <param name="statisticName">Name of the PlayFab statistic to be retrieved.</param>
    /// <param name="friendsOnly">Only retrieve info from Facebook friends?</param>
    /// <param name="maxResultsCount">Maximum number of records to retrieve.</param>
    /// <param name="successCallback">Action to be executed when the process is done correctly.</param>
    /// <param name="startPosition">Starting point to retrieve the statistic values.</param>
    public void GetLeaderboard(string statisticName, bool friendsOnly, int maxResultsCount, Action<GetLeaderboardResult> successCallback, int startPosition = 0)
    {
        if (!IsLoggedOnPlayFab)
        {
            Debug.Log("FacebookAndPlayFabManager.GetFriendLeaderboard => Not logged on PlayFab!");
            return;
        }

        if (friendsOnly)
        {
            var request = new GetFriendLeaderboardRequest
            {
                StatisticName = statisticName,
                MaxResultsCount = maxResultsCount,
                IncludeFacebookFriends = true,
                StartPosition = startPosition
            };

            PlayFabClientAPI.GetFriendLeaderboard(request, successCallback, PlayFabErrorCallback);
        }
        else
        {
            var request = new GetLeaderboardRequest
            {
                StatisticName = statisticName,
                MaxResultsCount = maxResultsCount,
                StartPosition = startPosition
            };

            PlayFabClientAPI.GetLeaderboard(request, successCallback, PlayFabErrorCallback);
        }
    }

    /// <summary>
    /// Shares a post on Facebook.
    /// </summary>
    /// <param name="successCallback">Action to be executed when the process is done correctly.</param>
    /// <param name="errorCallback">Action to be executed when the process fails.</param>
    public void ShareOnFacebook(Action<IShareResult> successCallback = null, Action<IShareResult> errorCallback = null)
    {
        if (!IsLoggedOnFacebook)
        {
            Debug.Log("FacebookAndPlayFabManager.ShareOnFacebook => Not logged on Facebook!");
            return;
        }

        FB.ShareLink(new Uri(contentUrl), contentTitle, contentDescription, new Uri(pictureUrl),
            res =>
            {
                if (!ValidateResult(res))
                {
                    if (errorCallback != null)
                        errorCallback(res);

                    return;
                }

                if (successCallback != null)
                    successCallback(res);
            });
    }

    /// <summary>
    /// Sends friends a notification inviting them to download your app.
    /// </summary>
    /// <param name="successCallback">Action to be executed when the process is done correctly.</param>
    /// <param name="errorCallback">Action to be executed when the process fails.</param>
    public void InviteOnFacebook(Action<IAppInviteResult> successCallback = null, Action<IAppInviteResult> errorCallback = null)
    {
        if (!IsLoggedOnFacebook)
        {
            Debug.Log("FacebookAndPlayFabManager.InviteOnFacebook => Not logged on Facebook!");
            return;
        }

        if (string.IsNullOrEmpty(appLink))
        {
            Debug.LogError("FacebookAndPlayFabManager.InviteOnFacebook => App Link is empty!");
            return;
        }

        Uri previewImageUri = null;

        if (!string.IsNullOrEmpty(previewImageLink))
            previewImageUri = new Uri(previewImageLink);

        FB.Mobile.AppInvite(new Uri(appLink), previewImageUri, res =>
        {
            if (!ValidateResult(res))
            {
                if (errorCallback != null)
                    errorCallback(res);

                return;
            }

            if (successCallback != null)
                successCallback(res);
        });
    }

    private bool ValidateResult(IResult result)
    {
        if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
            return true;

        Debug.Log(string.Format("{0} is invalid (Cancelled={1}, Error={2}, JSON={3})", 
            result.GetType(), result.Cancelled, result.Error, Facebook.MiniJSON.Json.Serialize(result.ResultDictionary)));

        return false;
    }

	//Lets Try to save player data;
	public void _SetUserData(int score, int _b) {
		IsPlayerDataDone = false;

		int high = 0; 
//		int daily = 0;

		try {
			high = int.Parse(_result.Data ["Highscore"].Value);
		} 
		
		catch{
			high = score;
		}

//		try { 
//			daily = int.Parse(_result.Data ["Dailyscore"].Value); } catch { daily = score;}
	

		if(score >= high){ 
			high = score;
			UpdateStat("score", high); //global
		}

//		if(score >= daily){ 
//			daily = score;
//			UpdateStat("daily", daily); //daily
//		}

		UpdateStat("daily", score); 

			PlayFabClientAPI.UpdateUserData (new UpdateUserDataRequest () {
				Data = new Dictionary<string, string> () {
					{ "Highscore", high.ToString () }
					//{ "Dailyscore", daily.ToString () },
					//{ "Bombs", _b.ToString() }
				}
			}, 
				result => Debug.Log ("Successfully updated user data"),
				error => {
					Debug.Log ("Got error setting user data");
					Debug.Log (error.GenerateErrorReport ());
				});
		
		_GetUserData ();
}
	



//	public int getTotalBombs()
//	{ 
//		int bombs;
//
//		try {
//			bombs = int.Parse (_result.Data ["Bombs"].Value);
//		} catch { 
//			Debug.Log ("Player needs more bombs so giving 100");
//			bombs = 100;
//		}
//
//		try {
//			if (_result.Data ["bombs"].Value == "0") {
//				Debug.Log ("Player needs more bombs");
//			}
//		} catch {
//		}
//		return bombs;
//	}


	public void _GetUserData() {
		PlayFabClientAPI.GetUserData(new GetUserDataRequest() {
			PlayFabId = PlayFabUserId,
			Keys = null
		}, result => {

			if (result.Data == null) {
				Debug.Log("No data found");
			}
			else{
				_result = result;
				IsPlayerDataDone = true;
//
//				//check if player is new
//				try
//				{
//					PlayerBombs = int.Parse (_result.Data ["Bombs"].Value.ToString());
//					Debug.Log("Player Not new");
//
//				}catch{
//					Debug.Log("New player for sure");
//					NewPlayerSetup();
//				}
//				//done checking
			} 
		}, (error) => {
			_result = null;
			IsPlayerDataDone = true;
			Debug.Log("Got error retrieving user data:");
			Debug.Log(error.GenerateErrorReport());
		});

	}

//
//			void NewPlayerSetup()
//	{
//			
//				PlayFabClientAPI.UpdateUserData (new UpdateUserDataRequest () {
//					Data = new Dictionary<string, string> () {
//						{ "Highscore", "0" },
//						{ "Dailyscore", "0" }
//					}
//				}, 
//					result => Debug.Log ("Successfully updated user data"),
//					error => {
//						Debug.Log ("Got error setting user data");
//						Debug.Log (error.GenerateErrorReport ());
//					});
//		WelcomeMessage.SetActive (true);
//		_GetUserData ();
//	}
		
}

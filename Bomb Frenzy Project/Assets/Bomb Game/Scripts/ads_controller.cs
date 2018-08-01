using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class ads_controller : MonoBehaviour {
	public Button ads_Button;
	private GameTimerController gt;
private string placementId = "rewardedVideo";


    void Update ()
    {
		if (ads_Button)
			ads_Button.interactable = Advertisement.IsReady (placementId);
    }
	
	
  public void ShowAd ()
    {
        ShowOptions options = new ShowOptions();
        options.resultCallback = HandleShowResult;

        Advertisement.Show(placementId, options);
    }
	
	
	
	
	   void HandleShowResult (ShowResult result)
    {
        if(result == ShowResult.Finished) {
        Debug.Log("Video completed - Offer a reward to the player");

			gt = GameObject.Find ("TimerController").GetComponent<GameTimerController> ();
			gt.setBombleft (50);
			this.gameObject.SetActive (false);

        }else if(result == ShowResult.Skipped) {
            Debug.LogWarning("Video was skipped - Do NOT reward the player");

        }else if(result == ShowResult.Failed) {
            Debug.LogError("Video failed to show");
        }
    }

}

using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using SimpleJSON;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager singleton_;

    void Awake ()
    {
        if (singleton_ == null)
        {
            singleton_ = this;
            DontDestroyOnLoad (this.gameObject);
        } else
        {
            DestroyImmediate (this.gameObject);
        }
    }

    public void Initialize ()
    {
#if UNITY_ANDROID
        InitializeGooglePlayServices ();
#endif

#if UNITY_IOS
        Social.localUser.Authenticate (ProcessAuthentication);
#endif
    }


#if UNITY_IOS
    void ProcessAuthentication (bool success) {
		if (success) {
			Debug.Log ("Authenticated, checking achievements");
			 int posOnleaderoard = SaveSystemManager.GetHighesScore ();
              Social.ReportScore (posOnleaderoard, Utilities.iOSLeaderabordString, (bool successPosted) => {
            // handle success or failure
        });

        Social.ShowLeaderboardUI ();
			// Request loaded achievements, and register a callback for processing them
			//Social.LoadAchievements (ProcessLoadedAchievements);
		}
		else
			Debug.Log ("Failed to authenticate");
	}
	
	// This function gets called when the LoadAchievement call completes
	void ProcessLoadedAchievements (IAchievement[] achievements) {
		if (achievements.Length == 0)
			Debug.Log ("Error: no achievements found");
		else
			Debug.Log ("Got " + achievements.Length + " achievements");
	}
#endif

    void InitializeGooglePlayServices ()
    {
        Debug.Log ("Trying to initialize google play");
        //Activate google play first and enable saved games.

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder ()
        .Build ();

        PlayGamesPlatform.InitializeInstance (config);
        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate ();

        //Now login
        Social.localUser.Authenticate ((bool succes) =>
        {
            if (succes)
            {
                //If authentificated then try to load data from cloud.
                Debug.Log ("Succesfull authentification");

                int posOnleaderoard = SaveSystemManager.GetHighesScore ();
                Social.ReportScore (posOnleaderoard, GPS_Resources.leaderboard_kind_war_leaderboard, (bool succesPosted) => {
                    if (succesPosted) Debug.Log ("Posted on leaderboard");
                });

                Social.ShowLeaderboardUI ();
            }
            else
            {
                //Else just load the next scene
                Debug.LogWarning ("Failed to authentificate");
            }
        });
    }

    public void PostAndShowLeaderboard ()
    {
        Initialize ();

        int posOnleaderoard = SaveSystemManager.GetHighesScore ();

#if UNITY_ANDROID
        Social.ReportScore (posOnleaderoard, GPS_Resources.leaderboard_kind_war_leaderboard, (bool succes) => {
            if (succes) Debug.Log ("Posted on leaderboard");
        });
#endif

#if UNITY_IOS
         Social.ReportScore (posOnleaderoard, Utilities.iOSLeaderabordString, (bool success) => {
            // handle success or failure
        });
#endif
        Social.ShowLeaderboardUI ();
    }
}

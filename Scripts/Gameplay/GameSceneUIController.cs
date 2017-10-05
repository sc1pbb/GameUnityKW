using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.IO;
using System.Runtime.InteropServices;

public class GameSceneUIController : MonoBehaviour
{
    public static GameSceneUIController singleton_;
    [SerializeField] private RectTransform mainGamePanel_;
    [SerializeField] private Text scoreTextUI_;
    [SerializeField] private Image planetHealthImage_;

    [SerializeField] private RectTransform gameOverPanel_;
    [SerializeField] private RectTransform gameOverPanelContainer_;
    [SerializeField] private Text gameOverScoreTextUI_;
    [SerializeField] private Text newHighscoreTextUI_;

    [SerializeField] private Text gameOver_totalAmountOfCoins_;

    [SerializeField] private Image fadeImageUI_;

    void Awake ()
    {
        if (singleton_ == null)
            singleton_ = this;
    }

    void Start ()
    {
        FadeOut (1f);
    }

    public void UpdatePlanetHealthUI (float value)
    {
        Sequence seq = DOTween.Sequence ();
        seq.Append (planetHealthImage_.DOColor (Color.green, 0.05f));
        seq.Append (planetHealthImage_.DOColor (Color.white, 0.05f));


        //    planetHealthImage_.fillAmount = value;
        planetHealthImage_.DOFillAmount (value, 0.2f);
    }

    public void ActivateGameOverPanel ()
    {
        mainGamePanel_.gameObject.SetActive (false);
        gameOverPanel_.gameObject.SetActive (true);
        gameOverPanelContainer_.transform.localScale = Vector3.zero;
        gameOverPanelContainer_.transform.DOScale (1f, 1f).SetEase (Ease.OutBounce).SetUpdate (UpdateType.Normal, true);
        UpdateGameOverTotalAmountsOfCoins ();
    }

    public void UpdateGameOverTotalAmountsOfCoins ()
    {
        gameOver_totalAmountOfCoins_.text = SaveSystemManager.GetCoinsNumber ().ToString ();
    }

    public void UpdateScoreTextUI (int value, bool playAnim = true)
    {
        scoreTextUI_.text = value.ToString ();
        if (playAnim)
        {
            Sequence seq = DOTween.Sequence ();
            seq.Append (scoreTextUI_.DOFade (0.5f, 0.1f));
            seq.Append (scoreTextUI_.DOFade (1f, 0.1f));
        }
    }

    public void UpdateGameOverScoreTextUI (int value, bool newHighscore)
    {
        gameOverScoreTextUI_.text = value.ToString ();
       // gameOverScoreTextUI_.DOText (value.ToString (), 3f, true, ScrambleMode.Numerals).SetUpdate (UpdateType.Normal, true);
        newHighscoreTextUI_.gameObject.SetActive (newHighscore);

        if (newHighscore)
        {
            newHighscoreTextUI_.gameObject.transform.localScale = Vector3.zero;
            newHighscoreTextUI_.transform.DOScale (Vector3.one, 1f).SetEase (Ease.OutBounce).SetUpdate (UpdateType.Normal, true);
        }
    }

    #region UI
    public void UIButton_Home ()
    {
        FadeIn (1f, Utilities.SCENE_MENUSCENE);
        //Application.LoadLevel (Application.loadedLevel);
        if (SoundManager.singleton_ != null)
        {
            SoundManager.singleton_.PlaySoundFX (SoundManager.singleton_.clickedButtonSound_);
        }
    }

    public void UIButton_CompanyLogo ()
    {
        Application.OpenURL (Utilities.COMPANY_URL);
        if (SoundManager.singleton_ != null)
        {
            SoundManager.singleton_.PlaySoundFX (SoundManager.singleton_.clickedButtonSound_);
        }
    }

    public void UIButton_RateButton ()
    {
#if UNITY_ANDROID
        Application.OpenURL (Utilities.STORE_GAME_URL_ANDROID);
#endif

#if UNITY_IOS
        Application.OpenURL (Utilities.STORE_GAME_URL_IOS);
#endif
        if (SoundManager.singleton_ != null)
        {
            SoundManager.singleton_.PlaySoundFX (SoundManager.singleton_.clickedButtonSound_);
        }
    }

    public void UIButton_ShareButton ()
    {
        ShareImage ();
#if UNITY_ANDROID
     //   string facebookshare = "https://www.facebook.com/sharer/sharer.php?u=" + System.Uri.EscapeUriString (Utilities.STORE_GAME_URL_ANDROID);
     //   Application.OpenURL (facebookshare);
#endif

#if UNITY_IOS
      //  string facebookshare = "https://www.facebook.com/sharer/sharer.php?u=" + System.Uri.EscapeUriString (Utilities.STORE_GAME_URL_IOS);
      //  Application.OpenURL (facebookshare);
#endif
        if (SoundManager.singleton_ != null)
        {
            SoundManager.singleton_.PlaySoundFX (SoundManager.singleton_.clickedButtonSound_);
        }
    }

    public void UIButton_RetryButton ()
    {
        SaveSystemManager.UpdatePlayerData (0, 1, 0);
        FadeIn (1f, SceneManager.GetActiveScene ().name);
        if (SoundManager.singleton_ != null)
        {
            SoundManager.singleton_.PlaySoundFX (SoundManager.singleton_.clickedButtonSound_);
        }
    }

    public void UIButton_EliminateAds ()
    {
        IAPManager.singleton_.Buy_REMOVE_ADS ();
        if (SoundManager.singleton_ != null)
        {
            SoundManager.singleton_.PlaySoundFX (SoundManager.singleton_.clickedButtonSound_);
        }
    }

    public void UIButton_WatchAd ()
    {
        //AdsManager.singleton_.ShowRewardedAd ();
        if (SoundManager.singleton_ != null)
        {
            SoundManager.singleton_.PlaySoundFX (SoundManager.singleton_.clickedButtonSound_);
        }
    }
#endregion

    void Update ()
    {
        if (Input.GetKeyDown (KeyCode.L))
        {
            ShakeCoinImage ();
            ShakeHeartImage ();
        }
    }

    public void ShakeHeartImage ()
    {
        Transform heartIamge = planetHealthImage_.transform.parent.GetChild (planetHealthImage_.transform.parent.childCount - 1);

        heartIamge.DOShakeScale (0.5f, 1f, 15, 90);
    }

    public void ShakeCoinImage ()
    {
        Transform coinIamge = scoreTextUI_.transform.parent.GetChild (scoreTextUI_.transform.parent.childCount - 1);

        coinIamge.DOShakeScale (0.5f, 1f, 15, 90);
    }

    private bool isProcessing = false;

    private string shareText = "Can you do more than that ?\n";
    private string gameLink = "Download the game on play store at " + "\nhttps://play.google.com/store/apps/details?id=com.aporte.kindwar&pcampaignid=GPC_shareGame";
    private string subject = "Awesome game !";
    private string imageName = "UnityScreenshot"; // without the extension, for iinstance, MyPic 

    public void ShareImage ()
    {
        isProcessing = false;
    //    Application.CaptureScreenshot (imageName, 4);// +".png", 4);
        if (!isProcessing)
        {
            StopCoroutine (ShareScreenshot ());
            StartCoroutine (ShareScreenshot ());
        }

    }
    
    private IEnumerator ShareScreenshot ()
    {
#if UNITY_ANDROID
        isProcessing = true;
        yield return new WaitForEndOfFrame ();

        Texture2D screenTexture = new Texture2D (Screen.width, Screen.height, TextureFormat.RGB24, true);
        screenTexture.ReadPixels (new Rect (0f, 0f, Screen.width, Screen.height), 0, 0);
        screenTexture.Apply ();

        Debug.Log ("IMAGE: " + Application.persistentDataPath + imageName);
        //    byte[] dataToSave = Resources.Load<TextAsset> (Application.persistentDataPath + "/"+imageName).bytes;
        byte[] dataToSave = screenTexture.EncodeToJPG ();
        string destination = Path.Combine (Application.persistentDataPath, System.DateTime.Now.ToString ("yyyy-MM-dd-HHmmss") + ".png");
        Debug.Log (destination);
        File.WriteAllBytes (destination, dataToSave);

        if (!Application.isEditor)
        {

            AndroidJavaClass intentClass = new AndroidJavaClass ("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject ("android.content.Intent");
            intentObject.Call<AndroidJavaObject> ("setAction", intentClass.GetStatic<string> ("ACTION_SEND"));
            AndroidJavaClass uriClass = new AndroidJavaClass ("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject> ("parse", "file://" + destination);
            intentObject.Call<AndroidJavaObject> ("putExtra", intentClass.GetStatic<string> ("EXTRA_STREAM"), uriObject);
            intentObject.Call<AndroidJavaObject> ("putExtra", intentClass.GetStatic<string> ("EXTRA_TEXT"), shareText + gameLink);
            intentObject.Call<AndroidJavaObject> ("putExtra", intentClass.GetStatic<string> ("EXTRA_SUBJECT"), subject);
            intentObject.Call<AndroidJavaObject> ("setType", "image/jpeg");
            AndroidJavaClass unity = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject> ("currentActivity");

            currentActivity.Call ("startActivity", intentObject);

        }
        isProcessing = false;
#endif


#if UNITY_IOS
        yield return new WaitForEndOfFrame ();
        isProcessing = true;
        string destination = Path.Combine (Application.persistentDataPath, System.DateTime.Now.ToString ("yyyy-MM-dd-HHmmss") + ".png");
        Debug.Log (destination);
        CallSocialShareAdvanced (shareText, subject, gameLink, destination);
         isProcessing = false;
#endif
    }
#if UNITY_IOS
     public struct ConfigStruct
     {
         public string title;
         public string message;
     }
 
     [DllImport ("__Internal")] private static extern void showAlertMessage(ref ConfigStruct conf);
     
     public struct SocialSharingStruct
     {
         public string text;
         public string url;
         public string image;
         public string subject;
     }
     
     [DllImport ("__Internal")] private static extern void showSocialSharing(ref SocialSharingStruct conf);
     
     public static void CallSocialShare(string title, string message)
     {
         ConfigStruct conf = new ConfigStruct();
         conf.title  = title;
         conf.message = message;
         showAlertMessage(ref conf);
     }
 
     public static void CallSocialShareAdvanced(string defaultTxt, string subject, string url, string img)
     {
         SocialSharingStruct conf = new SocialSharingStruct();
         conf.text = defaultTxt; 
         conf.url = url;
         conf.image = img;
         conf.subject = subject;
         
         showSocialSharing(ref conf);
     }
#endif

    public void FadeOut (float duration)
    {
        fadeImageUI_.color = new Color (1f, 1f, 1f, 1f);
        fadeImageUI_.transform.SetAsLastSibling ();
        fadeImageUI_.gameObject.SetActive (true);
        fadeImageUI_.DOFade (0f, duration).SetUpdate (UpdateType.Normal, true).OnComplete (() => { fadeImageUI_.gameObject.SetActive (false); });
    }

    public void FadeIn (float duration, string sceneToLoad)
    {
        fadeImageUI_.color = new Color (1f, 1f, 1f, 0f);
        fadeImageUI_.transform.SetAsLastSibling ();
        fadeImageUI_.gameObject.SetActive (true);
        fadeImageUI_.DOFade (1f, duration).SetUpdate (UpdateType.Normal, true).OnComplete (() => { Debug.LogError ("AAAA");  SceneManager.LoadScene (sceneToLoad); });
    }
}

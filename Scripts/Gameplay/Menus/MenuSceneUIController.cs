using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuSceneUIController : MonoBehaviour
{
    public static MenuSceneUIController singleton_ { get; private set; }

    [Header ("Canvas reference")]
    [SerializeField] private RectTransform canvasReference_;

    [Header ("Main menu panel")]
    [SerializeField] private RectTransform mainMenuPanel_;
    [SerializeField] private Text coinsText_;

    [Header ("Shop panel")]
    [SerializeField] private RectTransform shopPanel_;
    [SerializeField] private RectTransform shopScrollRect_;
    [SerializeField] private ShopUIElement shopUIElementPrefab_;
    [SerializeField] private Sprite lockedPlanetSprite_;

    [Header ("Settings Panel")]
    [SerializeField] private RectTransform settingsPanel_;
    [SerializeField] private Button soundButtonToggle_;
    [SerializeField] private Sprite unmutedSprite_;
    [SerializeField] private Sprite mutedSprite_;
    [SerializeField] private Toggle soundToggle_;

    [Header ("Loading screen")]
    [SerializeField] private RectTransform loadingScreenPanel_;
    [SerializeField] private Text hintText_;
    [SerializeField] private Image loadingBarUI_;

    [Header ("Get more coins panel")]
    [SerializeField] private RectTransform getMoreCoinsPanel_;

    [Header ("Not enough coins pop up")]
    [SerializeField] private RectTransform notEnoughCoinsPopUp_;

    [Header ("Fade")]
    [SerializeField] private Image fadeImageUI_;

    [Header ("Pop Up")]
    public PopUpUI popUp_;

    //Current panel
    private RectTransform currentPanel_;

    void Awake ()
    {
        if (singleton_ == null)
            singleton_ = this;
    }

    void Start ()
    {
        Time.timeScale = 1;
        currentPanel_ = mainMenuPanel_;
        FadeOut (1f);
    }

    public void UpdateCoinsText (int value)
    {
        coinsText_.text = value.ToString ();
    }

    #region SETTINGS
    public void SetSoundToggleSprite (int state)
    {
        //soundToggle_.isOn = state == 1 ? true : false;
        Debug.Log ("SET SOUND TOGGLE CALLED. STATE: " + state);
        if (state == 1)
        {
            //Sound is on.
            soundButtonToggle_.GetComponent<Image> ().sprite = unmutedSprite_; 
        }
        else
        {
            //Sound is off
            soundButtonToggle_.GetComponent<Image> ().sprite = mutedSprite_;
        }
    }
    #endregion

    #region UI_BUTTONS
    public void UIButton_PlayButton ()
    {
        //Add one game played
        if (SaveSystemManager.GetGamesPlayed () == 0)
        {
            SaveSystemManager.UpdatePlayerData (0, 1, 0);
            FadeIn (1f, "TutorialScene");
        }
        else
        {
            SaveSystemManager.UpdatePlayerData (0, 1, 0);
            //   MenuSceneManager.singleton_.LoadScene (ShopManager.singleton_.usingPlanet_.sceneString_);
            FadeIn (1f, ShopManager.singleton_.usingPlanet_.sceneString_);
        }
    }

    public void UIButton_SettingsButton ()
    {
        ActivatePanel (settingsPanel_, 1f);
    }

    public void UIButton_GetMoreCoinsButton ()
    {
        getMoreCoinsPanel_.transform.localScale = Vector3.zero;
        getMoreCoinsPanel_.gameObject.SetActive (true);
        getMoreCoinsPanel_.DOScale (Vector3.one, 0.5f).SetEase (Ease.OutBounce);
    }

    public void UIButton_QuitMoreCoinsButton ()
    {
        getMoreCoinsPanel_.DOScale (Vector3.zero, 0.5f).SetEase (Ease.OutBounce).OnComplete ( () => {getMoreCoinsPanel_.gameObject.SetActive (false); });
    }

    public void UIButton_RestorePurchases ()
    {
        if (IAPManager.singleton_ != null)
        {
            IAPManager.singleton_.RestorePurchases ();
        }
    }

    public void UIButton_VisitUSButton ()
    {
        Application.OpenURL (Utilities.COMPANY_URL);
    }

    public void UIButton_WatchAd ()
    {
        //AdsManager.singleton_.ShowRewardedAd ();
        if (SoundManager.singleton_ != null)
        {
            SoundManager.singleton_.PlaySoundFX (SoundManager.singleton_.clickedButtonSound_);
        }
    }

    public void UIButton_RateGameButton ()
    {
#if UNITY_ANDROID
        Application.OpenURL (Utilities.STORE_GAME_URL_ANDROID);
#endif

#if UNITY_IOS
        Application.OpenURL (Utilities.STORE_GAME_URL_IOS);
#endif
    }

    public void UIButton_SoundToggle ()
    {
        if (SoundManager.singleton_ != null)
        {
            SoundManager.singleton_.UpdateSoundState (!SoundManager.singleton_.soundOn_);
            //  SetSoundToggle (SoundManager.singleton_.soundOn_ == true ? 1: 0);
            SetSoundToggleSprite (SaveSystemManager.GetSoundSettings ());
        }
    }

    public void UIButton_ShopPanel ()
    {
        ActivateShopPanel (shopPanel_, 1f);
    }

    public void UIButton_BackToMainPanel ()
    {
        DeActivateShopPanel (shopPanel_, 1f);
    }

    public void UIButton_ExitButton ()
    {
        Application.Quit ();
    }

    public void UIButton_ClosePopUp ()
    {
        notEnoughCoinsPopUp_.DOScale (0f, 1f).SetEase (Ease.OutBounce).OnComplete (() => { notEnoughCoinsPopUp_.gameObject.SetActive (true); });
    }

    public void UIButton_ShowLeaderboard ()
    {
        if (LeaderboardManager.singleton_ != null)
            LeaderboardManager.singleton_.PostAndShowLeaderboard ();
        else
            Debug.LogWarning ("Didn't find leadearboard manager");
    }

    public void UIButton_PlaySoundEffect ()
    {
        if (SoundManager.singleton_ != null)
            SoundManager.singleton_.PlaySoundFX (SoundManager.singleton_.clickedButtonSound_);
        else
            Debug.LogWarning ("Couldn't find the Sound Manager");
    }
    #endregion

    #region SHOP
    public void UpdateShopUI (bool initializeFirst = false)
    {
        for (int i=0; i<ShopManager.singleton_.shopitemList_.Count; i++)
        {
            ShopItem currentShopItem = ShopManager.singleton_.shopitemList_[i];

            if (initializeFirst)
                currentShopItem.itemShopUIElement_ = (ShopUIElement)Instantiate (shopUIElementPrefab_, Vector3.zero, Quaternion. identity);

            currentShopItem.itemShopUIElement_.transform.SetParent (shopScrollRect_.transform, false);

            if (currentShopItem.bought_ == false && currentShopItem.using_ == false)
                currentShopItem.itemShopUIElement_.UpdateShopUIElement (currentShopItem.bought_, currentShopItem.using_, currentShopItem.shopIcon_, currentShopItem.price_, i, ShopManager.singleton_.BuyNewPlanet_Button);

            if (currentShopItem.bought_ == true)
                currentShopItem.itemShopUIElement_.UpdateShopUIElement (currentShopItem.bought_, currentShopItem.using_, currentShopItem.shopIcon_, currentShopItem.price_, i, ShopManager.singleton_.UseNewPlanet_Button);

            if (currentShopItem.using_ == true)
                currentShopItem.itemShopUIElement_.UpdateShopUIElement (currentShopItem.bought_, currentShopItem.using_, currentShopItem.shopIcon_, currentShopItem.price_, i);
        }
    }

    public void ActivateNotEnoughtCoinsPopUp ()
    {
        notEnoughCoinsPopUp_.gameObject.SetActive (true);
        notEnoughCoinsPopUp_.transform.localScale = Vector3.zero;
        notEnoughCoinsPopUp_.transform.DOScale (1f, 1f).SetEase (Ease.OutBounce);
    }
    #endregion

    #region Loading SCREEN
    public void ActivateLoadingscreenPanelUI (string hint)
    {
        loadingScreenPanel_.gameObject.SetActive (true);
        loadingBarUI_.fillAmount = 0;
        loadingScreenPanel_.GetComponent<Image> ().color = new Color (1f, 1f, 1f, 0f);
        hintText_.text = hint;
        loadingScreenPanel_.GetComponent<Image> ().DOFade (1f, 0.5f);
    }

    public void UpdateLoadingbarBarUI (float value)
    {
        loadingBarUI_.fillAmount = value;
    }
    #endregion

    public void ActivatePanel (RectTransform nextPanel, float duration)
    {
        Vector2 currentPanelNormalPosition = currentPanel_.anchoredPosition;
        Vector2 currentPanelGoToPosition = new Vector2 (currentPanel_.sizeDelta.x, currentPanel_.anchoredPosition.y);
        Vector2 nextPanelGoToPosition = nextPanel.anchoredPosition;
        nextPanel.anchoredPosition = new Vector2 (nextPanel.sizeDelta.x, nextPanel.anchoredPosition.y);
        nextPanel.gameObject.SetActive (true);

        Sequence menuAnimSeq = DOTween.Sequence ();
        menuAnimSeq.Append (currentPanel_.DOAnchorPos (currentPanelGoToPosition, duration/2));
        menuAnimSeq.Append (nextPanel.DOAnchorPos (nextPanelGoToPosition, duration / 2).OnComplete ( () => {
            currentPanel_.anchoredPosition = currentPanelNormalPosition;
            currentPanel_.gameObject.SetActive (false);
            currentPanel_ = nextPanel;
        }));
    }

    public void ActivateShopPanel (RectTransform nextPanel, float duration)
    {
        Vector2 nextPanelGoToPosition = Vector2.zero;
        nextPanel.anchoredPosition = new Vector2 (nextPanel.sizeDelta.x, nextPanel.anchoredPosition.y);
        nextPanel.gameObject.SetActive (true);

        Sequence menuAnimSeq = DOTween.Sequence ();

       nextPanel.DOAnchorPos (nextPanelGoToPosition, duration);
    }

    public void DeActivateShopPanel (RectTransform currentPanel, float duration)
    {
        Debug.Log ("Deactivate shop panel");
        Vector2 currentPanelGoToPosition = new Vector2 (currentPanel.sizeDelta.x, currentPanel.anchoredPosition.y);
        currentPanel.DOAnchorPos (currentPanelGoToPosition, duration).OnComplete (() => { currentPanel.gameObject.SetActive (false); });
    }

    public void FadeOut (float duration)
    {
        fadeImageUI_.color = new Color (1f, 1f, 1f, 1f);
        fadeImageUI_.transform.SetAsLastSibling ();
        fadeImageUI_.gameObject.SetActive (true);
        fadeImageUI_.DOFade (0f, duration).OnComplete (() => { fadeImageUI_.gameObject.SetActive (false); });
    }

    public void FadeIn (float duration, string sceneToLoad)
    {
        fadeImageUI_.color = new Color (1f, 1f, 1f, 0f);
        fadeImageUI_.transform.SetAsLastSibling ();
        fadeImageUI_.gameObject.SetActive (true);
        fadeImageUI_.DOFade (1f, duration).OnComplete (() => { SceneManager.LoadScene (sceneToLoad); });
    }
}

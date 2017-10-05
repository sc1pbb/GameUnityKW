using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MenuSceneManager : MonoBehaviour
{
    public static MenuSceneManager singleton_ { get; private set; }

    [SerializeField] private List <string> hintsString_ = new List<string>();

    public int coins_;

    void Awake ()
    {
        if (singleton_ == null)
            singleton_ = this;
    }

    void Start ()
    {
        //Update sound settings
        coins_ = SaveSystemManager.GetCoinsNumber ();

        MenuSceneUIController.singleton_.UpdateCoinsText (coins_);
        MenuSceneUIController.singleton_.SetSoundToggleSprite (SaveSystemManager.GetSoundSettings ());

        if (SoundManager.singleton_ != null)
        {
            SoundManager.singleton_.PlayBackgroundTheme (SoundManager.singleton_.menuBackgroundSound_);
        }
    }

    public void LoadScene (string sceneName)
    {
        //I will probably call menuscene manager
        StartCoroutine (LoadLevelSync (sceneName));
    }

    IEnumerator LoadLevelSync (string sceneName)
    {
        MenuSceneUIController.singleton_.ActivateLoadingscreenPanelUI ("");// hintsString_[(int) Random.Range (0, hintsString_.Count)]);

        yield return new WaitForSeconds (2f);

        AsyncOperation asyncScene;
        asyncScene = SceneManager.LoadSceneAsync (sceneName);
        

        while (!asyncScene.isDone)
        {
       //     MenuSceneUIController.singleton_.UpdateLoadingbarBarUI (asyncScene.progress / 0.9f);
            yield return null;
        }
    }

    public void AddCoins (int coinsValue)
    {
        coins_ += coinsValue;
        MenuSceneUIController.singleton_.UpdateCoinsText (coins_);
        SaveSystemManager.UpdatePlayerData (coinsValue);
    }
}

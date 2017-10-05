using UnityEngine;
using System.Collections;

public class InitializeManager : MonoBehaviour
{
    void Awake ()
    {
        if (PlayerPrefs.HasKey (Utilities.PP_SOUND_SETTINGS) == false)
            SaveSystemManager.InitializeSoundSettings ();

        if (PlayerPrefs.HasKey (Utilities.PP_PLAYER_DATA) == false)
            SaveSystemManager.InitializePlayerData ();

        if (PlayerPrefs.HasKey (Utilities.PP_SHOP_DATA) == false)
            SaveSystemManager.InitializeShopData ();
    }
}

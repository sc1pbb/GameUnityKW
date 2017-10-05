using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;

public static class SaveSystemManager
{
    public static void InitializeSoundSettings ()
    {
        PlayerPrefs.SetInt (Utilities.PP_SOUND_SETTINGS, 1);
        Debug.Log ("Initialized sound settings: " + PlayerPrefs.GetInt (Utilities.PP_SOUND_SETTINGS));
    }

    public static void SetSoundSettings (int sound)
    {
        PlayerPrefs.SetInt (Utilities.PP_SOUND_SETTINGS, sound);
        Debug.Log ("Saved sound settings: " + PlayerPrefs.GetInt (Utilities.PP_SOUND_SETTINGS));
    }

    public static int GetSoundSettings ()
    {
        return PlayerPrefs.GetInt (Utilities.PP_SOUND_SETTINGS);
    }

    public static void InitializePlayerData ()
    {
        JSONNode playerDataJSON = JSON.Parse ("{}");

        playerDataJSON["PlayerData"]["Coins"].AsInt = 0;
        playerDataJSON["PlayerData"]["GamesPlayed"].AsInt = 0;
        playerDataJSON["PlayerData"]["VideosWatched"].AsInt = 0;
        playerDataJSON["PlayerData"]["Highscore"].AsInt = 0;
        playerDataJSON["PlayerData"]["PaidForNoAds"].AsBool = false;

        PlayerPrefs.SetString (Utilities.PP_PLAYER_DATA, playerDataJSON.ToJSON (1));
        Debug.LogError ("Initialized player data: " + PlayerPrefs.GetString (Utilities.PP_PLAYER_DATA));
    }

    public static void PaidForNoAds ()
    {
        JSONNode playerDataJSON = JSON.Parse ("{}");
        playerDataJSON["PlayerData"]["PaidForNoAds"].AsBool = true;

        PlayerPrefs.SetString (Utilities.PP_PLAYER_DATA, playerDataJSON.ToJSON (1));
        Debug.Log ("Initialized player data: " + PlayerPrefs.GetString (Utilities.PP_PLAYER_DATA));
    }

    public static bool GetIfPaidForAds ()
    {
        JSONNode playerDataJSON = JSON.Parse (PlayerPrefs.GetString (Utilities.PP_PLAYER_DATA));
        return playerDataJSON["PlayerData"]["PaidForNoAds"].AsBool;
    }

    public static int GetHighesScore ()
    {
        JSONNode playerDataJSON = JSON.Parse (PlayerPrefs.GetString (Utilities.PP_PLAYER_DATA));
        return playerDataJSON["PlayerData"]["Highscore"].AsInt;
    }

    public static void SetNewHighscore (int newhighscore)
    {
        JSONNode playerDataJSON = JSON.Parse ("{}");
        playerDataJSON["PlayerData"]["Highscore"].AsInt = newhighscore;
        PlayerPrefs.SetString (Utilities.PP_PLAYER_DATA, playerDataJSON.ToJSON (1));
        Debug.Log ("Set a new highscore in player data: " + PlayerPrefs.GetString (Utilities.PP_PLAYER_DATA));
    }

    public static void UpdatePlayerData (int coinsToAdd = 0, int gamesPlayedToAdd = 0, int videosWatched = 0)
    {
        JSONNode playerDataJSON = JSON.Parse (PlayerPrefs.GetString (Utilities.PP_PLAYER_DATA));

        if (coinsToAdd > 0)
        {
            int numberOfCoins = playerDataJSON["PlayerData"]["Coins"].AsInt + coinsToAdd;
            playerDataJSON["PlayerData"]["Coins"].AsInt = numberOfCoins;
        }

        if (gamesPlayedToAdd > 0)
        {
            int numberOfGamesPlayed = playerDataJSON["PlayerData"]["GamesPlayed"].AsInt + gamesPlayedToAdd;
            playerDataJSON["PlayerData"]["GamesPlayed"].AsInt = numberOfGamesPlayed;
        }

        if (videosWatched > 0)
        {
            int numberOfVideosWatched = playerDataJSON["PlayerData"]["VideosWatched"].AsInt + videosWatched;
            playerDataJSON["PlayerData"]["VideosWatched"].AsInt = numberOfVideosWatched;
        }

        PlayerPrefs.SetString (Utilities.PP_PLAYER_DATA, playerDataJSON.ToJSON (1));
        Debug.LogError ("Updated player data: " + PlayerPrefs.GetString (Utilities.PP_PLAYER_DATA));
    }

    public static int GetCoinsNumber ()
    {
        JSONNode playerDataJSON = JSON.Parse (PlayerPrefs.GetString (Utilities.PP_PLAYER_DATA));
        return playerDataJSON["PlayerData"]["Coins"].AsInt;
    }

    public static int GetGamesPlayed ()
    {
        JSONNode playerDataJSON = JSON.Parse (PlayerPrefs.GetString (Utilities.PP_PLAYER_DATA));
        return playerDataJSON["PlayerData"]["GamesPlayed"].AsInt;
    }

    public static int GetVideosWatched ()
    {
        JSONNode playerDataJSON = JSON.Parse (PlayerPrefs.GetString (Utilities.PP_PLAYER_DATA));
        return playerDataJSON["PlayerData"]["VideosWatched"].AsInt;
    }

    public static void InitializeShopData ()
    {
        JSONNode shopJSON = JSON.Parse ("{}");

        //The default skin is bought
        int firstPlanet = 0;
        shopJSON["UsingPlanetIndex"].AsInt = firstPlanet;
        shopJSON["NumberOfPlanetsBought"].AsInt = 1;
        shopJSON["PlanetBought [" + firstPlanet.ToString () + "]"]["PlanetIndex"].AsInt = firstPlanet;

        PlayerPrefs.SetString (Utilities.PP_SHOP_DATA, shopJSON.ToJSON (1));
        Debug.Log ("Initialized Shop data: " + PlayerPrefs.GetString (Utilities.PP_SHOP_DATA));
    }

    public static void AddBoughtPlanet (int indexOfPlanet)
    {
        JSONNode shopJSON = JSON.Parse (PlayerPrefs.GetString (Utilities.PP_SHOP_DATA));

        shopJSON["PlanetBought [" + shopJSON["NumberOfPlanetsBought"].AsInt.ToString () + "]"]["PlanetIndex"].AsInt = indexOfPlanet;
        shopJSON["NumberOfPlanetsBought"].AsInt = shopJSON["NumberOfPlanetsBought"].AsInt + 1;
       

        PlayerPrefs.SetString (Utilities.PP_SHOP_DATA, shopJSON.ToJSON (1));
        Debug.Log ("Saved shop data: " + PlayerPrefs.GetString (Utilities.PP_SHOP_DATA));
    }

    public static void SetUsingPlanet (int planetIndex)
    {
        JSONNode shopJSON = JSON.Parse (PlayerPrefs.GetString (Utilities.PP_SHOP_DATA));
        shopJSON["UsingPlanetIndex"].AsInt = planetIndex;
        PlayerPrefs.SetString (Utilities.PP_SHOP_DATA, shopJSON.ToJSON (1));
        Debug.Log ("Saved shop data: " + PlayerPrefs.GetString (Utilities.PP_SHOP_DATA));
    }

    public static int LoadUsingPlanet ()
    {
        JSONNode shopJSON = JSON.Parse (PlayerPrefs.GetString (Utilities.PP_SHOP_DATA));
        return shopJSON["UsingPlanetIndex"].AsInt;
    }

    public static List<int> LoadShopData ()
    {
        JSONNode shopJSON = JSON.Parse (PlayerPrefs.GetString (Utilities.PP_SHOP_DATA));

        List<int> boughtPlanets = new List<int> ();

        int boughtPlanetsNumber = shopJSON["NumberOfPlanetsBought"].AsInt;

        for (int i=0; i< boughtPlanetsNumber; i++)
        {
            boughtPlanets.Add (shopJSON["PlanetBought [" + i.ToString () + "]"]["PlanetIndex"].AsInt);
        }

        return boughtPlanets;
    }
}

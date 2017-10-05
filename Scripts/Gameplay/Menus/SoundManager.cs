using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour 
{
	//For all managers we will use a singleton.
	public static SoundManager singleton_;

    [HideInInspector] public bool soundOn_;
    
    [SerializeField] private float maxVolume_ = 1;
    [SerializeField] private float sfxMaxVolume_ = 0.1f;

    [Header ("Audio Sources")]
	//Audio Sources
	[SerializeField] private AudioSource backgroundAudioSource_;
	[SerializeField] private AudioSource fxAudioSource_;

	[Header ("Audio clips")]
	//Audio clips. These variables are public because all the other functions will call PlaySingle (Audioclip)
	public AudioClip menuBackgroundSound_;
    public AudioClip battleSound_;
    public AudioClip clickedButtonSound_;
    public AudioClip soldierAttackSound_;
    public AudioClip tankAttackSound_;
    public AudioClip airplaneAttackSound_;
    public AudioClip gameOverSound_;
    public AudioClip buildingExplosionSound_;
    public AudioClip powerUpSound_;
    public AudioClip blowSound_;
    public AudioClip notEnoghMoneySound_;

    [Header ("Random pitch")]
	//I tried to randomize a bit sound effects.
	[SerializeField] public bool randomizeSoundPitch_ = true;
	[SerializeField] private float lowPitchRange_ = 0.9f;
	[SerializeField] private float highPitchRange_ = 1.1f;

	void Awake ()
	{
		if (singleton_ == null)
		{
			//If singleton doesnt exist then intialize it
			singleton_ = this;
			DontDestroyOnLoad (this.gameObject);
		} else
		{
			//Else destroy it. (This object will be present in all scenes)
			DestroyImmediate (this.gameObject);
		}
	}

    void Start ()
    {
        SetSoundState ();
    }

    public void SetSoundState ()
    {
        backgroundAudioSource_.volume = fxAudioSource_.volume = SaveSystemManager.GetSoundSettings () == 1 ? maxVolume_ : 0;
        soundOn_ = SaveSystemManager.GetSoundSettings () == 1 ? true : false;
        Debug.Log ("zzzz - " + SaveSystemManager.GetSoundSettings ());
    }

    public void UpdateSoundState (bool state)
    {
        Debug.Log ("STATE: " + state);
        SaveSystemManager.SetSoundSettings (state == true ? 1 : 0);
        soundOn_ = state;
        SetSoundState ();
    }

	/// <summary>
	/// Plaies an audioclip. Will be called from other classes
	/// </summary>
	/// <param name="audioClip">Audio clip.</param>
	/// <param name="randomizePitch">If set to <c>true</c> randomize pitch.</param>
	public void PlaySoundFX (AudioClip audioClip)
	{
		Debug.Log ("SOUND EFFECT: " + audioClip);
		fxAudioSource_.clip = audioClip;
		fxAudioSource_.loop = false; //just to be sure

		if (randomizeSoundPitch_)
		{
			float randomPitch = Random.Range (lowPitchRange_, highPitchRange_);
			fxAudioSource_.pitch = randomPitch;
		}

		fxAudioSource_.Play ();
	}

	/// <summary>
	/// Plaies the background theme.
	/// </summary>
	/// <param name="bgThemeSound">Background theme sound.</param>
	public void PlayBackgroundTheme (AudioClip bgThemeSound)
	{
		if (backgroundAudioSource_.clip != bgThemeSound)
		{
			backgroundAudioSource_.loop = true;
			backgroundAudioSource_.clip = bgThemeSound;
			backgroundAudioSource_.Play ();
		}
	}

	/// <summary>
	/// Stops the background sound.
	/// </summary>
	public void StopBackgroundSound ()
	{
		backgroundAudioSource_.clip = null;
	}

    public void SetAudioClipToSource (AudioSource audioSource, AudioClip audioClip, bool randomizePitch = false)
    {
        if (soundOn_) 
        {
            if (audioSource == null)
                audioSource.gameObject.AddComponent<AudioSource> ();
            audioSource.volume = sfxMaxVolume_;
            audioSource.clip = audioClip;
            
            if (randomizePitch)
            {
                float randomPitch = Random.Range (lowPitchRange_, highPitchRange_);
                audioSource.pitch = randomPitch;
            }

            audioSource.Play ();
        }
    }
}


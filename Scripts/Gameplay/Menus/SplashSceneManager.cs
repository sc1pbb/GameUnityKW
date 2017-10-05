using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashSceneManager : MonoBehaviour 
{
    public static SplashSceneManager singleton_;
	[SerializeField] private Image fadeImageUI_;
    [SerializeField] private Image aporteLogo_;
    [SerializeField] private Image igidLogo_;

    void Awake ()
    {
        if (singleton_ == null)
        {
            singleton_ = this;
        }
    }

	IEnumerator Start ()
	{
		if (SoundManager.singleton_ != null)
			SoundManager.singleton_.PlayBackgroundTheme (SoundManager.singleton_.menuBackgroundSound_);
		else
		    Debug.LogWarning ("Couldn't find the Sound Manager");

        igidLogo_.color = new Color (1f, 1f, 1f, 0f);

		FadeOut (1f);

        yield return new WaitForSeconds (3f);

        aporteLogo_.DOFade (0f, 1f);

        yield return new WaitForSeconds (1.2f);

        igidLogo_.DOFade (1f, 1f);

        yield return new WaitForSeconds (2f);

        FadeIn (1f, "MenuScene");
	}

	/// <summary>
	/// Fades the out.
	/// </summary>
	/// <param name="duration">Duration.</param>
	public void FadeOut (float duration)
	{
		fadeImageUI_.color = new Color (1f, 1f, 1f, 1f);
		fadeImageUI_.transform.SetAsLastSibling ();
		fadeImageUI_.gameObject.SetActive (true);
		fadeImageUI_.DOFade (0f, duration).OnComplete ( () => {fadeImageUI_.gameObject.SetActive(false);});
	}

	/// <summary>
	/// Fades the in.
	/// </summary>
	/// <param name="duration">Duration.</param>
	/// <param name="sceneToLoad">Scene to load.</param>
	public void FadeIn (float duration, string sceneToLoad)
	{
		fadeImageUI_.color = new Color (1f, 1f, 1f, 0f);
		fadeImageUI_.transform.SetAsLastSibling ();
		fadeImageUI_.gameObject.SetActive (true);
		fadeImageUI_.DOFade (1f, duration).OnComplete ( () => {SceneManager.LoadScene (sceneToLoad);});
	}
}

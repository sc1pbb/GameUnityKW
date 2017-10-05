using UnityEngine;
using System.Collections;
using DG.Tweening;

[RequireComponent (typeof (AudioSource))]
public class BonusItem : MonoBehaviour
{
    public BonusItemType bonusItemType_;
    public BonusItemSpawnPoint spawnPoint_;

    public AudioSource audioSource_;

    void Awake ()
    {
        audioSource_ = this.GetComponent<AudioSource> ();
    }

    public void ActivateBonusItem ()
    {
        this.transform.DOMove (this.transform.up * 15f, 1f).SetEase (Ease.InSine).OnComplete (() => { Destroy (this.gameObject); } );
        spawnPoint_.taken_ = false;
    }
}

public enum BonusItemType
{
    COIN,
    HP
}

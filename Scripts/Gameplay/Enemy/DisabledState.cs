using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DisabledState : IEnemyState
{
    private readonly Enemy enemy_;
    Vector3 randomRotationDirection_;

    public DisabledState (Enemy enemy)
    {
        enemy_ = enemy;
    }

    public void StartState ()
    {
        enemy_.isActive_ = false;

        if (enemy_.enableSpiralAnimation_)
        {
            Debug.LogError ("Анимация смерти");
            enemy_.transform.DOMove (-enemy_.transform.up * Random.Range (12.5f, 12.5f), 2f).SetEase (Ease.InSine).OnComplete (DisableEnemy);
        }
        else
            DisableEnemy ();
        
        GameManager.singleton_.AddCoins (enemy_.valueInCoins_);
        GameManager.singleton_.AddScore (1);
    }

    public void UpdateState ()
    {
    }

    public void ToDisableState ()
    {

    }

    public void ToEnablingState ()
    {

    }

    public void ToChaseState ()
    {

    }

    public void ToAttackState ()
    {

    }

    void DisableEnemy ()
    {
        enemy_.isActive_ = false;
        enemy_.transform.position = new Vector3 (1000, 1000, 1000);
        enemy_.gameObject.SetActive (false);
    }
}

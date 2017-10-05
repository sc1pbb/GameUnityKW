using UnityEngine;
using System.Collections;
using DG.Tweening;

public class AttackState : IEnemyState
{

    private readonly Enemy enemy_;
    private float lastTimeAttacked_;
    bool finishedLookToTargetAnimation_;


    public AttackState (Enemy enemy)
    {
        enemy_ = enemy;
    }

    public void StartState ()
    {
        Debug.Log ("Enabled Attack state");
        finishedLookToTargetAnimation_ = false;

        if (enemy_.enableLookAtTargetAnimation_)
        {
            Vector3 directionToRotate = ProjectPointOnPlane (enemy_.transform.up, enemy_.transform.position, enemy_.currentTarget_.transform.position);
            enemy_.objectToRotateToTarget_.transform.DOLookAt (directionToRotate, enemy_.rotationAnimationSpeed_, AxisConstraint.None, enemy_.transform.up).OnComplete (() =>
           {
               finishedLookToTargetAnimation_ = true;
               lastTimeAttacked_ = Time.time;
           });

            if (enemy_.isAnimated_)
            {
                enemy_.animatorController_.ResetTrigger (enemy_.runAnimationTrigger_);
                int randomAnimation = (int)Random.Range (0, 2);
                if (randomAnimation == 0)
                {
                    enemy_.animatorController_.SetTrigger (enemy_.shootAnimationTrigger_);
                }
                else
                {
                    enemy_.animatorController_.SetTrigger (enemy_.crouchShootAnimationTrigger_);
                }
              
            }
        } 
        else
        {
            finishedLookToTargetAnimation_ = true;
            lastTimeAttacked_ = Time.time;
        } 
    }

    public void UpdateState ()
    {
        switch (enemy_.enemyType_)
        {
            case EnemyType.SOLDIER:
                if (finishedLookToTargetAnimation_ && Time.time >= lastTimeAttacked_ + enemy_.cooldown_)
                {
                    Attack ();
                }
                break;

            case EnemyType.TANK:
                if (finishedLookToTargetAnimation_ && Time.time >= lastTimeAttacked_ + enemy_.cooldown_)
                {
                    Attack ();
                }
                break;

            case EnemyType.AIRPLANE:
                Attack ();
                ToChaseState ();
                break;

            default:
                if (finishedLookToTargetAnimation_ && Time.time >= lastTimeAttacked_ + enemy_.cooldown_)
                {
                    Attack ();
                }
                break;
        }
       
    }

    public void ToDisableState ()
    {

    }

    public void ToEnablingState ()
    {

    }

    public void ToChaseState ()
    {
        enemy_.ChangeState (enemy_.enemyChaseState_);
    }

    public void ToAttackState ()
    {

    }

    void Attack ()
    {
        if (enemy_.currentTarget_.isAlive_)
        {
            //Атака
            enemy_.currentTarget_.AttackBuilding (enemy_.damage_);

            //Анимация атаки
            StartAttackAnimation ();
          
            //Спавн
            LaunchProjectile ();

            //Время
            lastTimeAttacked_ = Time.time;

            //Звук
            switch (enemy_.enemyType_)
            {
                case EnemyType.AIRPLANE:
                    if (SoundManager.singleton_ != null)
                    {
                        SoundManager.singleton_.SetAudioClipToSource (enemy_.audioSource_, SoundManager.singleton_.airplaneAttackSound_, true);
                    }
                    break;

                case EnemyType.SOLDIER:
                    if (SoundManager.singleton_ != null)
                    {
                        SoundManager.singleton_.SetAudioClipToSource (enemy_.audioSource_, SoundManager.singleton_.soldierAttackSound_, true);
                    }
                    break;

                case EnemyType.TANK:
                    if (SoundManager.singleton_ != null)
                    {
                        SoundManager.singleton_.SetAudioClipToSource (enemy_.audioSource_, SoundManager.singleton_.tankAttackSound_, true);
                    }
                    break;

                default:
                    break;
            }
             
        } else
        {
            //
            ToChaseState ();
        }
    }

    void StartAttackAnimation ()
    {
        if (enemy_.enableShakeAnimation_)
            enemy_.transform.DOShakePosition (enemy_.shakeDuration_, enemy_.transform.forward * enemy_.shakeStrength_, enemy_.shakeVibrato_, enemy_.shakeRandomness_);
        if (enemy_.shotParticle_)
        {
            enemy_.shotParticle_.gameObject.SetActive (false);
            enemy_.shotParticle_.gameObject.SetActive (true);
        }
    }

    void LaunchProjectile ()
    {
        // ProjectileController newProjectile = (ProjectileController) MonoBehaviour.Instantiate (enemy_.projectile_, enemy_.projectileStartPosition_.position, Quaternion.identity);
        ProjectileController newProjectile = ParticlesPoolManager.singleton_.GetFirstProjectileAvaible (enemy_.projectileType_);
        newProjectile.transform.forward = enemy_.transform.forward;
        newProjectile.EnableProjectile (enemy_.projectileStartPosition_.position, newProjectile.transform.forward, enemy_.projectileForce_);
    }

    Vector3 ProjectPointOnPlane (Vector3 planeNormal, Vector3 planePoint, Vector3 point)
    {
        planeNormal.Normalize ();
        float distance = -Vector3.Dot (planeNormal.normalized, (point - planePoint));
        return point + planeNormal * distance;
    }
}

using UnityEngine;
using System.Collections;
using DG.Tweening;

public class EnablingState : IEnemyState
{
    private readonly Enemy enemy_;
    private Vector3 moveToPosition_;

    public EnablingState (Enemy enemy)
    {
        enemy_ = enemy;
    }

    public void StartState ()
    {
        switch (enemy_.enemyType_)
        {
            case EnemyType.AIRPLANE:
                moveToPosition_ = enemy_.enableAnimationEndPoint_;
                float agentMeshYPos = enemy_.agentMesh_.localPosition.y;
                enemy_.agentMesh_.localPosition = Vector3.zero;
                enemy_.agentMesh_.DOLocalMoveY (agentMeshYPos, 5f).SetEase (Ease.InSine);
                break;

            case EnemyType.SOLDIER:
                moveToPosition_ = enemy_.enableAnimationEndPoint_;
                if (enemy_.isAnimated_)
                {
                    enemy_.animatorController_ = enemy_.transform.GetChild (1).transform.GetComponent<Animator> ();

                    enemy_.animatorController_.ResetTrigger (enemy_.shootAnimationTrigger_);
                    enemy_.animatorController_.ResetTrigger (enemy_.crouchShootAnimationTrigger_);
                    enemy_.animatorController_.SetTrigger (enemy_.runAnimationTrigger_);
                }
                break;

            case EnemyType.TANK:
                moveToPosition_ = enemy_.enableAnimationEndPoint_;
                break;

            default:
                moveToPosition_ = enemy_.enableAnimationEndPoint_;
                break;
        }
       
        Debug.Log ("STARTED ENABLING STATE");
    }

    public void UpdateState ()
    {
        Move ();
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

    void Move ()
    {
        MoveTowards (moveToPosition_);
        float distanceToEndAnimationPoint = enemy_.sphericalGrid_.GetSphericalDistance (enemy_.transform.position, moveToPosition_);
        if (distanceToEndAnimationPoint <= 0.2f)
        {
            ToChaseState ();
        }
        
    }

    public void MoveTowards (Vector3 targetPos)
    {
        Quaternion newRot = enemy_.planetBody_.LookAtTarget (targetPos);
        enemy_.transform.rotation = Quaternion.Slerp (enemy_.transform.rotation, newRot, enemy_.lookSpeed_ * Time.deltaTime);
        enemy_.transform.position = enemy_.planetBody_.MoveForward (enemy_.currentSpeed_);
    }
}

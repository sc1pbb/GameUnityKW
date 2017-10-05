using UnityEngine;
using System.Collections;

public class ChaseState : IEnemyState
{
    private readonly Enemy enemy_;
    private Vector3[] path_;
    bool foundPath_;
    int targetIndex_;
    bool chasing_;
    private float distanceToTarget_;
    private Vector3 currentWaypoint_;
    private Transform randomPosistionForBuilding_;

    public ChaseState (Enemy enemy)
    {
        enemy_ = enemy;
    }

    public void StartState ()
    {
        //First i set a random speed a random distance to target
        distanceToTarget_ = enemy_.distanceToTarget_;

        if (enemy_.isAnimated_)
        {
            enemy_.animatorController_.ResetTrigger (enemy_.shootAnimationTrigger_);
            enemy_.animatorController_.ResetTrigger (enemy_.crouchShootAnimationTrigger_);
            enemy_.animatorController_.SetTrigger (enemy_.runAnimationTrigger_);
        }

        //Now get a target
        SetTarget ();
    }

    public void UpdateState ()
    {
        if (foundPath_ && chasing_)
        {
            float dist = enemy_.sphericalGrid_.GetSphericalDistance (enemy_.transform.position, currentWaypoint_);

            if (dist <= distanceToTarget_)
            {
                targetIndex_++;
                if (targetIndex_ >= path_.Length)
                {
                    ToAttackState ();
                    chasing_ = false;
                }

                //Break in case we have reached the target
                if (chasing_)
                    currentWaypoint_ = path_[targetIndex_];

                //Debugging
                enemy_.gizmosTargetIndex_ = targetIndex_;

            }

            MoveTowards (currentWaypoint_);
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

    }

    public void ToAttackState ()
    {
        enemy_.ChangeState (enemy_.enemyAttackState_);
    }

    /// <summary>
    /// Set a new target. 
    /// If changeTarget is true then we will look for another target
    /// If it is false then we had a problem with one of its position, be we will try to get it again.
    /// </summary>
    /// <param name="changeTarget"></param>
    void SetTarget (bool changeTarget = true)
    {
        //Let's check if our object is not on a notwalkable point.
          NodeType currentNode = enemy_.sphericalGrid_.GroundTypeFromWorldPoint (enemy_.transform.position);
          if (currentNode == NodeType.NotWalkable)
          {
            // enemy_.ChangeState (enemy_.enemyDisabledState_);
            Debug.LogError ("I'm on a non walkable node. my pos is: " + enemy_.transform.position);
            Node startNode = enemy_.sphericalGrid_.NodeFromWorldPoint (enemy_.transform.position);
            if (startNode.nodeType == NodeType.NotWalkable)
                startNode.nodeType = NodeType.Walkable;

         //   UnityEditor.EditorApplication.isPaused = true;
          }

        //Get a random target.
        if (changeTarget)
            enemy_.currentTarget_ = EnemyBaseManager.singleton_.GetTarget ();

        //Let's say we didn't find any target
        if (enemy_.currentTarget_ == null)
        {
            //Something is wrong. Disable now
            ToDisableState ();
        }
        else
        {
            //We have found a PlanetBuildingTarget. Set one of its positions as pathnavigator's target
            //Whenever i don't find a way to that point i remove it from the list so i will have to check if the random position for building is null
            randomPosistionForBuilding_ = enemy_.currentTarget_.GetRandomPositionForPathNavigator ();

            if (randomPosistionForBuilding_ != null)
            {
                //If not null then try to find the path
                //Now we will have to check if we can do that path   
                PathRequestManager.RequestPath (enemy_.transform.position, randomPosistionForBuilding_.position, OnPathFound);
            }
            else
            {
                //If the position is null something is wrong. I will have to delete that planetbuilding and find a new target
                Debug.LogError ("I couldn't find a position for the target: " + enemy_.currentTarget_  + "so, i will have to destroy it" + "\n Position helper: " + enemy_.currentTarget_.transform.position);
              
          //      enemy_.fuckingBuilding_ = enemy_.currentTarget_;
                // UnityEditor.EditorApplication.isPaused = true;
                //  if (EnemyBaseManager.singleton_.testing_ == false)
                //      enemy_.currentTarget_.ForceDestroy ();
                ToDisableState ();
              //  SetTarget ();
            }
        }
    }

    
    public void OnPathFound (Vector3[] newPath, bool pathSuccessful)
    {
        foundPath_ = pathSuccessful;
        if (pathSuccessful && newPath.Length > 0)
        {
            path_ = newPath;
            targetIndex_ = 0;
            currentWaypoint_ = path_[targetIndex_];
            chasing_ = true;

            //Debugging
            enemy_.gizmosPath_ = path_;
            Debug.Log ("Found path. I should move");
        }
        else
        {
            //If we haven't found a path, then let's try to delete that transform and find another one
            enemy_.currentTarget_.RemoveUnreachableTransform (randomPosistionForBuilding_);
            SetTarget (false);
            Debug.LogWarning ("Haven't found a path. I will delete the position: " + enemy_.currentTarget_.enemiesPositions_.IndexOf (randomPosistionForBuilding_) + " of target called: " + enemy_.currentTarget_);
        }
    }
  
    public void MoveTowards (Vector3 targetPos)
    {
        Quaternion newRot = enemy_.planetBody_.LookAtTarget (targetPos);
        enemy_.transform.rotation = Quaternion.Slerp (enemy_.transform.rotation, newRot, enemy_.lookSpeed_ * Time.deltaTime);

        enemy_.transform.position = enemy_.planetBody_.MoveForward (enemy_.currentSpeed_);
    }
}

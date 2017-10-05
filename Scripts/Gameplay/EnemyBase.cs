using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class EnemyBase : MonoBehaviour
{
    [Header ("Units type")]
    [SerializeField] private bool spawnInfantery_; //soldiers
    [SerializeField] private bool spawnTanks_; //tanks
    [SerializeField] private bool spawnAirplanes_; //ballons, airplanes, jet

    //Type of enemies that this base can spawn
    private List<EnemyType> availableEnemiesType_ = new List<EnemyType>();

    [Header ("Enemy base activasion delay")]
    [SerializeField] private float minActivationDelay_ = 2f;
    [SerializeField] private float maxActivationDelay_ = 3f;

    [Header ("Time between waves")]
    [SerializeField] private float minTimeBetweenWaves_ = 6f; //min time between waves
    [SerializeField] private float maxTimeBetweenWaves_ = 12f; //max time between waves

    [Header ("Number of enemies per wave")]
    [SerializeField] private int minEnemiesToSpawnInWave_ = 2; //min number of enemies spawned per wave
    [SerializeField] private int maxEnemiesToSpawnInWave_ = 5; //max number of enemies spawned per wave

    [Header ("Time between enemies in a wave")]
    [SerializeField] private float minTimeBetweenEnemiesInWave_ = 0.5f; //min time between enemies spawn in wave
    [SerializeField] private float maxTimeBetweenEnemiesInWave_ = 1.5f; //min time between enemies spawn in wave

    [Header ("Spawning Animation")]
    [SerializeField] private Transform enemyStartPoint_;
    [SerializeField] private Transform enemyGoToPoint_; 
    [SerializeField] private Transform door_;

    
    void Awake ()
    {
        //First get all available enemies type for this base
     //   availableEnemiesType_ = SetAvailableEnemiesType ();
    }

    void Start ()
    {
        availableEnemiesType_ = SetAvailableEnemiesType ();
        StopCoroutine (StartSpawning ());
        StartCoroutine (StartSpawning ());
    }

    List <EnemyType> SetAvailableEnemiesType ()
    {
        List<EnemyType> availableEnemies = new List<EnemyType> ();
        //This is harcorded. It can be replaced with a bits writing
        if (spawnInfantery_)
            availableEnemies.Add (EnemyType.SOLDIER);

        if (spawnTanks_)
            availableEnemies.Add (EnemyType.TANK);

        if (spawnAirplanes_)
            availableEnemies.Add (EnemyType.AIRPLANE);
      
        return availableEnemies;
    }

    IEnumerator StartSpawning ()
    {
        float randomTimeToDelayActivation = Random.Range (minActivationDelay_, maxActivationDelay_);

        yield return new WaitForSeconds (randomTimeToDelayActivation);
        Debug.Log ("Waited: " + randomTimeToDelayActivation);

        while (GameManager.singleton_.gameOver_ == false)
        {
            Debug.LogError ("SPAWNING");
            //After i wait some seconds for activation i will start spawning the wave
            //First set time between enemies spawn and the number of enemies that has to be spawned
            //float timeBetweenEnemiesSpawn = Random.Range (minTimeBetweenEnemiesInWave_, maxTimeBetweenEnemiesInWave_); - i randomized this and put it in the for loop.
            int numberOfEnemiesToSpawn = (int)Random.Range (minEnemiesToSpawnInWave_, maxEnemiesToSpawnInWave_);
            //Debug.Log ("Spawning: " + numberOfEnemiesToSpawn);

            for (int i=1; i<= numberOfEnemiesToSpawn; i++)
            {
                SpawnEnemy ();
                float timeBetweenEnemiesSpawn = Random.Range (minTimeBetweenEnemiesInWave_, maxTimeBetweenEnemiesInWave_);
                yield return new WaitForSeconds (timeBetweenEnemiesSpawn);
            }

            //Now wait for the next wave
            float timeForNextWave = Random.Range (minTimeBetweenWaves_, maxTimeBetweenWaves_);
            Debug.Log ("Now i'm waiting: " + timeForNextWave + " for the next wave.");
            yield return new WaitForSeconds (timeForNextWave);
        }

        
    }

    void SpawnEnemy ()
    {
        //Choose a random type.
        int randomEnemyType = (int)Random.Range (0, availableEnemiesType_.Count);
        Enemy newEnemy = EnemyBaseManager.singleton_.GetFirstEnemyAvaible ((EnemyType)availableEnemiesType_[randomEnemyType]);
        newEnemy.EnableEnemy (enemyStartPoint_.position, enemyGoToPoint_.position);

        //Play animation for this building
        switch (enemyBaseAnimationType_)
        {
            case EnemyBaseAnimationType.DOOR:
                Debug.Log ("AAAAAAAAA");
                Sequence doorOpeningSequnce = DOTween.Sequence ();

                float normalScale = door_.transform.localScale.y;
                doorOpeningSequnce.Append (door_.DOScaleZ (0.1f, 0.25f).SetEase (Ease.InSine));
                doorOpeningSequnce.Append (door_.DOScaleZ (normalScale, 0.25f).SetEase (Ease.InSine).SetDelay (2f));
                break;

            default:
                break;
        }
    }
    [SerializeField] private EnemyBaseAnimationType enemyBaseAnimationType_;

    #region DEBUGGING
    private List<Vector3> allPositions = new List<Vector3> ();
    int currentPosToSearch = 0;

    int realPOS_;
    void Update ()
    {
        if (Input.GetKeyDown (KeyCode.B))
        {
            currentPosToSearch = 0;
            foreach (PlanetBuilding pb in EnemyBaseManager.singleton_.targets_)
            {
                foreach (Transform t in pb.enemiesPositions_)
                {
                    foreach (PlanetBuilding pbb in EnemyBaseManager.singleton_.targets_)
                    {
                        foreach (Transform tt in pb.enemiesPositions_)
                        {
                            allPositions.Add (tt.position);
                            PathRequestManager.RequestPath (this.transform.position, allPositions[currentPosToSearch], PATHFOUND);
                            currentPosToSearch++;
                        }
                    }

                }
            }


        }

        if (Input.GetKeyDown (KeyCode.N))
        {
            PathRequestManager.RequestPath (this.transform.position, allPositions[currentPosToSearch], PATHFOUND);
            currentPosToSearch++;
        }
    }

    void PATHFOUND (Vector3[] newPath, bool succes)
    {
        if (succes)
        {
            Debug.Log ("Found !");
        }
        else
            Debug.LogError ("HAVEN'T FOUDN PATH TO :  " + newPath[newPath.Length - 1]);

        realPOS_++;
    }

    #endregion
}

public enum EnemyBaseAnimationType
{
    DOOR,
    NOTHING
}

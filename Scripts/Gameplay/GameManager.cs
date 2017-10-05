using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton_ { get; private set; }
    public bool gameStarted_;
    public bool gameOver_;

    public float planetMaxHealth_;
    public float planetCurrentHealth_;

    public List<Enemy> enemiesOnScreen_ = new List<Enemy>();
    public Collider [] enemiesOnScreenColliders_;
    [SerializeField] private Transform cameraTransform_;
    [SerializeField] private Transform planetTransform_;
    public LayerMask enemiesLayerMask_;

    public const int NUMBER_OF_COINS_PER_AD = 20;

    [Header ("Бонусы")]
    public int maxNumberOfItems_ = 5;
    private int currentNumberOfSpawnedItems_ = 0;

    [SerializeField] private float minTimeBetweenBonusItemsSpawn_ = 5f;
    [SerializeField] private float maxTimeBetweenBonusItemsSpawn_ = 15f;

    [SerializeField] private List<BonusItemSpawnPoint> bonusItemsPositions_ = new List<BonusItemSpawnPoint> ();

    public BonusItem coinBonusItem_;
    public BonusItem hpBonusItem_;

    const int numberOfCoinsForBonusItem = 5;
    const int numberOfHPforBonusItem = 5;

    public LayerMask bonusItemLayerMask_;

    [SerializeField] private GameObject planetExplosionPrefab_;

    [Header ("Счет")]
    public int score_;

    [Header ("Вибрация камеры")]
    public bool enabledCameraShake_ = true;
    [SerializeField] private float cameraShakeDuration_ = 0.4f;
    [SerializeField] private float cameraShakeStrenght_ = 10f;
    [SerializeField] private int cameraShakeVibrato_ = 20;
    [SerializeField] private float cameraShakeRandomness_ = 90f;

    void Awake ()
    {
        if (singleton_ == null)
            singleton_ = this;
    }

    void Start ()
    {
        gameStarted_ = true;
        gameOver_ = false;
        score_ = 0;
        Time.timeScale = 1f;
        GameSceneUIController.singleton_.UpdateScoreTextUI (score_, false);

        //Установка жизини противников_
        planetMaxHealth_ = 0;
        PlanetBuilding[] planetBuildings = FindObjectsOfType<PlanetBuilding> ();
        for (int i = 0; i < planetBuildings.Length; i++)
        {
            planetMaxHealth_ += planetBuildings[i].maxHealth_;
        }

        planetCurrentHealth_ = planetMaxHealth_;

        StartCoroutine (SpawnBonusItemsCoroutine ());
        currentNumberOfSpawnedItems_ = 0;

        if (SoundManager.singleton_ != null)
        {
            SoundManager.singleton_.PlayBackgroundTheme (SoundManager.singleton_.battleSound_);
        }
    }

    void Update ()
    {
        if (Input.GetKeyDown (KeyCode.Z))
        {
            Time.timeScale += 5f;
        }

        if (Input.GetKeyDown (KeyCode.V))
        {
            EnableCameraShake ();
        }

        if (EnemyBaseManager.singleton_.targets_.Count == 0)
        {
            GameOver ();
        }

        if (Input.GetMouseButtonDown (0))
        {
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            
            RaycastHit hit;
            if (Physics.Raycast (ray, out hit, 300, bonusItemLayerMask_))
            {
                if (hit.transform.tag == "BonusItem")
                {
                    BonusItem hitBonusItem = hit.transform.parent.GetComponent<BonusItem> ();
                    hitBonusItem.ActivateBonusItem ();
                    currentNumberOfSpawnedItems_--;
                    //Активация бонуса 
                    ActivateBonusByType (hitBonusItem.bonusItemType_);

                    if (SoundManager.singleton_ != null)
                    {
                        SoundManager.singleton_.SetAudioClipToSource (hitBonusItem.audioSource_, SoundManager.singleton_.powerUpSound_);
                    }
                }
            }
        }
    }
    
    private IEnumerator SpawnBonusItemsCoroutine ()
    {
        while (gameOver_ == false)
        {
            yield return new WaitForSeconds (Random.Range (minTimeBetweenBonusItemsSpawn_, maxTimeBetweenBonusItemsSpawn_));

            if (currentNumberOfSpawnedItems_ <= maxNumberOfItems_)
            {
                SpawnBonusItem ();
            }
        }
    }

    private void SpawnBonusItem ()
    {
        //рандомный тип
        // проверка бонуса
        List<BonusItemSpawnPoint> availableBonusItemSpawnPoints = new List<BonusItemSpawnPoint> ();
        foreach (BonusItemSpawnPoint bonusSP in bonusItemsPositions_)
        {
            if (bonusSP.taken_ == false)
                availableBonusItemSpawnPoints.Add (bonusSP);
        }

        if (availableBonusItemSpawnPoints.Count > 0)
        {
            //Выбор рандомной точки
            BonusItemSpawnPoint chosenSpawnPoint = availableBonusItemSpawnPoints[(int)Random.Range (0, availableBonusItemSpawnPoints.Count)];

            int randomBonusItemType = Random.Range (0, 2);

            BonusItem newBonusItem = null;
            if (randomBonusItemType == 0)
            {
                //Бонус HP
                newBonusItem = (BonusItem)Instantiate (hpBonusItem_, Vector3.zero, Quaternion.identity);
            }

            if (randomBonusItemType == 1)
            {
                //Бонус Монет
                newBonusItem = (BonusItem)Instantiate (coinBonusItem_, Vector3.zero, Quaternion.identity);
            }

            newBonusItem.transform.up = chosenSpawnPoint.transform.up;
            newBonusItem.transform.position = chosenSpawnPoint.transform.position;
            newBonusItem.spawnPoint_ = chosenSpawnPoint;
            chosenSpawnPoint.taken_ = true;

            currentNumberOfSpawnedItems_++;
        }
        else
            Debug.LogWarning ("Не нашло бонусов");
    }

    void ActivateBonusByType (BonusItemType itemType)
    {
        if (itemType == BonusItemType.COIN)
        {
            GameSceneUIController.singleton_.ShakeCoinImage ();
            AddScore (numberOfCoinsForBonusItem);
            AddCoins (numberOfCoinsForBonusItem);
        }

        if (itemType == BonusItemType.HP)
        {
            GameSceneUIController.singleton_.ShakeHeartImage ();
            PlanetBuilding randomPlanet = EnemyBaseManager.singleton_.GetTarget ();
            if (randomPlanet.isAlive_)
            {
                randomPlanet.maxHealth_ += numberOfHPforBonusItem;
                randomPlanet.currentHealth_ += numberOfHPforBonusItem;
            }
        }
    }

    void GameOver ()
    {
        if (gameOver_ == false)
        {
            Instantiate (planetExplosionPrefab_, Vector3.zero, Quaternion.identity);
            gameOver_ = true;
            Time.timeScale = 0f;
            GameSceneUIController.singleton_.ActivateGameOverPanel ();
            int highestScore = SaveSystemManager.GetHighesScore ();
            Debug.LogError ("РЕКОРД: " + highestScore);
            GameSceneUIController.singleton_.UpdateGameOverScoreTextUI (score_, score_ > highestScore ? true : false);

            if (score_ > highestScore)
                SaveSystemManager.SetNewHighscore (score_);

            if (SoundManager.singleton_ != null)
            {
                SoundManager.singleton_.PlayBackgroundTheme (SoundManager.singleton_.gameOverSound_);
            }

            if (SaveSystemManager.GetGamesPlayed () % 3 == 0 && SaveSystemManager.GetIfPaidForAds () == false)
            {
               // AdsManager.singleton_.ShowSimpleAd ();
            }
        }
    }

    public void UpdatePlanetHealth (float damage)
    {
        planetCurrentHealth_ -= damage;
        GameSceneUIController.singleton_.UpdatePlanetHealthUI (planetCurrentHealth_ / planetMaxHealth_);

        if (planetCurrentHealth_ <= 0)
        {
            planetCurrentHealth_ = 0;
            GameOver ();
            GameSceneUIController.singleton_.UpdatePlanetHealthUI (planetCurrentHealth_ / planetMaxHealth_);
        }
    }

    public void KillObject()
    {
        //Destroy();

    }

    public void DestroyVisibleEnemies()
    {

        //Все коллайдеры бокса
        if (SoundManager.singleton_ != null)
        {
            //    SoundManager.singleton_.PlaySoundFX (SoundManager.singleton_.blowSound_);
        }

        Vector3 collisionBoxPosition = (cameraTransform_.position - planetTransform_.position) / 2f;
        Quaternion collisionBoxRotation = cameraTransform_.rotation;
        Vector3 boxCollisionSize = Vector3.one * Vector3.Distance(planetTransform_.position, cameraTransform_.position);
        enemiesOnScreenColliders_ = null;
        enemiesOnScreenColliders_ = Physics.OverlapBox (collisionBoxPosition, boxCollisionSize / 2f, collisionBoxRotation, enemiesLayerMask_);

        enemiesOnScreen_.Clear();

        foreach (Collider enemyCollider in enemiesOnScreenColliders_)
        {
            //Класс врага
            Enemy enemyRefrence = enemyCollider.transform.parent.GetComponent<Enemy>();

            if (enemyRefrence !=null)
                enemiesOnScreen_.Add(enemyRefrence);
        }

        foreach (Enemy enemy in enemiesOnScreen_)
        {
            if (enemy.isActive_ == true)
                enemy.ChangeState(enemy.enemyDisabledState_);
        }
    }

    public void EnableCameraShake ()
    {
        cameraTransform_.parent.DOShakeRotation (cameraShakeDuration_, cameraShakeStrenght_, cameraShakeVibrato_, cameraShakeRandomness_);
    }

    public void AddCoins (int coinsValue)
    {
        SaveSystemManager.UpdatePlayerData (coinsValue);
    }

    public void AddScore (int scoreValue)
    {
        score_ += scoreValue;
        GameSceneUIController.singleton_.UpdateScoreTextUI (score_);
    }

    void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.red;
        float distance = Vector3.Distance (planetTransform_.position, cameraTransform_.position);
        Vector3 boxSize = Vector3.one * distance;
        DrawCubeWithRotation ((cameraTransform_.position - planetTransform_.position) / 2f, cameraTransform_.rotation, boxSize);
    }

    public void DrawCubeWithRotation (Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Matrix4x4 cubeTransform = Matrix4x4.TRS (position, rotation, scale);
        Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

        Gizmos.matrix *= cubeTransform;

        Gizmos.DrawWireCube (Vector3.zero, Vector3.one);

        Gizmos.matrix = oldGizmosMatrix;
    }
}


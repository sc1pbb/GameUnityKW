using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBaseManager : MonoBehaviour
{
    public static EnemyBaseManager singleton_ { get; private set; }
    public SphericalGrid sphericalGrid_;
    public Transform planetTransform_;
    [SerializeField] List<Enemy> enemiesPrefabs_ = new List<Enemy> (System.Enum.GetValues (typeof(EnemyType)).Length);
    [SerializeField] private List <Enemy> enemiesSpawned_ = new List<Enemy>();

    public List<PlanetBuilding> targets_ = new List<PlanetBuilding> ();

    public bool testing_ = false;

    void Awake ()
    {
        if (singleton_ == null)
            singleton_ = this;

        PlanetBuilding [] buildings = FindObjectsOfType<PlanetBuilding> ();
        foreach (PlanetBuilding b in buildings)
        {
            targets_.Add (b);
        }

        if (sphericalGrid_ == null)
        {
            Debug.LogWarning ("4");
            sphericalGrid_ = FindObjectOfType<SphericalGrid> ();

            if (sphericalGrid_ == null)
            {
                Debug.LogError ("5 !");
            }
        }
    }

    public Enemy GetFirstEnemyAvaible (EnemyType enemyType)
    {
        //Видимость
        foreach (Enemy enemy in enemiesSpawned_)
        {
            if (enemy.enemyType_ == enemyType && enemy.isActive_ == false)
                return enemy;
        }

        //Спавн одного префаба
        Enemy enemyToSpawn = enemiesPrefabs_[0];
        //Ищем тип
        foreach (Enemy enemyByTpe in enemiesPrefabs_)
        {
            if (enemyByTpe.enemyType_ == enemyType)
                enemyToSpawn = enemyByTpe;
        }

        //Спавн нужного типа
        enemyToSpawn = (Enemy) Instantiate (enemyToSpawn, Vector3.zero, Quaternion.identity);
        enemiesSpawned_.Add (enemyToSpawn);

        return enemyToSpawn;
    }

    public PlanetBuilding GetTarget ()
    {
        if (targets_.Count > 0)
            return targets_[Random.Range (0, targets_.Count)];
        else
            return null;
    }
}

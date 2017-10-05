using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent (typeof (AudioSource))]
public class PlanetBuilding : MonoBehaviour
{
    public bool isAlive_ = true;
    public float maxHealth_ = 100;
    public float currentHealth;
    public float currentHealth_ { set
        {
            currentHealth = value;
            if (targetHealthImageUI_ != null)
                targetHealthImageUI_.fillAmount = currentHealth / maxHealth_;
        } get { return currentHealth; } }
    [SerializeField] private Image targetHealthImageUI_;

   
    [SerializeField] public List<Transform> enemiesPositions_;

    [SerializeField] private GameObject particleSystemExplosion_;

    [SerializeField] private GameObject meshContainer_;
    [SerializeField] private GameObject debris_;
    [SerializeField] private float minDebrisToSpawn_ = 5;
    [SerializeField] private float maxDebirsToSpawn_ = 8;
    [SerializeField] private Transform buildingPlatform_;

    private AudioSource audiosource_;
    
    void Awake ()
    {
        audiosource_ = this.GetComponent<AudioSource> ();
    }

    void Start ()
    {
        currentHealth_ = maxHealth_;
    }

    public void AttackBuilding (float damage)
    {
        if (isAlive_)
        {
            float amountOfDamageToAddToThePlanet = damage;
            currentHealth_ -= damage;
            if (currentHealth_ <= 0)
            {
                amountOfDamageToAddToThePlanet = damage + currentHealth_; 
                DestroyBuilding ();
            }

            GameManager.singleton_.UpdatePlanetHealth (amountOfDamageToAddToThePlanet);
              
        }
    }

    public Transform GetRandomPositionForPathNavigator ()
    {
        if (enemiesPositions_.Count > 0)
            return enemiesPositions_[(int)Random.Range (0, enemiesPositions_.Count)];
        else
            return null;
    }

    //Force destroy this building. it couldn't be reached.
    public void ForceDestroy ()
    {
        AttackBuilding (maxHealth_);
    }

    public void RemoveUnreachableTransform (Transform position)
    {
        enemiesPositions_.Remove (position);
    }
   
    void DestroyBuilding ()
    {
        isAlive_ = false;

        //Удаление зданий в листе
        EnemyBaseManager.singleton_.targets_.Remove (this);

        if (particleSystemExplosion_ != null)
            particleSystemExplosion_ = (GameObject)Instantiate (particleSystemExplosion_, this.transform.position, Quaternion.identity);

        if (debris_ != null)
        {
            foreach (Transform t in meshContainer_.transform)
            {
                t.gameObject.SetActive (false);
            }

            int randomNumberOfDebris = (int)Random.Range (minDebrisToSpawn_, maxDebirsToSpawn_);

            for (int i = 0; i < randomNumberOfDebris; i++)
            {
                GameObject Ndebree_ = (GameObject)Instantiate (debris_, meshContainer_.transform.position, meshContainer_.transform.rotation);
                Ndebree_.transform.localScale *= Random.Range (0.8f, 1.2f);

                Ndebree_.transform.DOMove (this.transform.up * Random.Range (3f, 5f) + this.transform.right * Random.Range (-4f, 4f), Random.Range (0.4f, 0.6f));
                Ndebree_.transform.DOScale (0, Random.Range (0.8f, 1.2f));
                Ndebree_.transform.DORotate (new Vector3 (Random.Range (0, 360f), Random.Range (0, 360f), Random.Range (0, 360f)), Random.Range (1, 1.5f));
            }

            if (buildingPlatform_ != null)
                buildingPlatform_.gameObject.SetActive (true);
        }
        else
            this.gameObject.SetActive (false);

        if (SoundManager.singleton_ != null)
        {
            SoundManager.singleton_.SetAudioClipToSource (audiosource_, SoundManager.singleton_.buildingExplosionSound_, true);
        }

        if (GameManager.singleton_.enabledCameraShake_)
        {
            GameManager.singleton_.EnableCameraShake ();
        }
    }

    public void PlayCollisionSpeedAnimation ()
    {
        this.transform.DOShakeRotation (1f, -this.transform.up * 15f, 10, 90);
    }


    void Update ()
    {
        if (Input.GetKeyDown (KeyCode.J))
            DestroyBuilding ();
    }

    #region EDITOR
   
    public void SetEnemiesPositionsOnSphere (Transform planetTransform, LayerMask layerMaskToCheck)
    {
        Debug.Log ("Настройка врагов на сфере");
        foreach (Transform t in enemiesPositions_)
        {
            t.position = GroundPosition (t.position, planetTransform, layerMaskToCheck);
        }
    }

    public void SetObstacleOnSphere (Transform obstacle, Transform planetTransform, LayerMask layerMaskToCheck)
    {
        obstacle.position = GroundPosition (obstacle.position, planetTransform, layerMaskToCheck);
    }

    public Vector3 GroundPosition (Vector3 currentPosition, Transform planetTransform, LayerMask layerMaskToCheck)
    {
        Vector3 dir = (planetTransform.position - currentPosition).normalized;
        Vector3 startRayPos = -dir * (3 * 1.1f);

        Ray ray = new Ray ();
        ray.origin = startRayPos;
        ray.direction = dir;

      
        RaycastHit hit;
        Debug.Log ("Имя слоя: " + LayerMask.LayerToName (layerMaskToCheck));
        //Debug.DrawRay(startRayPos,  dir * radius);
        //Debug.Break();
        if (Physics.Raycast (startRayPos, dir, out hit, (3 * 1.1f), 1 << layerMaskToCheck))
        {
            return hit.point;
        }

        return currentPosition;
    }

    #endregion
}

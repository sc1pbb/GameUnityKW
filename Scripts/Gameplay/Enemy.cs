using UnityEngine;
using System.Collections;
using DG.Tweening;

[RequireComponent (typeof (AudioSource))]
public class Enemy : MonoBehaviour
{
    public bool isActive_; // пулл активных объеков
    [Header ("Типы врагов")]
    public EnemyType enemyType_;
 //   public PlanetBuilding fuckingBuilding_;
    public IEnemyState currentEnemyState_;
    public ChaseState enemyChaseState_;
    public DisabledState enemyDisabledState_;
    public AttackState enemyAttackState_;
    public EnablingState enemyEnablingState_;

    public Vector3 normalScale_;

    [Header ("Цель врагов")]
    public PlanetBuilding currentTarget_;

    [Header ("Физика")]
    public PlanetBody planetBody_;
    public SphericalGrid sphericalGrid_;

    [Header ("Опции врагов")]
    [Range (0, 5)]
    [SerializeField]
    private float minSpeed_ = 0.2f;
    [Range (0, 5)]
    [SerializeField]
    private float maxSpeed_ = 0.8f;

    public float lookSpeed_ = 2f;

    [Range (0, 5)]
    [SerializeField]
    private float minDistanceToTarget_ = 0.1f;
    [Range (0, 5)]
    [SerializeField]
    private float maxDistanceToTarget_ = 1f;

    public float damage_ = 5f;
    public float cooldown_ = 3f;

    public float currentSpeed_;

    public float distanceToTarget_
    {
        get { return Random.Range (minDistanceToTarget_, maxDistanceToTarget_); }
        set { }
    }

    [Header ("Включаем анимацию")] 
    public bool playingEnableAnimation_;
    public Transform agentMesh_;
 
    [HideInInspector] public Vector3 enableAnimationEndPoint_;

    [Header ("Целевая анимация")]
    public bool enableLookAtTargetAnimation_ = true;
    public Transform objectToRotateToTarget_;
    public float rotationAnimationSpeed_ = 1f;

    [Header ("Частицы")]
    public GameObject shotParticle_;

    [Header ("Снаряды")]
    public float projectileForce_ = 3f;
    public ProjectileType projectileType_;
    public Transform projectileStartPosition_;

    [Header ("Анимация тряски")]
    public bool enableShakeAnimation_ = false;
    public float shakeDuration_ = 0.5f;
    public float shakeStrength_ = 0.04f;
    public int shakeVibrato_ = 3;
    public float shakeRandomness_ = 90f;

    [Header ("Спиральная анимация")]
    public bool enableSpiralAnimation_ = false;
    public float spiralDuration_ = 0.5f;
    public SpiralMode spiralMode_ = SpiralMode.Expand;
    public float spiralSpeed_ = 3f;
    public float spiralFrequency_ = 10f;
    public float spiralDepth_ = 5f;
    public Ease spiralEaseType_;
    public float scaleDuration_ = 0.5f;
    public Ease scaleEaseType_;

    [Header ("Новая спиральная анимация")]
    [HideInInspector] public bool deadRotation_ = false;
    [HideInInspector] public Vector3 randomRotationDirection_;
    [HideInInspector] public float rotationSpeed_ = 600f;

    [Header ("Анимированные или нет")]
    public bool isAnimated_ = false;
    public Animator animatorController_;
    public string runAnimationTrigger_ = "Run";
    public string shootAnimationTrigger_ = "Shoot";
    public string crouchShootAnimationTrigger_ = "CrouchShoot";

    [Header ("Звуки")]
    public AudioSource audioSource_;

    [Header ("Значение монет")]
    [SerializeField]
    public int valueInCoins_ = 1;

    //Debugging
    [HideInInspector] public Vector3[] gizmosPath_;
    [Header ("Отладка")]
    public bool drawPath_;
    [HideInInspector] public int gizmosTargetIndex_;



    void Awake ()
    {
        planetBody_ = this.GetComponent<PlanetBody> ();
        sphericalGrid_ = EnemyBaseManager.singleton_.sphericalGrid_;
        normalScale_ = this.transform.localScale;
        audioSource_ = this.GetComponent<AudioSource> ();

        enemyChaseState_ = new ChaseState (this);
        enemyDisabledState_ = new DisabledState (this);
        enemyAttackState_ = new AttackState (this);
        enemyEnablingState_ = new EnablingState (this);
    }

    public void EnableEnemy (Vector3 startPosition, Vector3 endPosition)
    {
        this.transform.position = startPosition;
        this.transform.localScale = normalScale_;
        this.transform.forward = (endPosition - startPosition).normalized;
        this.gameObject.SetActive (true);

        deadRotation_ = false;

        currentSpeed_ = Random.Range (minSpeed_, maxSpeed_);

        enableAnimationEndPoint_ = endPosition;    

        isActive_ = true;
        ChangeState (enemyEnablingState_);
    }

    void Update ()
    {
        if (isActive_)
             currentEnemyState_.UpdateState ();

        if (deadRotation_)
            this.transform.Rotate (850f * randomRotationDirection_ * Time.deltaTime);
    }
    

    public void ChangeState (IEnemyState toState)
    {
        currentEnemyState_ = toState;
        currentEnemyState_.StartState ();
    }

    public void OnDrawGizmos ()
    {
        if (gizmosPath_ != null && drawPath_)
        {
            for (int i = gizmosTargetIndex_; i < gizmosPath_.Length; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube (gizmosPath_[i], Vector3.one * 0.02f);

                if (i == gizmosTargetIndex_)
                {
                    Gizmos.DrawLine (transform.position, gizmosPath_[i]);
                }
                else
                {
                    Gizmos.DrawLine (gizmosPath_[i - 1], gizmosPath_[i]);
                }
            }
        }
    }
}

  

public enum EnemyType
{
    TANK,
    AIRPLANE,
    SOLDIER
}

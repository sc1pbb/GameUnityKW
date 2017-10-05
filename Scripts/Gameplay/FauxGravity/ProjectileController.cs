using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class ProjectileController : MonoBehaviour
{
    public bool isActive_;
    public ProjectileType projectileType_;
    public FauxGravityBody fauxGravityBody_;
    private Rigidbody rigidbody_;

    [SerializeField] private ParticleSystem projectileExplosion_;

    void Awake ()
    {
        rigidbody_ = this.GetComponent<Rigidbody> ();
        fauxGravityBody_ = this.GetComponent<FauxGravityBody> ();
        fauxGravityBody_.fauxGravityAttractor_ = FindObjectOfType<FauxGravityAttractor> ();
    }

    public void EnableProjectile (Vector3 position, Vector3 direction, float force, ForceMode forceMode = ForceMode.Impulse)
    {
        isActive_ = true;
        this.transform.position = position;
        rigidbody_.constraints = RigidbodyConstraints.FreezeRotation;
        rigidbody_.useGravity = false;
        rigidbody_.isKinematic = false;

        rigidbody_.AddForce (direction * force, forceMode);

        Invoke ("DisableProjectile", 3f);
    }

    void OnTriggerEnter (Collider other)
    {
        if (other.tag == "BuildingCollisionBox")
        {
            Debug.Log ("Collided !!!");
            DisableProjectile ();
        }
    }

    void DisableProjectile ()
    {
        isActive_ = false;
        projectileExplosion_.Play ();
        this.fauxGravityBody_.enabled = false;
        this.rigidbody_.isKinematic = true;
    }
}

public enum ProjectileType
{
    BULLET,
    SHELL,
    ROCKET
}

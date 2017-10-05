using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class FauxGravityBody : MonoBehaviour
{
    public Rigidbody rigidbody_;
    public FauxGravityAttractor fauxGravityAttractor_;

    void Awake ()
    {
        rigidbody_ = this.GetComponent<Rigidbody> ();

        rigidbody_.constraints = RigidbodyConstraints.FreezeRotation;
        rigidbody_.useGravity = false;
    }

    void Update ()
    {
        fauxGravityAttractor_.Attract (this);
    }
}

using UnityEngine;
using System.Collections;

public class FauxGravityAttractor : MonoBehaviour
{
    public float gravity_ = -10;

    public void Attract (FauxGravityBody body)
    {
        Vector3 gravityUp = (body.transform.position - this.transform.position).normalized;
        Vector3 bodyUp = body.transform.up;

        body.rigidbody_.AddForce (gravityUp * gravity_);

        Quaternion targetRotation = Quaternion.FromToRotation (bodyUp, gravityUp) * body.transform.rotation;
        body.transform.rotation = Quaternion.Slerp (body.transform.rotation, targetRotation, 50 * Time.deltaTime);
    }
}

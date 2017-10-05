using UnityEngine;
using System.Collections;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private float rotationSpeed_ = 60f;

    void Update ()
    {
        this.transform.Rotate (Vector3.up * rotationSpeed_ * Time.deltaTime);
    }
}

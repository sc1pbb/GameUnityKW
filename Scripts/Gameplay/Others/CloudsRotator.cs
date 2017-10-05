using UnityEngine;
using System.Collections;

public class CloudsRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed_ = 5f;
    private Vector3 randomRotationDirection_;

    void Start ()
    {
        randomRotationDirection_ = new Vector3 (Random.Range (00f, 1f), Random.Range (00f, 1f), Random.Range (00f, 1f));
    }

    void Update ()
    {
        this.transform.Rotate (randomRotationDirection_ * rotationSpeed_ * Time.deltaTime);
    }
}
